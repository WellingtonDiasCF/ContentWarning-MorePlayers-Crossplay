# Content Warning — More Players + Crossplay

Expande o lobby do Content Warning para mais de quatro jogadores mantendo a entrada por código para PC e Xbox.

Somente quem cria a sala instala o mod. Os convidados entram normalmente e não precisam baixar nada.

## Download

[**Baixar o instalador para Windows**](https://github.com/WellingtonDiasCF/ContentWarning-MorePlayers-Crossplay/releases/latest/download/HostOnlyLobby-Setup.exe)

O instalador encontra o jogo pela Steam, deixa escolher entre 5 e 16 jogadores e configura tudo automaticamente. A opção padrão é 8.

## Como instalar

1. Baixe `HostOnlyLobby-Setup.exe` pelo link acima.
2. Feche o Content Warning.
3. Abra o instalador e confira a pasta detectada.
4. Escolha o número máximo de jogadores e clique em **Instalar / Atualizar**.
5. Abra o jogo pela Steam e crie a sala normalmente.
6. Envie o código para os outros jogadores.

Se o Windows mostrar o aviso do SmartScreen, clique em **Mais informações** e depois em **Executar assim mesmo**. O arquivo não possui assinatura digital comercial; o código e o checksum de cada versão ficam publicados neste repositório.

## Antes de começar a partida

Espere todos entrarem enquanto o grupo ainda está dentro da casa. Abra a porta somente depois que os cinco jogadores estiverem conectados.

O jogo possui quatro camas. O mod mantém quatro atribuições e permite avançar quando quatro jogadores estiverem prontos. Se houver menos sobreviventes, todos os sobreviventes precisam estar prontos.

## O que é instalado

- BepInEx 5.4.23.5;
- CrossPatcher 1.0.0;
- HostOnlyLobby 1.0.1.

Os pacotes do BepInEx e CrossPatcher são baixados das páginas originais do Thunderstore e conferidos por SHA-256 antes da instalação. O instalador não usa Virality nem `ViralityDeprecatedSoIFixedIt`. Caso encontre uma DLL Virality antiga, ele a desativa e guarda uma cópia em `BepInEx\disabled-plugins`.

## Desinstalação

Abra o mesmo instalador e clique em **Desinstalar**. Ele remove o HostOnlyLobby e mantém BepInEx/CrossPatcher, pois outros mods podem depender deles.

## Limitações conhecidas

- O mod foi validado localmente com as assemblies da versão 1.24.x do jogo.
- Voz e sincronização entre PC e Xbox ainda dependem dos serviços do próprio jogo.
- Atualizações do Content Warning podem exigir uma nova versão do mod.
- Se o áudio do jogo falhar entre plataformas, use Discord ou uma party do Xbox.

## Verificação realizada

O teste automatizado cobre 13 cenários de lobby, Photon, Steam, camas e transição de sono. A inicialização real pela Steam confirmou os sete patches do mod. O teste definitivo de rede ainda é uma partida com máquinas e consoles separados.

## Compilar

O instalador requer o SDK do .NET 8:

```powershell
dotnet publish src/Installer/HostOnlyLobby.Installer.csproj -c Release -r win-x64
```

Para compilar a DLL do mod, defina `CONTENT_WARNING_DIR` com a pasta de uma instalação legítima do jogo e execute:

```powershell
dotnet build src/HostOnlyLobby/ContentWarningHostOnlyLobby.csproj -c Release
```

## Créditos e licença

CrossPatcher é de gingerphoenix10. BepInEx é mantido pela equipe BepInEx. O HostOnlyLobby usa técnicas da família Virality, licenciada sob LGPL-3.0.

Consulte [THIRD-PARTY-NOTICES.md](THIRD-PARTY-NOTICES.md) e [LICENSE](LICENSE).
