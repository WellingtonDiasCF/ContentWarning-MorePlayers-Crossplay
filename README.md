# Content Warning — Mais jogadores com crossplay

**Português** · [English](README.en.md) · [Español](README.es.md)

[![Versão 1.2.0](https://img.shields.io/badge/versão-1.2.0-6a6df0)](https://github.com/WellingtonDiasCF/ContentWarning-MorePlayers-Crossplay/releases/latest)
[![Instalação](https://img.shields.io/badge/instalação-host%20%2B%20PC%20extra-2f9e73)](https://github.com/WellingtonDiasCF/ContentWarning-MorePlayers-Crossplay/releases/latest)
[![PC + Xbox](https://img.shields.io/badge/crossplay-PC%20%2B%20Xbox-107c10?logo=xbox)](https://github.com/WellingtonDiasCF/ContentWarning-MorePlayers-Crossplay/releases/latest)

Crie salas de 5 a 16 jogadores no Content Warning e convide pessoas de PC ou Xbox pelo código normal da sala.

O lobby é controlado pelo host. Para corrigir o spawn de uma quinta pessoa, qualquer PC que ocupar a quinta posição ou uma posição posterior também deve instalar o mesmo pacote. Nada é instalado no Xbox.

## Download

[**Baixar o instalador para Windows**](https://github.com/WellingtonDiasCF/ContentWarning-MorePlayers-Crossplay/releases/latest/download/HostOnlyLobby-Setup.exe)

- Arquivo: `HostOnlyLobby-Setup.exe`
- Versão atual: 1.2.0
- SHA-256: `7E7EC9AB2793F53CB7CB288E985BEDB700FBD9DCE7CBB8A7691290462F1EFB19`

## Instalação rápida

1. O host deve fechar o Content Warning.
2. Baixe e abra `HostOnlyLobby-Setup.exe`.
3. Confira a pasta do jogo detectada pela Steam.
4. Escolha o limite da sala, entre 5 e 16 jogadores.
5. Clique em **Instalar / Atualizar**.
6. Abra o jogo pela Steam e crie a sala normalmente.

Para quatro pessoas, somente o host precisa instalar. Para o grupo de dois PCs e três Xbox, execute o mesmo instalador nos dois PCs.

## Grupo com 2 PCs e 3 Xbox

Use esta ordem de entrada:

1. O PC host cria a sala.
2. Os três jogadores de Xbox entram pelo código.
3. O segundo PC, já com o mod instalado, entra por último.
4. Todos confirmam que estão dentro da casa antes de abrir a porta.

O jogo calcula o spawn localmente em cada aparelho e só possui quatro índices originais. A versão 1.2.0 faz o segundo PC reutilizar um ponto válido. A correção também vale ao voltar para a superfície dentro do sino e ao aparecer no hospital. No subterrâneo, o jogo já utiliza um ponto de spawn compartilhado.

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

As camas e o tamanho visual do quarto não são aumentados: objetos de cenário criados apenas no host não apareceriam corretamente nos Xbox.

## O que o instalador faz

- Instala BepInEx 5.4.23.5.
- Instala CrossPatcher 1.0.0.
- Instala HostOnlyLobby 1.2.0.
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

**O quinto jogador aparece voando fora da casa**

Esse aparelho ocupou um índice de spawn extra sem a correção local. Em um grupo com dois PCs e três Xbox, instale o mod no segundo PC e faça os três Xbox entrarem antes dele. Depois crie uma sala nova.

**A voz não funciona entre PC e Xbox**

O áudio entre plataformas depende do próprio jogo. Use Discord ou uma party do Xbox caso a voz falhe.

**O jogo atualizou e o mod parou**

Consulte a [página de versões](https://github.com/WellingtonDiasCF/ContentWarning-MorePlayers-Crossplay/releases). Atualizações do jogo podem exigir uma nova compilação.

## O que já foi validado

- 19/19 testes simulados de lobby, Photon, Steam, camas, sono e remapeamento de spawn.
- Instalação e remoção em pasta isolada.
- Inicialização real pela Steam com 8/8 patches registrados.

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
