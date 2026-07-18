# Content Warning — Mais jogadores com crossplay

**Português** · [English](README.en.md) · [Español](README.es.md)

[![Versão 1.1.0](https://img.shields.io/badge/versão-1.1.0-6a6df0)](https://github.com/WellingtonDiasCF/ContentWarning-MorePlayers-Crossplay/releases/latest)
[![Host only](https://img.shields.io/badge/instalação-somente%20o%20host-2f9e73)](https://github.com/WellingtonDiasCF/ContentWarning-MorePlayers-Crossplay/releases/latest)
[![PC + Xbox](https://img.shields.io/badge/crossplay-PC%20%2B%20Xbox-107c10?logo=xbox)](https://github.com/WellingtonDiasCF/ContentWarning-MorePlayers-Crossplay/releases/latest)

Crie salas de 5 a 16 jogadores no Content Warning e convide pessoas de PC ou Xbox pelo código normal da sala.

**Somente o host do PC instala.** Os convidados não precisam baixar o mod.

## Download

[**Baixar o instalador para Windows**](https://github.com/WellingtonDiasCF/ContentWarning-MorePlayers-Crossplay/releases/latest/download/HostOnlyLobby-Setup.exe)

- Arquivo: `HostOnlyLobby-Setup.exe`
- Versão atual: 1.1.0
- SHA-256: `B8E0BB7D438BF11206419ED1EA243CAC6024BF0B83C1513A75B8DD8AB1ABE5DE`

## Instalação rápida

1. O host deve fechar o Content Warning.
2. Baixe e abra `HostOnlyLobby-Setup.exe`.
3. Confira a pasta do jogo detectada pela Steam.
4. Escolha o limite da sala, entre 5 e 16 jogadores.
5. Clique em **Instalar / Atualizar**.
6. Abra o jogo pela Steam e crie a sala normalmente.

Não instale o mod nos PCs convidados. Não há instalação possível ou necessária no Xbox.

## Indicador na tela

Ao entrar na sala e sempre que um novo mundo é carregado, o host vê por 12 segundos um cartão no canto superior direito:

```text
HOSTONLYLOBBY ATIVO
Sala do host • até 8 jogadores
```

O aviso confirma que o plugin está funcionando e mostra o limite configurado. Ele aparece somente para o host. É possível desativá-lo ou mudar sua duração em `BepInEx\config\local.contentwarning.hostonlylobby.cfg`.

## Como entrar e começar a partida

1. O host cria a sala e envia o código.
2. Todos entram enquanto o grupo ainda está dentro da casa.
3. Confirme que os cinco ou mais jogadores aparecem conectados.
4. Só então o host abre a porta da casa.

Entrar depois que a porta foi aberta não é suportado nesta versão.

## Camas e passagem do dia

O jogo possui quatro camas. Para evitar erros de sincronização, o mod atribui somente essas quatro camas. Em um grupo com cinco ou mais jogadores, quatro pessoas prontas permitem avançar. Se houver menos de quatro sobreviventes, todos os sobreviventes precisam estar prontos.

## O que o instalador faz

- Instala BepInEx 5.4.23.5.
- Instala CrossPatcher 1.0.0.
- Instala HostOnlyLobby 1.1.0.
- Confere os downloads por SHA-256.
- Desativa DLLs Virality conflitantes e as guarda em `BepInEx\disabled-plugins`.
- Permite atualizar ou remover o HostOnlyLobby pelo mesmo executável.

O instalador não usa Virality nem `ViralityDeprecatedSoIFixedIt`.

## Desinstalação

Abra o instalador novamente, confira a pasta do jogo e clique em **Desinstalar**. O HostOnlyLobby e sua configuração serão removidos. BepInEx e CrossPatcher permanecem porque outros mods podem utilizá-los.

## Solução de problemas

**O Xbox não encontra a sala**

Confirme que CrossPatcher foi instalado, que o host abriu o jogo pela Steam e que todos estão usando a versão atual do Content Warning.

**O quinto jogador não consegue entrar**

Execute o instalador novamente e escolha pelo menos 5 jogadores. Crie uma sala nova depois da instalação.

**A voz não funciona entre PC e Xbox**

O áudio entre plataformas depende do próprio jogo. Use Discord ou uma party do Xbox caso a voz falhe.

**O jogo atualizou e o mod parou**

Consulte a [página de versões](https://github.com/WellingtonDiasCF/ContentWarning-MorePlayers-Crossplay/releases). Atualizações do jogo podem exigir uma nova compilação.

## O que já foi validado

- 13/13 testes simulados de lobby, Photon, Steam, camas e sono.
- Instalação e remoção em pasta isolada.
- Inicialização real pela Steam com 7/7 patches registrados.

A comunicação real entre vários PCs e Xbox, incluindo voz e troca de cenas, ainda depende da infraestrutura do jogo e deve ser confirmada durante a partida.

<details>
<summary>Informações para desenvolvimento</summary>

Para compilar o instalador com o SDK .NET 8:

```powershell
dotnet publish src/Installer/HostOnlyLobby.Installer.csproj -c Release -r win-x64
```

Para compilar o plugin, defina `CONTENT_WARNING_DIR` com a pasta do jogo e execute:

```powershell
dotnet build src/HostOnlyLobby/ContentWarningHostOnlyLobby.csproj -c Release
```

</details>

## Créditos e licença

CrossPatcher é de gingerphoenix10. BepInEx é mantido pela equipe BepInEx. O HostOnlyLobby usa técnicas da família Virality, licenciada sob LGPL-3.0.

Consulte [THIRD-PARTY-NOTICES.md](THIRD-PARTY-NOTICES.md) e [LICENSE](LICENSE).
