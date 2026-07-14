# Content Warning — Más jugadores con crossplay

[Português](README.md) · [English](README.en.md) · **Español**

[![Versión 1.0.1](https://img.shields.io/badge/versión-1.0.1-6a6df0)](https://github.com/WellingtonDiasCF/ContentWarning-MorePlayers-Crossplay/releases/latest)
[![Solo host](https://img.shields.io/badge/instalación-solo%20el%20host-2f9e73)](https://github.com/WellingtonDiasCF/ContentWarning-MorePlayers-Crossplay/releases/latest)
[![PC + Xbox](https://img.shields.io/badge/crossplay-PC%20%2B%20Xbox-107c10?logo=xbox)](https://github.com/WellingtonDiasCF/ContentWarning-MorePlayers-Crossplay/releases/latest)

Crea salas de Content Warning para 5 a 16 jugadores e invita a usuarios de PC o Xbox con el código normal de la sala.

**Solo el host de PC instala el mod.** Los invitados no necesitan descargar nada.

## Descargar

[**Descargar el instalador para Windows**](https://github.com/WellingtonDiasCF/ContentWarning-MorePlayers-Crossplay/releases/latest/download/HostOnlyLobby-Setup.exe)

- Archivo: `HostOnlyLobby-Setup.exe`
- Versión actual: 1.0.1
- SHA-256: `3CA71C7A1A4CA875EC98DE63A2C69474A7194D91E44164CBE33B1775C9DD1295`

## Instalación rápida

1. El host cierra Content Warning.
2. Descarga y abre `HostOnlyLobby-Setup.exe`.
3. Confirma la carpeta del juego detectada mediante Steam.
4. Elige un límite de 5 a 16 jugadores.
5. Haz clic en **Instalar / Atualizar**.
6. Abre el juego desde Steam y crea una sala normalmente.

No instales el mod en los PC invitados. En Xbox no es posible ni necesario instalarlo.

## Entrar y comenzar la partida

1. El host crea la sala y comparte el código.
2. Todos entran mientras el grupo todavía está dentro de la casa.
3. Confirma que los cinco o más jugadores están conectados.
4. Solo entonces el host abre la puerta de la casa.

Entrar después de abrir la puerta no es compatible con esta versión.

## Camas y cambio de día

El juego tiene cuatro camas. Para reducir errores de sincronización, el mod solo asigna esas cuatro camas. En un grupo de cinco o más, cuatro jugadores preparados permiten avanzar. Si quedan menos de cuatro supervivientes, todos deben estar preparados.

## Qué hace el instalador

- Instala BepInEx 5.4.23.5.
- Instala CrossPatcher 1.0.0.
- Instala HostOnlyLobby 1.0.1.
- Verifica las dependencias descargadas mediante SHA-256.
- Desactiva DLL Virality conflictivas y las guarda en `BepInEx\disabled-plugins`.
- Permite actualizar o quitar HostOnlyLobby con el mismo ejecutable.

El instalador no utiliza Virality ni `ViralityDeprecatedSoIFixedIt`.

## Desinstalación

Abre de nuevo el instalador, confirma la carpeta del juego y pulsa **Desinstalar**. Se eliminarán HostOnlyLobby y su configuración. BepInEx y CrossPatcher se conservan porque otros mods pueden utilizarlos.

## Solución de problemas

**Xbox no encuentra la sala**

Confirma que CrossPatcher está instalado, que el host abrió el juego desde Steam y que todos utilizan la versión actual de Content Warning.

**El quinto jugador no puede entrar**

Ejecuta otra vez el instalador y selecciona al menos 5 jugadores. Crea una sala nueva después de instalar.

**La voz no funciona entre PC y Xbox**

El audio entre plataformas depende del juego. Usa Discord o un grupo de Xbox si falla.

**El juego se actualizó y el mod dejó de funcionar**

Consulta la [página de versiones](https://github.com/WellingtonDiasCF/ContentWarning-MorePlayers-Crossplay/releases). Las actualizaciones del juego pueden requerir una nueva compilación.

## Validación

- 13/13 pruebas simuladas de lobby, Photon, Steam, camas y sueño.
- Instalación y eliminación probadas en una carpeta aislada.
- Un inicio real desde Steam confirmó los 7/7 parches.

La comunicación entre varios PC y consolas Xbox, incluida la voz y los cambios de escena, sigue dependiendo de la infraestructura del juego y debe confirmarse en una sesión real.

<details>
<summary>Información para desarrolladores</summary>

Compila el instalador con el SDK de .NET 8:

```powershell
dotnet publish src/Installer/HostOnlyLobby.Installer.csproj -c Release -r win-x64
```

Para compilar el plugin, define `CONTENT_WARNING_DIR` con la carpeta del juego y ejecuta:

```powershell
dotnet build src/HostOnlyLobby/ContentWarningHostOnlyLobby.csproj -c Release
```

</details>

## Créditos y licencia

CrossPatcher es de gingerphoenix10. BepInEx es mantenido por el equipo de BepInEx. HostOnlyLobby utiliza técnicas de la familia Virality, bajo licencia LGPL-3.0.

Consulta [THIRD-PARTY-NOTICES.md](THIRD-PARTY-NOTICES.md) y [LICENSE](LICENSE).
