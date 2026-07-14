using System.IO.Compression;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.Win32;

namespace HostOnlyLobbyInstaller;

internal static class InstallerEngine
{
    internal const string RepositoryUrl = "https://github.com/WellingtonDiasCF/ContentWarning-MorePlayers-Crossplay";

    private const string BepInExUrl =
        "https://thunderstore.io/package/download/BepInEx/BepInExPack/5.4.2305/";
    private const string BepInExSha256 =
        "F99B6302B6BDCA5BAB26F275C261CCA448B29BBAC34EAA5CBEBB42C4246FB244";
    private const string CrossPatcherUrl =
        "https://thunderstore.io/package/download/gingerphoenix10/CrossPatcher/1.0.0/";
    private const string CrossPatcherSha256 =
        "0652D4FA6B8FF4EA06D5BCE38892A65497DC9FE9E96F2B2EBC2A4A85E5847153";
    private const string PayloadResource = "HostOnlyLobby.Payload.ContentWarningHostOnlyLobby.dll";

    internal static async Task<string> InstallAsync(
        string gameDirectory,
        int maxPlayers,
        IProgress<string>? progress)
    {
        gameDirectory = ValidateGameDirectory(gameDirectory);
        maxPlayers = Math.Clamp(maxPlayers, 5, 16);

        string temporaryDirectory = Path.Combine(
            Path.GetTempPath(),
            "HostOnlyLobby-Setup",
            Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(temporaryDirectory);

        try
        {
            string bepinexArchive = Path.Combine(temporaryDirectory, "BepInExPack.zip");
            string crossPatcherArchive = Path.Combine(temporaryDirectory, "CrossPatcher.zip");

            progress?.Report("Baixando BepInEx 5.4.23.5...");
            await DownloadAndVerifyAsync(BepInExUrl, bepinexArchive, BepInExSha256);

            progress?.Report("Baixando CrossPatcher 1.0.0...");
            await DownloadAndVerifyAsync(CrossPatcherUrl, crossPatcherArchive, CrossPatcherSha256);

            progress?.Report("Instalando os arquivos básicos...");
            ExtractSubtree(bepinexArchive, "BepInExPack/", gameDirectory);

            string pluginsDirectory = Path.Combine(gameDirectory, "BepInEx", "plugins");
            Directory.CreateDirectory(pluginsDirectory);
            string disabledSummary = DisableConflictingViralityPlugins(pluginsDirectory);

            progress?.Report("Instalando CrossPatcher...");
            string crossPatcherDirectory = Path.Combine(pluginsDirectory, "CrossPatcher");
            Directory.CreateDirectory(crossPatcherDirectory);
            ExtractSingleFile(
                crossPatcherArchive,
                "plugins/CrossPatcher.dll",
                Path.Combine(crossPatcherDirectory, "CrossPatcher.dll"));

            progress?.Report("Instalando HostOnlyLobby 1.0.1...");
            string hostOnlyDirectory = Path.Combine(pluginsDirectory, "HostOnlyLobby");
            Directory.CreateDirectory(hostOnlyDirectory);
            string pluginPath = Path.Combine(hostOnlyDirectory, "ContentWarningHostOnlyLobby.dll");
            BackupExistingPlugin(gameDirectory, pluginPath);
            WriteEmbeddedPlugin(pluginPath);

            ConfigureBepInExConsole(gameDirectory);
            WriteLobbyConfiguration(gameDirectory, maxPlayers);

            progress?.Report("Instalação concluída.");
            return string.IsNullOrEmpty(disabledSummary)
                ? $"HostOnlyLobby instalado para até {maxPlayers} jogadores."
                : $"HostOnlyLobby instalado para até {maxPlayers} jogadores.\n\n{disabledSummary}";
        }
        finally
        {
            try
            {
                Directory.Delete(temporaryDirectory, recursive: true);
            }
            catch
            {
                // Temporary files are harmless and Windows will clear them later.
            }
        }
    }

    internal static void Uninstall(string gameDirectory)
    {
        gameDirectory = ValidateGameDirectory(gameDirectory);
        string pluginDirectory = Path.Combine(gameDirectory, "BepInEx", "plugins", "HostOnlyLobby");
        string pluginPath = Path.Combine(pluginDirectory, "ContentWarningHostOnlyLobby.dll");
        string configPath = Path.Combine(
            gameDirectory,
            "BepInEx",
            "config",
            "local.contentwarning.hostonlylobby.cfg");

        if (File.Exists(pluginPath))
        {
            File.Delete(pluginPath);
        }

        if (Directory.Exists(pluginDirectory) && !Directory.EnumerateFileSystemEntries(pluginDirectory).Any())
        {
            Directory.Delete(pluginDirectory);
        }

        if (File.Exists(configPath))
        {
            File.Delete(configPath);
        }
    }

    internal static string? FindGameDirectory()
    {
        foreach (string candidate in FindCandidateDirectories())
        {
            if (File.Exists(Path.Combine(candidate, "Content Warning.exe")))
            {
                return Path.GetFullPath(candidate);
            }
        }

        return null;
    }

    private static IEnumerable<string> FindCandidateDirectories()
    {
        var candidates = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (RegistryView view in new[] { RegistryView.Registry64, RegistryView.Registry32 })
        {
            try
            {
                using RegistryKey baseKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, view);
                using RegistryKey? gameKey = baseKey.OpenSubKey(
                    @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\Steam App 2881650");
                if (gameKey?.GetValue("InstallLocation") is string installLocation &&
                    !string.IsNullOrWhiteSpace(installLocation))
                {
                    candidates.Add(installLocation);
                }
            }
            catch
            {
                // Continue with Steam library discovery.
            }
        }

        foreach (string steamRoot in FindSteamRoots())
        {
            candidates.Add(Path.Combine(steamRoot, "steamapps", "common", "Content Warning"));
            string librariesFile = Path.Combine(steamRoot, "steamapps", "libraryfolders.vdf");
            if (!File.Exists(librariesFile))
            {
                continue;
            }

            string contents = File.ReadAllText(librariesFile);
            foreach (Match match in Regex.Matches(contents, "\\\"path\\\"\\s+\\\"(?<path>[^\\\"]+)\\\""))
            {
                string library = match.Groups["path"].Value.Replace("\\\\", "\\");
                candidates.Add(Path.Combine(library, "steamapps", "common", "Content Warning"));
            }
        }

        return candidates;
    }

    private static IEnumerable<string> FindSteamRoots()
    {
        var roots = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        try
        {
            using RegistryKey? key = Registry.CurrentUser.OpenSubKey(@"Software\Valve\Steam");
            if (key?.GetValue("SteamPath") is string steamPath && !string.IsNullOrWhiteSpace(steamPath))
            {
                roots.Add(steamPath.Replace('/', Path.DirectorySeparatorChar));
            }
        }
        catch
        {
            // Default locations below cover the common case.
        }

        roots.Add(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), "Steam"));
        return roots;
    }

    private static string ValidateGameDirectory(string gameDirectory)
    {
        if (string.IsNullOrWhiteSpace(gameDirectory))
        {
            throw new InvalidOperationException("Selecione a pasta do Content Warning.");
        }

        string fullPath = Path.GetFullPath(gameDirectory.Trim());
        if (!File.Exists(Path.Combine(fullPath, "Content Warning.exe")))
        {
            throw new InvalidOperationException(
                "Content Warning.exe não foi encontrado nessa pasta. Selecione a pasta do jogo pela Steam.");
        }

        return fullPath;
    }

    private static async Task DownloadAndVerifyAsync(string url, string destination, string expectedSha256)
    {
        using var client = new HttpClient { Timeout = TimeSpan.FromMinutes(2) };
        client.DefaultRequestHeaders.UserAgent.ParseAdd("HostOnlyLobby-Setup/1.0.1");
        using HttpResponseMessage response = await client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead);
        response.EnsureSuccessStatusCode();
        await using (Stream source = await response.Content.ReadAsStreamAsync())
        await using (FileStream target = File.Create(destination))
        {
            await source.CopyToAsync(target);
        }

        string actualSha256 = Convert.ToHexString(SHA256.HashData(File.ReadAllBytes(destination)));
        if (!actualSha256.Equals(expectedSha256, StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidDataException(
                $"A verificação de segurança do download falhou. Esperado: {expectedSha256}; recebido: {actualSha256}.");
        }
    }

    private static void ExtractSubtree(string archivePath, string prefix, string destinationRoot)
    {
        using ZipArchive archive = ZipFile.OpenRead(archivePath);
        foreach (ZipArchiveEntry entry in archive.Entries)
        {
            if (!entry.FullName.StartsWith(prefix, StringComparison.Ordinal) ||
                entry.FullName.Equals(prefix, StringComparison.Ordinal))
            {
                continue;
            }

            string relativePath = entry.FullName[prefix.Length..].Replace('/', Path.DirectorySeparatorChar);
            string destination = SafeDestination(destinationRoot, relativePath);

            if (entry.FullName.EndsWith('/'))
            {
                Directory.CreateDirectory(destination);
                continue;
            }

            Directory.CreateDirectory(Path.GetDirectoryName(destination)!);
            entry.ExtractToFile(destination, overwrite: true);
        }
    }

    private static void ExtractSingleFile(string archivePath, string entryName, string destination)
    {
        using ZipArchive archive = ZipFile.OpenRead(archivePath);
        ZipArchiveEntry entry = archive.GetEntry(entryName)
            ?? throw new InvalidDataException($"O pacote não contém {entryName}.");
        Directory.CreateDirectory(Path.GetDirectoryName(destination)!);
        entry.ExtractToFile(destination, overwrite: true);
    }

    private static string SafeDestination(string root, string relativePath)
    {
        string rootPath = Path.GetFullPath(root).TrimEnd(Path.DirectorySeparatorChar) + Path.DirectorySeparatorChar;
        string destination = Path.GetFullPath(Path.Combine(rootPath, relativePath));
        if (!destination.StartsWith(rootPath, StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidDataException("O pacote contém um caminho inválido.");
        }

        return destination;
    }

    private static string DisableConflictingViralityPlugins(string pluginsDirectory)
    {
        string disabledDirectory = Path.Combine(
            Path.GetDirectoryName(pluginsDirectory)!,
            "disabled-plugins",
            DateTime.Now.ToString("yyyyMMdd-HHmmss"));
        var moved = new List<string>();

        foreach (string file in Directory.EnumerateFiles(pluginsDirectory, "*.dll", SearchOption.AllDirectories))
        {
            if (!Path.GetFileName(file).Contains("Virality", StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            Directory.CreateDirectory(disabledDirectory);
            string destination = UniquePath(Path.Combine(disabledDirectory, Path.GetFileName(file)));
            File.Move(file, destination);
            moved.Add(Path.GetFileName(file));
        }

        return moved.Count == 0
            ? string.Empty
            : "Plugins antigos foram desativados e guardados em BepInEx\\disabled-plugins: " +
              string.Join(", ", moved);
    }

    private static string UniquePath(string path)
    {
        if (!File.Exists(path))
        {
            return path;
        }

        string directory = Path.GetDirectoryName(path)!;
        string name = Path.GetFileNameWithoutExtension(path);
        string extension = Path.GetExtension(path);
        for (int number = 2; ; number++)
        {
            string candidate = Path.Combine(directory, $"{name}-{number}{extension}");
            if (!File.Exists(candidate))
            {
                return candidate;
            }
        }
    }

    private static void BackupExistingPlugin(string gameDirectory, string pluginPath)
    {
        if (!File.Exists(pluginPath))
        {
            return;
        }

        string backupDirectory = Path.Combine(gameDirectory, "BepInEx", "HostOnlyLobby-backups");
        Directory.CreateDirectory(backupDirectory);
        string backup = Path.Combine(
            backupDirectory,
            $"ContentWarningHostOnlyLobby-{DateTime.Now:yyyyMMdd-HHmmss}.dll");
        File.Copy(pluginPath, backup, overwrite: false);
    }

    private static void WriteEmbeddedPlugin(string destination)
    {
        using Stream source = Assembly.GetExecutingAssembly().GetManifestResourceStream(PayloadResource)
            ?? throw new InvalidOperationException("A DLL do HostOnlyLobby não está incluída no instalador.");
        using FileStream target = File.Create(destination);
        source.CopyTo(target);
    }

    private static void ConfigureBepInExConsole(string gameDirectory)
    {
        string configPath = Path.Combine(gameDirectory, "BepInEx", "config", "BepInEx.cfg");
        if (!File.Exists(configPath))
        {
            return;
        }

        string[] lines = File.ReadAllLines(configPath);
        bool insideConsoleSection = false;
        for (int i = 0; i < lines.Length; i++)
        {
            string trimmed = lines[i].Trim();
            if (trimmed.StartsWith('[') && trimmed.EndsWith(']'))
            {
                insideConsoleSection = trimmed.Equals("[Logging.Console]", StringComparison.OrdinalIgnoreCase);
            }
            else if (insideConsoleSection && trimmed.StartsWith("Enabled", StringComparison.OrdinalIgnoreCase))
            {
                lines[i] = "Enabled = false";
                break;
            }
        }

        File.WriteAllLines(configPath, lines, new UTF8Encoding(encoderShouldEmitUTF8Identifier: false));
    }

    private static void WriteLobbyConfiguration(string gameDirectory, int maxPlayers)
    {
        string configDirectory = Path.Combine(gameDirectory, "BepInEx", "config");
        Directory.CreateDirectory(configDirectory);
        string configPath = Path.Combine(configDirectory, "local.contentwarning.hostonlylobby.cfg");
        string contents =
            "## Settings file for HostOnlyLobby v1.0.1\r\n" +
            "## Only the host needs this plugin.\r\n\r\n" +
            "[Lobby]\r\n\r\n" +
            "## Maximum lobby size. Valid range: 5 to 16.\r\n" +
            "# Setting type: Int32\r\n" +
            "# Default value: 8\r\n" +
            $"MaxPlayers = {maxPlayers}\r\n";
        File.WriteAllText(configPath, contents, new UTF8Encoding(encoderShouldEmitUTF8Identifier: false));
    }
}
