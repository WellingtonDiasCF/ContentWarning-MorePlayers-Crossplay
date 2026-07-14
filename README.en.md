# Content Warning — More players with crossplay

[Português](README.md) · **English** · [Español](README.es.md)

[![Version 1.0.1](https://img.shields.io/badge/version-1.0.1-6a6df0)](https://github.com/WellingtonDiasCF/ContentWarning-MorePlayers-Crossplay/releases/latest)
[![Host only](https://img.shields.io/badge/install-host%20only-2f9e73)](https://github.com/WellingtonDiasCF/ContentWarning-MorePlayers-Crossplay/releases/latest)
[![PC + Xbox](https://img.shields.io/badge/crossplay-PC%20%2B%20Xbox-107c10?logo=xbox)](https://github.com/WellingtonDiasCF/ContentWarning-MorePlayers-Crossplay/releases/latest)

Create Content Warning lobbies for 5 to 16 players and invite PC or Xbox guests with the normal room code.

**Only the PC host installs the mod.** Guests do not need to download anything.

## Download

[**Download the Windows installer**](https://github.com/WellingtonDiasCF/ContentWarning-MorePlayers-Crossplay/releases/latest/download/HostOnlyLobby-Setup.exe)

- File: `HostOnlyLobby-Setup.exe`
- Current version: 1.0.1
- SHA-256: `3CA71C7A1A4CA875EC98DE63A2C69474A7194D91E44164CBE33B1775C9DD1295`

## Quick installation

1. The host closes Content Warning.
2. Download and open `HostOnlyLobby-Setup.exe`.
3. Confirm the Steam game folder detected by the installer.
4. Select a lobby limit from 5 to 16 players.
5. Click **Instalar / Atualizar**.
6. Launch the game through Steam and create a room normally.

Do not install the mod on guest PCs. No installation is possible or required on Xbox.

## Joining and starting a game

1. The host creates a room and shares its code.
2. Everyone joins while the group is still inside the house.
3. Confirm that all five or more players are connected.
4. Only then should the host open the house door.

Joining after the door has been opened is not supported in this version.

## Beds and day transitions

The game has four beds. To reduce synchronization errors, the mod only assigns those four beds. In a group of five or more, four ready players can advance the day. If fewer than four players are alive, every survivor must be ready.

## What the installer does

- Installs BepInEx 5.4.23.5.
- Installs CrossPatcher 1.0.0.
- Installs HostOnlyLobby 1.0.1.
- Verifies dependency downloads with SHA-256.
- Disables conflicting Virality DLLs and stores them in `BepInEx\disabled-plugins`.
- Updates or removes HostOnlyLobby using the same executable.

The installer does not use Virality or `ViralityDeprecatedSoIFixedIt`.

## Uninstallation

Open the installer again, confirm the game folder, and click **Desinstalar**. HostOnlyLobby and its configuration will be removed. BepInEx and CrossPatcher remain because other mods may use them.

## Troubleshooting

**Xbox cannot find the room**

Confirm that CrossPatcher was installed, the host launched the game through Steam, and everyone is using the current Content Warning version.

**The fifth player cannot join**

Run the installer again and select at least 5 players. Create a new room after installation.

**Voice chat does not work between PC and Xbox**

Cross-platform voice depends on the game. Use Discord or an Xbox party if it fails.

**The game updated and the mod stopped working**

Check the [Releases page](https://github.com/WellingtonDiasCF/ContentWarning-MorePlayers-Crossplay/releases). Game updates may require a new mod build.

## Validation

- 13/13 simulated tests for lobby, Photon, Steam, beds, and sleep logic.
- Installation and removal tested in an isolated folder.
- A real Steam launch confirmed all 7/7 patches.

Communication between multiple PCs and Xbox consoles, including voice and scene transitions, still depends on the game's infrastructure and must be confirmed in a real session.

<details>
<summary>Developer information</summary>

Build the installer with the .NET 8 SDK:

```powershell
dotnet publish src/Installer/HostOnlyLobby.Installer.csproj -c Release -r win-x64
```

To build the plugin, set `CONTENT_WARNING_DIR` to the game folder and run:

```powershell
dotnet build src/HostOnlyLobby/ContentWarningHostOnlyLobby.csproj -c Release
```

</details>

## Credits and license

CrossPatcher is by gingerphoenix10. BepInEx is maintained by the BepInEx team. HostOnlyLobby uses techniques from the LGPL-3.0-licensed Virality family.

See [THIRD-PARTY-NOTICES.md](THIRD-PARTY-NOTICES.md) and [LICENSE](LICENSE).
