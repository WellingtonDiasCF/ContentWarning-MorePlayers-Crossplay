# More Players Crossplay

Expands Content Warning rooms to 5-16 players and keeps the room visible to PC and Xbox players through the normal room code.

## Important compatibility note

The expanded lobby is controlled by the PC host. Content Warning calculates spawn indices locally on every device and the vanilla surface scenes only contain four indices.

- With four players or fewer, only the host needs the mod.
- Any PC occupying slot 5 or later must also install this package for safe spawning.
- Nothing can be installed on Xbox, so Xbox players must occupy the first three guest slots.

For a group with two PCs and three Xbox consoles, use this order:

1. The PC host creates the room.
2. The three Xbox players join using the room code.
3. The second PC, with this package installed, joins last.
4. Everyone confirms they are inside the house before the host opens the door.

## Features

- Configurable room size from 5 to 16 players.
- PC and Xbox room discovery through CrossPatcher.
- Safe spawn reuse for the house, surface dive-bell return, and hospital.
- Four-bed assignment without out-of-range errors.
- Only four sleeping players are required to advance the day.
- On-screen host status indicator.
- No Virality or `ViralityDeprecatedSoIFixedIt` dependency.

The underground scenes already use a shared spawn point and do not require remapping. The physical bedroom remains unchanged because host-only scene objects would not be synchronized correctly with Xbox players.

## Configuration

After the first launch, edit:

`BepInEx/config/local.contentwarning.hostonlylobby.cfg`

Available settings:

- `MaxPlayers`: room limit from 5 to 16.
- `ShowStatusIndicator`: enables the host status card.
- `IndicatorSeconds`: controls how long the card remains visible.

Everyone must join before the house door is opened. Late joining is intentionally disabled.

## Português

O mod aumenta a sala para 5-16 jogadores e permite que pessoas de PC e Xbox entrem pelo código normal.

Para um grupo com dois PCs e três Xbox: o host cria a sala, os três Xbox entram primeiro e o segundo PC com o mod instalado entra por último. Nada é instalado nos consoles. O jogo continua com quatro camas físicas, e quatro pessoas dormindo são suficientes para avançar o dia.

Código-fonte, instalador manual e instruções completas: [GitHub](https://github.com/WellingtonDiasCF/ContentWarning-MorePlayers-Crossplay).

## Support

Report reproducible problems on the [GitHub issue tracker](https://github.com/WellingtonDiasCF/ContentWarning-MorePlayers-Crossplay/issues). Include the game version, number of PC/Xbox players, join order, and `BepInEx/LogOutput.log` when possible.
