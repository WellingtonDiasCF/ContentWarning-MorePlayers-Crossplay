# Content Warning — More players with crossplay

[Português](README.md) · **English** · [Español](README.es.md)

[![Version 1.2.0](https://img.shields.io/badge/version-1.2.0-6a6df0)](https://github.com/WellingtonDiasCF/ContentWarning-MorePlayers-Crossplay/releases/latest)
[![Installation](https://img.shields.io/badge/install-host%20%2B%20extra%20PC-2f9e73)](https://github.com/WellingtonDiasCF/ContentWarning-MorePlayers-Crossplay/releases/latest)
[![PC + Xbox](https://img.shields.io/badge/crossplay-PC%20%2B%20Xbox-107c10?logo=xbox)](https://github.com/WellingtonDiasCF/ContentWarning-MorePlayers-Crossplay/releases/latest)

Create Content Warning lobbies for 5 to 16 players and invite PC or Xbox guests with the normal room code.

The host controls the expanded lobby. To fix a fifth player's spawn, any PC occupying slot five or later must also install the same package. Nothing is installed on Xbox.

## Download

[**Download the Windows installer**](https://github.com/WellingtonDiasCF/ContentWarning-MorePlayers-Crossplay/releases/latest/download/HostOnlyLobby-Setup.exe)

- File: `HostOnlyLobby-Setup.exe`
- Current version: 1.2.0
- SHA-256: `7E7EC9AB2793F53CB7CB288E985BEDB700FBD9DCE7CBB8A7691290462F1EFB19`

## Quick installation

1. The host closes Content Warning.
2. Download and open `HostOnlyLobby-Setup.exe`.
3. Confirm the Steam game folder detected by the installer.
4. Select a lobby limit from 5 to 16 players.
5. Click **Instalar / Atualizar**.
6. Launch the game through Steam and create a room normally.

For up to four people, only the host needs it. For a group with two PCs and three Xbox consoles, run the same installer on both PCs.

## Group with 2 PCs and 3 Xbox consoles

Use this join order:

1. The PC host creates the room.
2. The three Xbox players join with the room code.
3. The second PC, with the mod installed, joins last.
4. Everyone confirms they are inside the house before opening the door.

Each device calculates its own spawn, and the game only provides four original indices. Version 1.2.0 makes the second PC reuse a valid point. The fix also covers returning to the surface inside the dive bell and spawning at the hospital. Underground scenes already use a shared spawn point.

## On-screen indicator

When entering a room and whenever a new world loads, the host sees this card in the upper-right corner for 12 seconds:

```text
HOSTONLYLOBBY ATIVO
Sala do host • até 8 jogadores
```

It confirms that the plugin is running and displays the configured lobby limit. Only the host sees it. The card can be disabled or its duration changed in `BepInEx\config\local.contentwarning.hostonlylobby.cfg`.

## Joining and starting a game

1. The host creates a room and shares its code.
2. Everyone joins while the group is still inside the house.
3. Confirm that all five or more players are connected.
4. Only then should the host open the house door.

Joining after the door has been opened is not supported in this version.

## Beds and day transitions

The game has four beds. To reduce synchronization errors, the mod only assigns those four beds. In a group of five or more, four ready players can advance the day. If fewer than four players are alive, every survivor must be ready.

The beds and the room's visual size are not expanded: scene objects created only on the host would not appear correctly on Xbox.

## What the installer does

- Installs BepInEx 5.4.23.5.
- Installs CrossPatcher 1.0.0.
- Installs HostOnlyLobby 1.2.0.
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

**The fifth player floats outside the house**

That device occupied an extra local spawn index without the compatibility fix. With two PCs and three Xbox consoles, install the mod on the second PC and have all three Xbox players join before it. Then create a new room.

**Voice chat does not work between PC and Xbox**

Cross-platform voice depends on the game. Use Discord or an Xbox party if it fails.

**The game updated and the mod stopped working**

Check the [Releases page](https://github.com/WellingtonDiasCF/ContentWarning-MorePlayers-Crossplay/releases). Game updates may require a new mod build.

## Validation

- 19/19 simulated tests for lobby, Photon, Steam, beds, sleep, and spawn remapping.
- Installation and removal tested in an isolated folder.
- A real Steam launch confirmed all 8/8 patches.

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
