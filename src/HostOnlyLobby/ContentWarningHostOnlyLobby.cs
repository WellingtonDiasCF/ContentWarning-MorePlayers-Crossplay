using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.SceneManagement;
using DefaultNamespace;

[assembly: IgnoresAccessChecksTo("Assembly-CSharp")]

namespace System.Runtime.CompilerServices
{
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
    internal sealed class IgnoresAccessChecksToAttribute : Attribute
    {
        public IgnoresAccessChecksToAttribute(string assemblyName) { }
    }
}

namespace ContentWarningHostOnlyLobby
{
    [ContentWarningPlugin(PluginGuid, PluginVersion, true)]
    [BepInPlugin(PluginGuid, PluginName, PluginVersion)]
    public sealed class HostOnlyLobbyPlugin : BaseUnityPlugin
    {
        public const string PluginGuid = "local.contentwarning.hostonlylobby";
        public const string PluginName = "HostOnlyLobby";
        public const string PluginVersion = "1.2.0";

        private Harmony? _harmony;
        private bool _wasInHostRoom;
        private int _lastSceneHandle = -1;
        private float _indicatorVisibleFrom;
        private float _indicatorVisibleUntil;
        private GUIStyle? _indicatorTitleStyle;
        private GUIStyle? _indicatorDetailStyle;

        internal static int MaxPlayers { get; private set; } = 8;
        internal static int RequiredSleepers { get; private set; } = 4;
        internal static bool ShowStatusIndicator { get; private set; } = true;
        internal static float IndicatorSeconds { get; private set; } = 12f;
        internal static ManualLogSource? Log { get; private set; }

        private void Awake()
        {
            Log = Logger;
            ConfigEntry<int> maxPlayers = Config.Bind(
                "Lobby",
                "MaxPlayers",
                8,
                "Maximum lobby size. Keep between 5 and 16 for cross-platform testing.");
            ConfigEntry<bool> showStatusIndicator = Config.Bind(
                "Interface",
                "ShowStatusIndicator",
                true,
                "Show a status card when the host enters a room or loads a world.");
            ConfigEntry<float> indicatorSeconds = Config.Bind(
                "Interface",
                "IndicatorSeconds",
                12f,
                "How many seconds the status card remains visible. Keep between 3 and 30.");

            MaxPlayers = Clamp(maxPlayers.Value, 5, 16);
            RequiredSleepers = Math.Min(4, MaxPlayers);
            ShowStatusIndicator = showStatusIndicator.Value;
            IndicatorSeconds = Mathf.Clamp(indicatorSeconds.Value, 3f, 30f);

            _harmony = new Harmony(PluginGuid);
            _harmony.PatchAll();

            Logger.LogInfo($"Lobby patches active. MaxPlayers={MaxPlayers}. Extra PC clients in slots 5+ also need this plugin for safe local spawning.");
            Logger.LogInfo("Late joining is intentionally disabled; have everyone join before opening the house door.");
            Logger.LogInfo("Spawn compatibility is active on this device for house, dive-bell return, and hospital spawns.");
            Logger.LogInfo($"Status indicator enabled: {ShowStatusIndicator} ({IndicatorSeconds:0.#} seconds).");
            RunStartupDiagnostics();
        }

        private void Update()
        {
            bool inHostRoom = PhotonNetwork.InRoom && PhotonNetwork.IsMasterClient;
            int sceneHandle = SceneManager.GetActiveScene().handle;

            if (ShowStatusIndicator && inHostRoom &&
                (!_wasInHostRoom || sceneHandle != _lastSceneHandle))
            {
                _indicatorVisibleFrom = Time.realtimeSinceStartup + 1f;
                _indicatorVisibleUntil = _indicatorVisibleFrom + IndicatorSeconds;
                Logger.LogInfo($"Status indicator scheduled in scene '{SceneManager.GetActiveScene().name}'.");
            }

            _wasInHostRoom = inHostRoom;
            _lastSceneHandle = sceneHandle;
        }

        private void OnGUI()
        {
            if (!ShowStatusIndicator ||
                Time.realtimeSinceStartup < _indicatorVisibleFrom ||
                Time.realtimeSinceStartup > _indicatorVisibleUntil)
            {
                return;
            }

            EnsureIndicatorStyles();

            float width = Mathf.Min(340f, Screen.width - 32f);
            var background = new Rect(Screen.width - width - 16f, 18f, width, 72f);
            var accent = new Rect(background.x, background.y, 5f, background.height);
            var title = new Rect(background.x + 18f, background.y + 10f, background.width - 28f, 24f);
            var detail = new Rect(background.x + 18f, background.y + 36f, background.width - 28f, 24f);

            Color previousColor = GUI.color;
            GUI.color = new Color(0.055f, 0.071f, 0.102f, 0.94f);
            GUI.DrawTexture(background, Texture2D.whiteTexture);
            GUI.color = new Color(0.25f, 0.88f, 0.55f, 1f);
            GUI.DrawTexture(accent, Texture2D.whiteTexture);
            GUI.color = Color.white;
            GUI.Label(title, "HOSTONLYLOBBY ATIVO", _indicatorTitleStyle);
            GUI.Label(detail, $"Sala do host  •  até {MaxPlayers} jogadores", _indicatorDetailStyle);
            GUI.color = previousColor;
        }

        private void OnDestroy()
        {
            _harmony?.UnpatchSelf();
            Log = null;
        }

        private void EnsureIndicatorStyles()
        {
            if (_indicatorTitleStyle != null && _indicatorDetailStyle != null)
            {
                return;
            }

            _indicatorTitleStyle = new GUIStyle(GUI.skin.label)
            {
                alignment = TextAnchor.MiddleLeft,
                fontSize = 15,
                fontStyle = FontStyle.Bold
            };
            _indicatorTitleStyle.normal.textColor = Color.white;

            _indicatorDetailStyle = new GUIStyle(GUI.skin.label)
            {
                alignment = TextAnchor.MiddleLeft,
                fontSize = 12
            };
            _indicatorDetailStyle.normal.textColor = new Color(0.78f, 0.82f, 0.88f, 1f);
        }

        private static int Clamp(int value, int minimum, int maximum)
        {
            if (value < minimum) return minimum;
            if (value > maximum) return maximum;
            return value;
        }

        internal static void LogSpawnRemap(int originalIndex, int safeIndex)
        {
            Log?.LogInfo($"Spawn compatibility remapped local slot {originalIndex} to vanilla spawn {safeIndex}.");
        }

        private void RunStartupDiagnostics()
        {
            MethodBase[] targets =
            {
                AccessTools.Method(typeof(SteamLobbyHandler), nameof(SteamLobbyHandler.HostMatch)),
                AccessTools.Method(typeof(PhotonNetwork), nameof(PhotonNetwork.CreateRoom), new Type[]
                {
                    typeof(string), typeof(RoomOptions), typeof(TypedLobby), typeof(string[])
                }),
                AccessTools.PropertyGetter(typeof(InviteFriendsTerminal), "IsGameFull"),
                AccessTools.Method(typeof(RichPresenceHandler), nameof(RichPresenceHandler.SetGroupSize)),
                AccessTools.Method(typeof(BedBoss), "OnPlayerJoined"),
                AccessTools.Method(typeof(PlayerHandler), "AllPlayersInBed"),
                AccessTools.Method(typeof(PlayerHandler), nameof(PlayerHandler.AllPlayersAsleep)),
                AccessTools.Method(typeof(SpawnHandler), "FindLocalSpawnIndex")
            };

            int patched = 0;
            foreach (MethodBase target in targets)
            {
                Patches? info = Harmony.GetPatchInfo(target);
                bool owned = false;

                if (info != null)
                {
                    foreach (string owner in info.Owners)
                    {
                        if (owner == PluginGuid)
                        {
                            owned = true;
                            break;
                        }
                    }
                }

                if (owned)
                {
                    patched++;
                }
                else
                {
                    Logger.LogError($"Startup diagnostic failed: patch missing on {target.DeclaringType?.Name}.{target.Name}.");
                }
            }

            if (patched == targets.Length)
            {
                Logger.LogInfo($"Startup diagnostics passed: {patched}/{targets.Length} host-side patches registered.");
            }
        }
    }

    internal static class HostOnlyLobbyRules
    {
        internal static byte PhotonRoomLimit => 0;

        internal static bool IsAtCapacity(int playerCount)
        {
            return playerCount >= HostOnlyLobbyPlugin.MaxPlayers;
        }

        internal static int RichPresenceMaximum(int requestedMaximum)
        {
            return requestedMaximum <= 4 ? HostOnlyLobbyPlugin.MaxPlayers : requestedMaximum;
        }

        internal static int BedAssignmentCount(int bedCount, int playerCount)
        {
            return Math.Min(4, Math.Min(bedCount, playerCount));
        }

        internal static bool EnoughPlayersReady(int alivePlayers, int readyPlayers)
        {
            int required = Math.Min(HostOnlyLobbyPlugin.RequiredSleepers, alivePlayers);
            return required > 0 && readyPlayers >= required;
        }

        internal static int SafeSpawnIndex(int originalIndex, int houseSpawnCount, int diveBellSpawnCount)
        {
            if (originalIndex <= 0)
            {
                return 0;
            }

            int spawnCount = Math.Min(houseSpawnCount, diveBellSpawnCount);
            if (spawnCount <= 1)
            {
                return 0;
            }

            // Spawn zero remains exclusive to the host. Extra clients rotate
            // through the remaining vanilla points instead of indexing past
            // the serialized arrays and appearing outside the map.
            return 1 + ((originalIndex - 1) % (spawnCount - 1));
        }
    }

    [HarmonyPatch(typeof(SpawnHandler))]
    [HarmonyPriority(Priority.First)]
    internal static class SpawnHandlerPatches
    {
        private static readonly FieldInfo LocalSpawnIndexField =
            typeof(SpawnHandler).GetField("m_LocalSpawnIndex", BindingFlags.Instance | BindingFlags.NonPublic)
            ?? throw new MissingFieldException(typeof(SpawnHandler).FullName, "m_LocalSpawnIndex");

        private static readonly FieldInfo HouseSpawnsField =
            typeof(SpawnHandler).GetField("m_HouseSpawns", BindingFlags.Instance | BindingFlags.NonPublic)
            ?? throw new MissingFieldException(typeof(SpawnHandler).FullName, "m_HouseSpawns");

        private static readonly FieldInfo DiveBellSpawnsField =
            typeof(SpawnHandler).GetField("m_DiveBellSpawns", BindingFlags.Instance | BindingFlags.NonPublic)
            ?? throw new MissingFieldException(typeof(SpawnHandler).FullName, "m_DiveBellSpawns");

        [HarmonyPostfix]
        [HarmonyPatch("FindLocalSpawnIndex")]
        private static void FindLocalSpawnIndexPostfix(SpawnHandler __instance)
        {
            int originalIndex = (int)LocalSpawnIndexField.GetValue(__instance);
            var houseSpawns = (Transform[]?)HouseSpawnsField.GetValue(__instance);
            var diveBellSpawns = (Transform[]?)DiveBellSpawnsField.GetValue(__instance);
            int safeIndex = HostOnlyLobbyRules.SafeSpawnIndex(
                originalIndex,
                houseSpawns?.Length ?? 0,
                diveBellSpawns?.Length ?? 0);

            if (safeIndex == originalIndex)
            {
                return;
            }

            LocalSpawnIndexField.SetValue(__instance, safeIndex);
            HostOnlyLobbyPlugin.LogSpawnRemap(originalIndex, safeIndex);
        }
    }

    [HarmonyPatch(typeof(SteamLobbyHandler))]
    internal static class SteamLobbyPatches
    {
        private static readonly FieldInfo MaxPlayersField =
            typeof(SteamLobbyHandler).GetField("m_MaxPlayers", BindingFlags.Instance | BindingFlags.NonPublic)
            ?? throw new MissingFieldException(typeof(SteamLobbyHandler).FullName, "m_MaxPlayers");

        [HarmonyPrefix]
        [HarmonyPatch(nameof(SteamLobbyHandler.HostMatch))]
        private static void HostMatchPrefix(SteamLobbyHandler __instance)
        {
            MaxPlayersField.SetValue(__instance, HostOnlyLobbyPlugin.MaxPlayers);
        }
    }

    [HarmonyPatch(typeof(PhotonNetwork))]
    internal static class PhotonRoomPatches
    {
        [HarmonyPrefix]
        [HarmonyPatch(nameof(PhotonNetwork.CreateRoom), new Type[]
        {
            typeof(string),
            typeof(RoomOptions),
            typeof(TypedLobby),
            typeof(string[])
        })]
        private static void CreateRoomPrefix(ref RoomOptions roomOptions)
        {
            roomOptions ??= new RoomOptions();

            // Photon treats zero as no explicit room cap. The Steam/host UI cap
            // remains controlled by MaxPlayers.
            roomOptions.MaxPlayers = HostOnlyLobbyRules.PhotonRoomLimit;
        }
    }

    [HarmonyPatch(typeof(InviteFriendsTerminal))]
    internal static class InviteTerminalPatches
    {
        [HarmonyPrefix]
        [HarmonyPatch("IsGameFull", MethodType.Getter)]
        private static bool IsGameFullPrefix(ref bool __result)
        {
            __result = PlayerHandler.instance != null &&
                       HostOnlyLobbyRules.IsAtCapacity(PlayerHandler.instance.players.Count);
            return false;
        }
    }

    [HarmonyPatch(typeof(RichPresenceHandler))]
    internal static class RichPresencePatches
    {
        [HarmonyPrefix]
        [HarmonyPatch(nameof(RichPresenceHandler.SetGroupSize))]
        private static void SetGroupSizePrefix(ref int maxSize)
        {
            maxSize = HostOnlyLobbyRules.RichPresenceMaximum(maxSize);
        }
    }

    [HarmonyPatch(typeof(BedBoss))]
    [HarmonyPriority(Priority.First)]
    internal static class BedPatches
    {
        [HarmonyPrefix]
        [HarmonyPatch("OnPlayerJoined")]
        private static bool OnPlayerJoinedPrefix(BedBoss __instance)
        {
            if (!PhotonNetwork.IsMasterClient || PlayerHandler.instance == null)
            {
                return false;
            }

            int assignments = HostOnlyLobbyRules.BedAssignmentCount(
                __instance.beds.Count,
                PlayerHandler.instance.players.Count);
            PhotonView view = __instance.GetComponent<PhotonView>();

            for (int i = 0; i < assignments; i++)
            {
                Player player = PlayerHandler.instance.players[i];
                view.RPC("AssignBed", RpcTarget.All, player.refs.view.ViewID, i);
            }

            return false;
        }
    }

    [HarmonyPatch(typeof(PlayerHandler))]
    internal static class SleepPatches
    {
        [HarmonyPostfix]
        [HarmonyPatch("AllPlayersInBed")]
        private static void AllPlayersInBedPostfix(PlayerHandler __instance, ref bool __result)
        {
            if (!PhotonNetwork.IsMasterClient)
            {
                return;
            }

            int inBed = 0;

            for (int i = 0; i < __instance.playersAlive.Count; i++)
            {
                if (__instance.playersAlive[i].data.currentBed != null)
                {
                    inBed++;
                }
            }

            __result = HostOnlyLobbyRules.EnoughPlayersReady(__instance.playersAlive.Count, inBed);
        }

        [HarmonyPostfix]
        [HarmonyPatch(nameof(PlayerHandler.AllPlayersAsleep))]
        private static void AllPlayersAsleepPostfix(PlayerHandler __instance, ref bool __result)
        {
            if (!PhotonNetwork.IsMasterClient)
            {
                return;
            }

            int asleep = 0;

            for (int i = 0; i < __instance.playersAlive.Count; i++)
            {
                if (__instance.playersAlive[i].data.sleepAmount >= 0.9f)
                {
                    asleep++;
                }
            }

            __result = HostOnlyLobbyRules.EnoughPlayersReady(__instance.playersAlive.Count, asleep);
        }
    }
}
