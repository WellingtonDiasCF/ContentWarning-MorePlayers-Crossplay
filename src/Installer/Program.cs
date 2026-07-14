namespace HostOnlyLobbyInstaller;

internal static class Program
{
    [STAThread]
    private static async Task<int> Main(string[] args)
    {
        if (args.Length >= 2 && args[0].Equals("--install", StringComparison.OrdinalIgnoreCase))
        {
            int maxPlayers = ReadMaximum(args);
            await InstallerEngine.InstallAsync(args[1], maxPlayers, null);
            return 0;
        }

        if (args.Length >= 2 && args[0].Equals("--uninstall", StringComparison.OrdinalIgnoreCase))
        {
            InstallerEngine.Uninstall(args[1]);
            return 0;
        }

        ApplicationConfiguration.Initialize();
        Application.Run(new InstallerForm());
        return 0;
    }

    private static int ReadMaximum(string[] args)
    {
        for (int i = 2; i < args.Length - 1; i++)
        {
            if (args[i].Equals("--max-players", StringComparison.OrdinalIgnoreCase) &&
                int.TryParse(args[i + 1], out int maximum))
            {
                return Math.Clamp(maximum, 5, 16);
            }
        }

        return 8;
    }
}
