using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Loader;

if (args.Length != 2)
{
    Console.Error.WriteLine("Usage: HostOnlyLobby.SimulationTests <plugin.dll> <game-directory>");
    return 2;
}

string pluginPath = Path.GetFullPath(args[0]);
string gameDirectory = Path.GetFullPath(args[1]);
string managedDirectory = Path.Combine(gameDirectory, "Content Warning_Data", "Managed");
string bepinexDirectory = Path.Combine(gameDirectory, "BepInEx", "core");

AssemblyLoadContext.Default.Resolving += (_, assemblyName) =>
{
    foreach (string directory in new[] { Path.GetDirectoryName(pluginPath)!, managedDirectory, bepinexDirectory })
    {
        string candidate = Path.Combine(directory, assemblyName.Name + ".dll");
        if (File.Exists(candidate))
        {
            return AssemblyLoadContext.Default.LoadFromAssemblyPath(candidate);
        }
    }

    return null;
};

Assembly plugin = AssemblyLoadContext.Default.LoadFromAssemblyPath(pluginPath);
Type rules = RequiredType(plugin, "ContentWarningHostOnlyLobby.HostOnlyLobbyRules");
Type pluginType = RequiredType(plugin, "ContentWarningHostOnlyLobby.HostOnlyLobbyPlugin");
int passed = 0;
int failed = 0;

void Check(string name, bool condition, string details)
{
    if (condition)
    {
        passed++;
        Console.WriteLine($"PASS  {name}: {details}");
    }
    else
    {
        failed++;
        Console.WriteLine($"FAIL  {name}: {details}");
    }
}

object? CallRule(string name, params object?[] parameters)
{
    MethodInfo method = rules.GetMethod(name, BindingFlags.Static | BindingFlags.NonPublic)
        ?? throw new MissingMethodException(rules.FullName, name);
    return method.Invoke(null, parameters);
}

int configuredMaximum = (int)(pluginType.GetProperty("MaxPlayers", BindingFlags.Static | BindingFlags.NonPublic)?.GetValue(null)
    ?? throw new MissingMemberException(pluginType.FullName, "MaxPlayers"));
Check("configured maximum", configuredMaximum == 8, $"expected 8, actual {configuredMaximum}");

Check("fifth player accepted", !(bool)CallRule("IsAtCapacity", 5)!, "5 players do not mark an 8-slot lobby full");
Check("eighth player fills lobby", (bool)CallRule("IsAtCapacity", 8)!, "8 players mark the lobby full");
Check("four beds for five players", (int)CallRule("BedAssignmentCount", 4, 5)! == 4, "four safe bed assignments");
Check("three available beds", (int)CallRule("BedAssignmentCount", 3, 5)! == 3, "assignment count respects available beds");
Check("three of five not enough", !(bool)CallRule("EnoughPlayersReady", 5, 3)!, "sleep transition remains blocked");
Check("four of five enough", (bool)CallRule("EnoughPlayersReady", 5, 4)!, "four beds can advance a five-player group");
Check("three surviving players", (bool)CallRule("EnoughPlayersReady", 3, 3)!, "all three survivors can advance");
Check("empty group cannot advance", !(bool)CallRule("EnoughPlayersReady", 0, 0)!, "zero players remains false");
Check("host keeps spawn zero", (int)CallRule("SafeSpawnIndex", 0, 4, 4)! == 0, "host remains on its dedicated spawn");
Check("fourth client reuses safe spawn", (int)CallRule("SafeSpawnIndex", 4, 4, 4)! == 1, "fifth player reuses vanilla client spawn 1");
Check("fifth client rotates spawn", (int)CallRule("SafeSpawnIndex", 5, 4, 4)! == 2, "sixth player reuses vanilla client spawn 2");
Check("smaller array controls mapping", (int)CallRule("SafeSpawnIndex", 4, 6, 4)! == 1, "mapping respects the smaller serialized array");
Check("single spawn fallback", (int)CallRule("SafeSpawnIndex", 7, 1, 1)! == 0, "single-point scenes safely fall back to spawn zero");

Type roomOptionsType = RequiredType(
    AssemblyLoadContext.Default.LoadFromAssemblyPath(Path.Combine(managedDirectory, "PhotonRealtime.dll")),
    "Photon.Realtime.RoomOptions");
object roomOptions = Activator.CreateInstance(roomOptionsType)!;
FieldInfo maxPlayersField = roomOptionsType.GetField("MaxPlayers")
    ?? throw new MissingFieldException(roomOptionsType.FullName, "MaxPlayers");
maxPlayersField.SetValue(roomOptions, 4);
Type photonPatch = RequiredType(plugin, "ContentWarningHostOnlyLobby.PhotonRoomPatches");
MethodInfo createRoomPrefix = photonPatch.GetMethod("CreateRoomPrefix", BindingFlags.Static | BindingFlags.NonPublic)!;
object?[] roomArguments = { roomOptions };
createRoomPrefix.Invoke(null, roomArguments);
int photonLimit = Convert.ToInt32(maxPlayersField.GetValue(roomArguments[0]));
Check("Photon room uncapped", photonLimit == 0, $"room cap changed from 4 to {photonLimit}");

Type richPresencePatch = RequiredType(plugin, "ContentWarningHostOnlyLobby.RichPresencePatches");
MethodInfo richPresencePrefix = richPresencePatch.GetMethod("SetGroupSizePrefix", BindingFlags.Static | BindingFlags.NonPublic)!;
object?[] defaultPresence = { 4 };
richPresencePrefix.Invoke(null, defaultPresence);
Check("rich presence expanded", (int)defaultPresence[0]! == 8, $"advertised maximum is {defaultPresence[0]}");
object?[] explicitPresence = { 12 };
richPresencePrefix.Invoke(null, explicitPresence);
Check("explicit larger presence preserved", (int)explicitPresence[0]! == 12, $"advertised maximum remains {explicitPresence[0]}");

Assembly gameAssembly = AssemblyLoadContext.Default.LoadFromAssemblyPath(Path.Combine(managedDirectory, "Assembly-CSharp.dll"));
Type spawnHandlerType = RequiredType(gameAssembly, "SpawnHandler");
object spawnHandler = RuntimeHelpers.GetUninitializedObject(spawnHandlerType);
FieldInfo localSpawnIndexField = spawnHandlerType.GetField("m_LocalSpawnIndex", BindingFlags.Instance | BindingFlags.NonPublic)
    ?? throw new MissingFieldException(spawnHandlerType.FullName, "m_LocalSpawnIndex");
FieldInfo houseSpawnsField = spawnHandlerType.GetField("m_HouseSpawns", BindingFlags.Instance | BindingFlags.NonPublic)
    ?? throw new MissingFieldException(spawnHandlerType.FullName, "m_HouseSpawns");
FieldInfo diveBellSpawnsField = spawnHandlerType.GetField("m_DiveBellSpawns", BindingFlags.Instance | BindingFlags.NonPublic)
    ?? throw new MissingFieldException(spawnHandlerType.FullName, "m_DiveBellSpawns");
Type transformType = RequiredType(
    AssemblyLoadContext.Default.LoadFromAssemblyPath(Path.Combine(managedDirectory, "UnityEngine.CoreModule.dll")),
    "UnityEngine.Transform");
houseSpawnsField.SetValue(spawnHandler, Array.CreateInstance(transformType, 4));
diveBellSpawnsField.SetValue(spawnHandler, Array.CreateInstance(transformType, 4));
localSpawnIndexField.SetValue(spawnHandler, 4);
Type spawnPatch = RequiredType(plugin, "ContentWarningHostOnlyLobby.SpawnHandlerPatches");
MethodInfo spawnIndexPostfix = spawnPatch.GetMethod("FindLocalSpawnIndexPostfix", BindingFlags.Static | BindingFlags.NonPublic)!;
spawnIndexPostfix.Invoke(null, new[] { spawnHandler });
Check("spawn patch remaps serialized slot", Convert.ToInt32(localSpawnIndexField.GetValue(spawnHandler)) == 1,
    "fifth local player is redirected before SpawnLocalPlayer reads the arrays");

Type steamLobbyType = RequiredType(gameAssembly, "SteamLobbyHandler");
object steamLobby = RuntimeHelpers.GetUninitializedObject(steamLobbyType);
FieldInfo steamMaximumField = steamLobbyType.GetField("m_MaxPlayers", BindingFlags.Instance | BindingFlags.NonPublic)
    ?? throw new MissingFieldException(steamLobbyType.FullName, "m_MaxPlayers");
steamMaximumField.SetValue(steamLobby, 4);
Type steamPatch = RequiredType(plugin, "ContentWarningHostOnlyLobby.SteamLobbyPatches");
MethodInfo hostMatchPrefix = steamPatch.GetMethod("HostMatchPrefix", BindingFlags.Static | BindingFlags.NonPublic)!;
hostMatchPrefix.Invoke(null, new[] { steamLobby });
int steamMaximum = Convert.ToInt32(steamMaximumField.GetValue(steamLobby));
Check("Steam lobby expanded", steamMaximum == 8, $"host limit changed from 4 to {steamMaximum}");

Console.WriteLine($"RESULT: {passed} passed, {failed} failed");
return failed == 0 ? 0 : 1;

static Type RequiredType(Assembly assembly, string name)
{
    return assembly.GetType(name, throwOnError: true)!;
}
