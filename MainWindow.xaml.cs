using System.Diagnostics;
using System.Globalization;
using System.Net;
using CadastroClientes.Data;
using CadastroClientes.Dialogs;
using CadastroClientes.InternationalDocumentManager.CountryConfiguration;
using CadastroClientes.InternationalDocumentManager.DocumentMasks;
using CadastroClientes.InternationalDocumentManager.Internationalization;
using CadastroClientes.Models;
using CadastroClientes.Services;
using CadastroClientes.Theming;
using Microsoft.UI.Composition.SystemBackdrops;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Windows.Storage.Pickers;
using WinRT.Interop;

namespace CadastroClientes;

public sealed partial class MainWindow : Window
{
    private readonly DatabaseService _db = new();
    private readonly GoogleDriveBackupService _drive = new();
    private int _total;
    private bool _inicializando = true; // evita disparar evento de idioma no carregamento

    public MainWindow()
    {
        InitializeComponent();

        Title = "TKM Cadastro de Cliente";

        // ── Ícone da aplicação na barra de título e barra de tarefas ──
        try
        {
            var iconPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "app.ico");
            if (File.Exists(iconPath))
                AppWindow.SetIcon(iconPath);
        }
        catch { }

        // ── Maximizar na abertura em qualquer monitor ──
        MaximizarJanela();

        // ── Mica (Windows 11) ou fundo sólido ──
        if (MicaController.IsSupported())
            SystemBackdrop = new Microsoft.UI.Xaml.Media.MicaBackdrop();

        // ── Carregar preferências salvas ──
        ThemeService.CarregarPreferencia();
        LocalizationService.CarregarPreferencia();

        // ── Preencher combo de idiomas ──
        PreencherIdiomas();

        // ── Aplicar tema e textos ──
        AplicarTema();
        AtualizarTextos();
        Carregar();

        _inicializando = false;
        Closed += (_, _) => _drive.Dispose();
    }

    // ══════════════════════════════════════════════════════════════
    //  MAXIMIZAR
    // ══════════════════════════════════════════════════════════════

    private void MaximizarJanela()
    {
        try
        {
            if (AppWindow.Presenter is OverlappedPresenter presenter)
            {
                presenter.Maximize();
            }
            else
            {
                var p = OverlappedPresenter.Create();
                p.Maximize();
                AppWindow.SetPresenter(p);
            }
        }
        catch
        {
            try
            {
                AppWindow.Resize(new Windows.Graphics.SizeInt32(1280, 800));
            }
            catch { }
        }
    }

    // ══════════════════════════════════════════════════════════════
    //  TEMA
    // ══════════════════════════════════════════════════════════════

    private void AplicarTema()
    {
        bool escuro = ThemeService.Tipo == TemaTipo.Escuro;
        Raiz.RequestedTheme = escuro ? ElementTheme.Dark : ElementTheme.Light;
        string chave = escuro ? "BtnThemeLight" : "BtnThemeDark";
        BtnTema.Content = LocalizationService.Get(chave);
    }

    private void BtnTema_Click(object sender, RoutedEventArgs e)
    {
        ThemeService.Alternar();
        AplicarTema();
    }

    private void BtnKofi_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            Process.Start(new ProcessStartInfo("https://ko-fi.com/tikemem") { UseShellExecute = true });
        }
        catch { }
    }

    // ══════════════════════════════════════════════════════════════
    //  IDIOMA
    // ══════════════════════════════════════════════════════════════

    private void PreencherIdiomas()
    {
        var idiomaAtual = LocalizationService.IdiomaAtual;
        int idx = 0, i = 0;
        foreach (var (id, nome) in LocalizationService.IdiomasDisponiveis)
        {
            CmbIdioma.Items.Add(new ComboBoxItem { Content = nome, Tag = id });
            if (id == idiomaAtual) idx = i;
            i++;
        }
        CmbIdioma.SelectedIndex = idx;
    }

    private void CmbIdioma_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (_inicializando) return;
        if (CmbIdioma.SelectedItem is ComboBoxItem item && item.Tag is Idioma idioma)
        {
            LocalizationService.DefinirIdioma(idioma);
            AtualizarTextos();
        }
    }

    /// <summary>Atualiza todos os textos estáticos da janela com o idioma atual.</summary>
    private void AtualizarTextos()
    {
        var L = LocalizationService.Get;

        Title                 = L("AppTitle");
        TxtTitulo.Text        = L("AppTitle");
        TxtSubtitulo.Text     = L("AppSubtitle");
        TxtBusca.PlaceholderText = L("SearchPlaceholder");
        BtnNovo.Content       = L("BtnNew");
        BtnEditar.Content     = L("BtnEdit");
        BtnExcluir.Content    = L("BtnDelete");
        BtnImprimir.Content   = L("BtnPrint");
        BtnConfigDrive.Content = L("BtnConfigDrive");
        BtnBackup.Content     = L("BtnBackup");
        BtnRestaurar.Content  = L("BtnRestore");
        ColNome.Text       = L("ColName");
        ColDocumento.Text  = L("ColDocument");
        ColEmail.Text      = L("ColEmail");
        ColTelefone.Text   = L("ColPhone");
        ColCidade.Text     = L("ColCity");
        ColUf.Text         = L("ColState");
        ColCadastrado.Text = L("ColCreated");
        AplicarTema(); // Atualiza o botão de tema também
        AtualizarStatus();
    }

    // ══════════════════════════════════════════════════════════════
    //  LISTA E BUSCA
    // ══════════════════════════════════════════════════════════════

    private LinhaCliente? Selecionado => Lista.SelectedItem as LinhaCliente;

    private void Carregar()
    {
        var clientes = _db.Listar(TxtBusca.Text);
        _total = clientes.Count;

        Lista.ItemsSource = clientes
            .Select(c => {
                var pais = CountryRegistry.ObterPorCodigo(c.PaisResidencia);
                var docFormatado = Validador.FormatarDocumento(c.Documento, c.TipoDocumento);
                return new LinhaCliente(
                    c.Id,
                    $"{pais.FlagEmoji}  {c.Nome}",
                    docFormatado,
                    c.Email ?? "",
                    c.Telefone ?? "",
                    c.Cidade ?? "",
                    c.Uf ?? "",
                    c.DataCadastro.ToString("dd/MM/yyyy"));
            })
            .ToList();

        AtualizarStatus();
    }

    private void TxtBusca_TextChanged(object sender, TextChangedEventArgs e) => Carregar();

    private void AtualizarStatus()
    {
        TxtStatus.Text = string.Format(LocalizationService.Get("StatusFormat"), _total, _db.CaminhoBanco);
    }

    // ══════════════════════════════════════════════════════════════
    //  CRUD
    // ══════════════════════════════════════════════════════════════

    private async void Novo_Click(object sender, RoutedEventArgs e)
    {
        var dlg = new ClienteDialog(_db);
        Configurar(dlg);
        if (await dlg.ShowAsync() == ContentDialogResult.Primary) Carregar();
    }

    private async void Editar_Click(object sender, RoutedEventArgs e) => await EditarAsync();

    private async void Lista_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e) => await EditarAsync();

    private async Task EditarAsync()
    {
        if (Selecionado is not { } linha)
        {
            await MensagemAsync(LocalizationService.Get("Attention"),
                                LocalizationService.Get("SelectToEdit"));
            return;
        }

        var cliente = _db.ObterPorId(linha.Id);
        if (cliente is null) return;

        var dlg = new ClienteDialog(_db, cliente);
        Configurar(dlg);
        if (await dlg.ShowAsync() == ContentDialogResult.Primary) Carregar();
    }

    private async void Excluir_Click(object sender, RoutedEventArgs e)
    {
        if (Selecionado is not { } linha)
        {
            await MensagemAsync(LocalizationService.Get("Attention"),
                                LocalizationService.Get("SelectToDelete"));
            return;
        }

        bool confirmado = await ConfirmarAsync(
            LocalizationService.Get("ConfirmDelete"),
            string.Format(LocalizationService.Get("ConfirmDeleteMsg"), linha.Nome),
            LocalizationService.Get("Delete"));

        if (!confirmado) return;

        _db.Excluir(linha.Id);
        Carregar();
    }

    // ══════════════════════════════════════════════════════════════
    //  IMPRESSÃO
    // ══════════════════════════════════════════════════════════════

    private async void Imprimir_Click(object sender, RoutedEventArgs e)
    {
        if (Selecionado is not { } linha)
        {
            await MensagemAsync(LocalizationService.Get("Attention"),
                                LocalizationService.Get("SelectToPrint"));
            return;
        }

        var cliente = _db.ObterPorId(linha.Id);
        if (cliente is null) return;

        var html = GerarHtmlFicha(cliente);
        var arquivo = Path.Combine(Path.GetTempPath(), $"ficha_cliente_{cliente.Id}_{Guid.NewGuid():N}.html");
        File.WriteAllText(arquivo, html, System.Text.Encoding.UTF8);

        // Abre o navegador padrão — o usuário imprime com Ctrl+P
        Process.Start(new ProcessStartInfo(arquivo) { UseShellExecute = true });
    }

    private string GerarHtmlFicha(Cliente c)
    {
        var L = LocalizationService.Get;
        var idioma = LocalizationService.IdiomaAtual;
        var pais = CountryRegistry.ObterPorCodigo(c.PaisResidencia);
        var docInfo = CountryRegistry.ObterDocumentoPorId(c.TipoDocumento);
        string docFormatado = Validador.FormatarDocumento(c.Documento, c.TipoDocumento);

        string nomePaisTraduzido = InternationalDocumentStrings.GetCountryName(pais.Code, idioma);
        string nomeDocTraduzido = docInfo is not null
            ? InternationalDocumentStrings.GetDocumentOfficialName(docInfo.Id, idioma)
            : L("ColDocument");
        string docLabel = docInfo is not null ? $"{docInfo.CodeName} ({nomeDocTraduzido})" : L("ColDocument");
        string paisLabel = InternationalDocumentStrings.GetCountryLabel(idioma).TrimEnd(' ', '*');
        string nacionalidadeLabel = InternationalDocumentStrings.GetNationalityLabel(idioma);

        bool escuro = ThemeService.Tipo == TemaTipo.Escuro;
        string bgPag  = escuro ? "#1E1E1E" : "#F3F4F6";
        string bgCard = escuro ? "#2D2D2D" : "#FFFFFF";
        string fg     = escuro ? "#E0E0E0" : "#111827";
        string accent = "#2563EB";
        string linha  = escuro ? "#484848" : "#E5E7EB";

        string Campo(string label, string? valor)
        {
            if (string.IsNullOrWhiteSpace(valor)) return "";
            return $@"
            <tr>
              <td style=""padding:8px 12px;font-weight:600;color:{fg};border-bottom:1px solid {linha};width:35%"">{label}</td>
              <td style=""padding:8px 12px;color:{fg};border-bottom:1px solid {linha}"">{System.Net.WebUtility.HtmlEncode(valor)}</td>
            </tr>";
        }

        return $@"<!DOCTYPE html>
<html lang=""{(LocalizationService.IdiomaAtual == Idioma.Ar ? "ar" : "pt-BR")}""
      dir=""{(LocalizationService.IdiomaAtual == Idioma.Ar ? "rtl" : "ltr")}"">
<head>
  <meta charset=""UTF-8"" />
  <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"" />
  <title>{System.Net.WebUtility.HtmlEncode(L("PrintTitle"))}</title>
  <style>
    @media print {{
      body {{ -webkit-print-color-adjust: exact; print-color-adjust: exact; }}
      .no-print {{ display: none; }}
    }}
    body {{
      font-family: 'Segoe UI', Arial, sans-serif;
      background: {bgPag};
      color: {fg};
      margin: 0;
      padding: 24px;
    }}
    .card {{
      background: {bgCard};
      border-radius: 12px;
      max-width: 720px;
      margin: 0 auto;
      overflow: hidden;
      box-shadow: 0 2px 16px rgba(0,0,0,0.15);
    }}
    .header {{
      background: {accent};
      padding: 24px 32px;
      display: flex;
      align-items: center;
      gap: 20px;
    }}
    .logo-box {{
      background: rgba(255,255,255,0.15);
      border-radius: 10px;
      width: 52px; height: 52px;
      display: flex; align-items: center; justify-content: center;
      font-size: 24px; font-weight: 900; color: #fff; letter-spacing: -2px;
    }}
    .header-text h1 {{
      margin: 0 0 4px;
      font-size: 22px;
      color: #fff;
    }}
    .header-text p {{
      margin: 0;
      font-size: 13px;
      color: rgba(255,255,255,0.8);
    }}
    .section-title {{
      background: {(escuro ? "#383838" : "#F9FAFB")};
      padding: 10px 20px;
      font-size: 11px;
      font-weight: 700;
      letter-spacing: 1.2px;
      color: {(escuro ? "#AAAAAA" : "#6B7280")};
      text-transform: uppercase;
      border-bottom: 1px solid {linha};
    }}
    table {{ width: 100%; border-collapse: collapse; }}
    td {{ font-size: 14px; }}
    .footer {{
      padding: 16px 20px;
      text-align: center;
      font-size: 11px;
      color: {(escuro ? "#888" : "#9CA3AF")};
      border-top: 1px solid {linha};
    }}
    .btn-print {{
      display: block;
      margin: 20px auto 0;
      padding: 12px 32px;
      background: {accent};
      color: #fff;
      border: none;
      border-radius: 8px;
      font-size: 15px;
      cursor: pointer;
      font-family: inherit;
    }}
    .btn-print:hover {{ opacity: 0.9; }}
  </style>
</head>
<body>
  <div class=""card"">
    <div class=""header"">
      <div class=""logo-box"">TK</div>
      <div class=""header-text"">
        <h1>{System.Net.WebUtility.HtmlEncode(L("AppTitle"))}</h1>
        <p>{System.Net.WebUtility.HtmlEncode(L("PrintTitle"))}</p>
      </div>
    </div>
    <div class=""section-title"">{System.Net.WebUtility.HtmlEncode(L("PrintClient"))}</div>
    <table>
      {Campo(paisLabel, $"{pais.FlagEmoji} {nomePaisTraduzido} ({pais.NativeName})")}
      {Campo(L("ColName"),     c.Nome)}
      {Campo(docLabel,         docFormatado)}
      {Campo(nacionalidadeLabel, c.Nacionalidade)}
      {Campo(L("ColEmail"),    c.Email)}
      {Campo(L("ColPhone"),    c.Telefone)}
      {Campo(L("DlgBirthdate"), c.DataNascimento?.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture))}
      {Campo(pais.PostalCodeName, c.Cep is not null and not "" ? DocumentMaskService.FormatarCodigoPostal(c.Cep, pais.PostalCodeMask) : null)}
      {Campo(L("DlgAddress"),  c.Logradouro)}
      {Campo(L("DlgNumber"),   c.Numero)}
      {Campo(L("DlgNeighborhood"), c.Bairro)}
      {Campo(L("ColCity"),     c.Cidade)}
      {Campo(pais.StateOrProvinceLabel, c.Uf)}
      {Campo(L("DlgNotes"),    c.Observacoes)}
      {Campo(L("ColCreated"),  c.DataCadastro.ToString("dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture))}
    </table>
    <div class=""footer"">
      {System.Net.WebUtility.HtmlEncode(L("AppTitle"))} &nbsp;•&nbsp;
      🔒 LGPD & GDPR Compliant &nbsp;•&nbsp;
      {DateTime.Now:dd/MM/yyyy HH:mm}
    </div>
  </div>
  <button class=""btn-print no-print"" onclick=""window.print()"">🖨 {System.Net.WebUtility.HtmlEncode(L("BtnPrint"))}</button>
</body>
</html>";
    }

    // ══════════════════════════════════════════════════════════════
    //  GOOGLE DRIVE
    // ══════════════════════════════════════════════════════════════

    private async void ConfigurarDrive_Click(object sender, RoutedEventArgs e)
    {
        if (await ImportarCredenciaisAsync())
            await MensagemAsync(LocalizationService.Get("GoogleDrive"),
                string.Format(LocalizationService.Get("DriveConfigured"), _drive.CaminhoClientSecret));
    }

    private async void Backup_Click(object sender, RoutedEventArgs e) =>
        await ExecutarAsync(LocalizationService.Get("BackupSending"), async () =>
        {
            var copia = Path.Combine(Path.GetTempPath(), $"clientes_tmp_{Guid.NewGuid():N}.db");
            try
            {
                _db.CriarCopia(copia);
                await GarantirConexaoAsync();

                var enviado = await _drive.EnviarAsync(copia);
                await _drive.RemoverAntigosAsync(manter: 10);

                await MensagemAsync(LocalizationService.Get("Backup"),
                    string.Format(LocalizationService.Get("BackupSuccess"), enviado.Nome));
            }
            finally
            {
                if (File.Exists(copia)) File.Delete(copia);
            }
        });

    private async void Restaurar_Click(object sender, RoutedEventArgs e) =>
        await ExecutarAsync(LocalizationService.Get("Connecting"), RestaurarAsync);

    private async Task RestaurarAsync()
    {
        await GarantirConexaoAsync();

        var backups = (await _drive.ListarAsync())
            .Select(b => new LinhaBackup(
                b.Id,
                b.Nome,
                b.Criado.ToString("dd/MM/yyyy HH:mm"),
                $"{b.Tamanho / 1024.0:N1} KB"))
            .ToList();

        if (backups.Count == 0)
        {
            await MensagemAsync(LocalizationService.Get("Restore"),
                                LocalizationService.Get("NoBackup"));
            return;
        }

        var seletor = new RestauracaoDialog(backups);
        Configurar(seletor);
        if (await seletor.ShowAsync() != ContentDialogResult.Primary || seletor.Escolhido is null)
            return;

        var escolhido = seletor.Escolhido;

        bool confirmado = await ConfirmarAsync(
            LocalizationService.Get("RestoreConfirm"),
            LocalizationService.Get("RestoreConfirmMsg"),
            LocalizationService.Get("Restore"));

        if (!confirmado) return;

        var destino = Path.Combine(Path.GetTempPath(), $"restore_{Guid.NewGuid():N}.db");
        try
        {
            await _drive.BaixarAsync(escolhido.Id, destino);

            var seguranca = Path.Combine(DatabaseService.PastaDados,
                $"antes_restauracao_{DateTime.Now:yyyyMMdd_HHmmss}.db");
            _db.CriarCopia(seguranca);
            _db.RestaurarDe(destino);

            Carregar();
            await MensagemAsync(LocalizationService.Get("Restore"),
                                LocalizationService.Get("RestoreSuccess"));
        }
        finally
        {
            if (File.Exists(destino)) File.Delete(destino);
        }
    }

    private async Task GarantirConexaoAsync()
    {
        if (_drive.Conectado) return;

        if (!_drive.ConfiguracaoExiste && !await ImportarCredenciaisAsync())
            throw new InvalidOperationException(LocalizationService.Get("NoCred"));

        await _drive.ConectarAsync();
    }

    private async Task<bool> ImportarCredenciaisAsync()
    {
        var picker = new FileOpenPicker();
        InitializeWithWindow.Initialize(picker, WindowNative.GetWindowHandle(this));
        picker.FileTypeFilter.Add(".json");
        picker.SuggestedStartLocation = PickerLocationId.Downloads;

        var arquivo = await picker.PickSingleFileAsync();
        if (arquivo is null) return false;

        try
        {
            _drive.ImportarClientSecret(arquivo.Path);
            return true;
        }
        catch (Exception ex)
        {
            await MensagemAsync(LocalizationService.Get("InvalidCred"),
                string.Format(LocalizationService.Get("InvalidCredMsg"), ex.Message));
            return false;
        }
    }

    // ══════════════════════════════════════════════════════════════
    //  EXECUÇÃO COM INDICADOR DE PROGRESSO
    // ══════════════════════════════════════════════════════════════

    private async Task ExecutarAsync(string mensagem, Func<Task> acao)
    {
        Ocupado(true, mensagem);
        try
        {
            await acao();
        }
        catch (Exception ex)
        {
            await MensagemAsync(LocalizationService.Get("Error"), ex.Message);
        }
        finally
        {
            Ocupado(false, "");
        }
    }

    private void Ocupado(bool ocupado, string mensagem)
    {
        foreach (var child in Acoes.Children.OfType<Control>())
            child.IsEnabled = !ocupado;

        BtnTema.IsEnabled = !ocupado;
        BtnKofi.IsEnabled = !ocupado;
        CmbIdioma.IsEnabled = !ocupado;
        Progresso.IsActive = ocupado;
        TxtStatus.Text = ocupado ? mensagem : string.Format(LocalizationService.Get("StatusFormat"), _total, _db.CaminhoBanco);
    }

    // ══════════════════════════════════════════════════════════════
    //  CAIXAS DE DIÁLOGO
    // ══════════════════════════════════════════════════════════════

    private void Configurar(ContentDialog dialogo)
    {
        dialogo.XamlRoot = Raiz.XamlRoot;
        dialogo.RequestedTheme = Raiz.RequestedTheme;
    }

    private async Task MensagemAsync(string titulo, string texto)
    {
        var dlg = new ContentDialog
        {
            Title = titulo,
            Content = new TextBlock { Text = texto, TextWrapping = TextWrapping.Wrap },
            CloseButtonText = LocalizationService.Get("OK")
        };
        Configurar(dlg);
        await dlg.ShowAsync();
    }

    private async Task<bool> ConfirmarAsync(string titulo, string texto, string textoPrimario)
    {
        var dlg = new ContentDialog
        {
            Title = titulo,
            Content = new TextBlock { Text = texto, TextWrapping = TextWrapping.Wrap },
            PrimaryButtonText = textoPrimario,
            CloseButtonText = LocalizationService.Get("Cancel"),
            DefaultButton = ContentDialogButton.Close
        };
        Configurar(dlg);
        return await dlg.ShowAsync() == ContentDialogResult.Primary;
    }
}

/// <summary>Linha exibida na lista de clientes.</summary>
public sealed record LinhaCliente(
    int Id, string Nome, string Documento, string Email, string Telefone,
    string Cidade, string Uf, string Cadastro);

