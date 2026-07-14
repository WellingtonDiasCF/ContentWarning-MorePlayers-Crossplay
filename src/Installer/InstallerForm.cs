using System.Diagnostics;
using System.Drawing;

namespace HostOnlyLobbyInstaller;

internal sealed class InstallerForm : Form
{
    private readonly TextBox _gameDirectory = new();
    private readonly NumericUpDown _maxPlayers = new();
    private readonly Button _installButton = new();
    private readonly Button _uninstallButton = new();
    private readonly Button _browseButton = new();
    private readonly TextBox _status = new();
    private readonly ProgressBar _progress = new();

    internal InstallerForm()
    {
        Text = "HostOnlyLobby — Instalador";
        ClientSize = new Size(700, 520);
        MinimumSize = new Size(716, 559);
        StartPosition = FormStartPosition.CenterScreen;
        BackColor = Color.FromArgb(247, 248, 250);
        Font = new Font("Segoe UI", 10F);

        var header = new Panel
        {
            Dock = DockStyle.Top,
            Height = 104,
            BackColor = Color.FromArgb(27, 31, 42)
        };
        var title = new Label
        {
            AutoSize = true,
            Location = new Point(28, 20),
            Font = new Font("Segoe UI Semibold", 21F),
            ForeColor = Color.White,
            Text = "HostOnlyLobby"
        };
        var subtitle = new Label
        {
            AutoSize = true,
            Location = new Point(31, 63),
            ForeColor = Color.FromArgb(205, 210, 220),
            Text = "Lobby maior no Content Warning — somente o host instala"
        };
        header.Controls.Add(title);
        header.Controls.Add(subtitle);

        var pathLabel = new Label
        {
            AutoSize = true,
            Location = new Point(30, 129),
            Text = "Pasta do Content Warning"
        };
        _gameDirectory.SetBounds(30, 153, 548, 29);
        _gameDirectory.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

        _browseButton.SetBounds(590, 151, 80, 32);
        _browseButton.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        _browseButton.Text = "Procurar";
        _browseButton.Click += BrowseClicked;

        var maximumLabel = new Label
        {
            AutoSize = true,
            Location = new Point(30, 204),
            Text = "Máximo de jogadores"
        };
        _maxPlayers.SetBounds(190, 200, 72, 29);
        _maxPlayers.Minimum = 5;
        _maxPlayers.Maximum = 16;
        _maxPlayers.Value = 8;

        var tip = new Label
        {
            AutoSize = true,
            Location = new Point(30, 240),
            ForeColor = Color.FromArgb(85, 89, 98),
            Text = "Os convidados entram pelo código normal. PC e Xbox não precisam instalar nada."
        };

        _installButton.SetBounds(30, 280, 178, 42);
        _installButton.Text = "Instalar / Atualizar";
        _installButton.BackColor = Color.FromArgb(61, 105, 225);
        _installButton.ForeColor = Color.White;
        _installButton.FlatStyle = FlatStyle.Flat;
        _installButton.FlatAppearance.BorderSize = 0;
        _installButton.Click += InstallClicked;

        _uninstallButton.SetBounds(220, 280, 126, 42);
        _uninstallButton.Text = "Desinstalar";
        _uninstallButton.Click += UninstallClicked;

        var playButton = new Button
        {
            Text = "Abrir o jogo",
            Location = new Point(358, 280),
            Size = new Size(126, 42)
        };
        playButton.Click += (_, _) => OpenUrl("steam://run/2881650");

        var projectLink = new LinkLabel
        {
            AutoSize = true,
            Location = new Point(502, 294),
            Text = "Página do projeto",
            LinkColor = Color.FromArgb(52, 84, 170)
        };
        projectLink.LinkClicked += (_, _) => OpenUrl(InstallerEngine.RepositoryUrl);

        _progress.SetBounds(30, 344, 640, 8);
        _progress.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        _progress.Style = ProgressBarStyle.Blocks;

        _status.SetBounds(30, 369, 640, 116);
        _status.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        _status.Multiline = true;
        _status.ReadOnly = true;
        _status.ScrollBars = ScrollBars.Vertical;
        _status.BackColor = Color.White;
        _status.Text = "Pronto para instalar.";

        Controls.AddRange(new Control[]
        {
            header,
            pathLabel,
            _gameDirectory,
            _browseButton,
            maximumLabel,
            _maxPlayers,
            tip,
            _installButton,
            _uninstallButton,
            playButton,
            projectLink,
            _progress,
            _status
        });

        string? detectedDirectory = InstallerEngine.FindGameDirectory();
        if (detectedDirectory != null)
        {
            _gameDirectory.Text = detectedDirectory;
            AppendStatus("Jogo encontrado automaticamente.");
        }
        else
        {
            AppendStatus("Use Procurar para selecionar a pasta onde está Content Warning.exe.");
        }
    }

    private void BrowseClicked(object? sender, EventArgs e)
    {
        using var dialog = new FolderBrowserDialog
        {
            Description = "Selecione a pasta onde está Content Warning.exe",
            UseDescriptionForTitle = true,
            ShowNewFolderButton = false
        };

        if (Directory.Exists(_gameDirectory.Text))
        {
            dialog.InitialDirectory = _gameDirectory.Text;
        }

        if (dialog.ShowDialog(this) == DialogResult.OK)
        {
            _gameDirectory.Text = dialog.SelectedPath;
        }
    }

    private async void InstallClicked(object? sender, EventArgs e)
    {
        SetBusy(true);
        _status.Clear();
        var progress = new Progress<string>(AppendStatus);

        try
        {
            string result = await InstallerEngine.InstallAsync(
                _gameDirectory.Text,
                decimal.ToInt32(_maxPlayers.Value),
                progress);
            _progress.Style = ProgressBarStyle.Blocks;
            _progress.Value = 100;
            MessageBox.Show(
                this,
                result + "\n\nAbra o jogo pela Steam e crie a sala normalmente. Convide todos antes de abrir a porta da casa.",
                "Instalação concluída",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }
        catch (UnauthorizedAccessException)
        {
            ShowError("O Windows bloqueou a gravação na pasta do jogo. Feche o instalador e abra-o como administrador.");
        }
        catch (Exception exception)
        {
            ShowError(exception.Message);
        }
        finally
        {
            SetBusy(false);
        }
    }

    private void UninstallClicked(object? sender, EventArgs e)
    {
        if (MessageBox.Show(
                this,
                "Remover o HostOnlyLobby? BepInEx e CrossPatcher serão mantidos porque podem ser usados por outros mods.",
                "Confirmar desinstalação",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question) != DialogResult.Yes)
        {
            return;
        }

        try
        {
            InstallerEngine.Uninstall(_gameDirectory.Text);
            AppendStatus("HostOnlyLobby removido. BepInEx e CrossPatcher foram mantidos.");
            MessageBox.Show(this, "HostOnlyLobby removido.", "Concluído", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        catch (Exception exception)
        {
            ShowError(exception.Message);
        }
    }

    private void SetBusy(bool busy)
    {
        _installButton.Enabled = !busy;
        _uninstallButton.Enabled = !busy;
        _browseButton.Enabled = !busy;
        _maxPlayers.Enabled = !busy;
        _progress.Style = busy ? ProgressBarStyle.Marquee : ProgressBarStyle.Blocks;
        if (!busy && _progress.Value != 100)
        {
            _progress.Value = 0;
        }
    }

    private void AppendStatus(string message)
    {
        if (_status.TextLength > 0)
        {
            _status.AppendText(Environment.NewLine);
        }

        _status.AppendText(message);
    }

    private void ShowError(string message)
    {
        _progress.Style = ProgressBarStyle.Blocks;
        _progress.Value = 0;
        AppendStatus("Erro: " + message);
        MessageBox.Show(this, message, "Não foi possível concluir", MessageBoxButtons.OK, MessageBoxIcon.Error);
    }

    private static void OpenUrl(string url)
    {
        Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
    }
}
