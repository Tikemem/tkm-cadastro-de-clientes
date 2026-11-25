using System.Net.Mail;
using CadastroClientes.Data;
using CadastroClientes.InternationalDocumentManager.CountryConfiguration;
using CadastroClientes.InternationalDocumentManager.DocumentMasks;
using CadastroClientes.InternationalDocumentManager.DocumentTypes;
using CadastroClientes.InternationalDocumentManager.DocumentValidators;
using CadastroClientes.InternationalDocumentManager.Internationalization;
using CadastroClientes.Models;
using CadastroClientes.Services;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;

namespace CadastroClientes.Dialogs;

public sealed partial class ClienteDialog : ContentDialog
{
    private readonly DatabaseService _db;
    private readonly Cliente? _original;

    private CountryInfo _paisAtual = CountryRegistry.Brasil;
    private DocumentTypeInfo? _documentoAtual;
    private bool _inicializando = true;
    private bool _formatandoDoc = false;
    private bool _formatandoCep = false;
    private bool _formatandoTelefone = false;

    public ClienteDialog(DatabaseService db, Cliente? cliente = null)
    {
        InitializeComponent();

        _db = db;
        _original = cliente;
        _inicializando = true;

        // 1. Determina país inicial: do cliente (se edição) ou do idioma ativo do programa
        if (cliente is not null && !string.IsNullOrWhiteSpace(cliente.PaisResidencia))
        {
            _paisAtual = CountryRegistry.ObterPorCodigo(cliente.PaisResidencia);
        }
        else
        {
            // O formulário já abre no formato do país correspondente ao idioma do programa
            _paisAtual = LocalizationService.IdiomaAtual switch
            {
                Idioma.PtPt => CountryRegistry.Portugal,
                Idioma.En => CountryRegistry.EstadosUnidos,
                Idioma.ZhCn => CountryRegistry.China,
                Idioma.Hi => CountryRegistry.India,
                Idioma.Es => CountryRegistry.Espanha,
                Idioma.Ar => CountryRegistry.ArabiaSaudita,
                _ => CountryRegistry.Brasil
            };
        }

        // 2. Inicializa combos de Países e Sub-países com nomes traduzidos
        InicializarPaises();

        // 3. Seleciona país inicial e carrega documentos
        SelecionarPaisNoCombo(_paisAtual);

        // 4. Aplica idioma e textos conforme o país
        AtualizarTextosIdioma();

        // 5. Preenche dados se for edição
        if (cliente is not null)
        {
            PreencherDados(cliente);
        }
        else
        {
            // Aplica DDI e placeholder padrão
            TxtTelefone.PlaceholderText = _paisAtual.PhonePlaceholder;
            TxtCep.PlaceholderText = _paisAtual.PostalCodePlaceholder;
        }

        _inicializando = false;

        // Revalida documento inicial se houver texto
        if (!string.IsNullOrEmpty(TxtDocumento.Text))
        {
            ValidarDocumentoTempoReal();
        }

        PrimaryButtonClick += OnSalvar;
    }

    // ══════════════════════════════════════════════════════════════
    //  INICIALIZAÇÃO DOS SELETORES
    // ══════════════════════════════════════════════════════════════

    private void InicializarPaises()
    {
        var idioma = LocalizationService.IdiomaAtual;

        CmbPais.Items.Clear();

        // Lista de opções principais de países com tradução dinâmica no idioma do programa
        var opcoes = new (string Codigo, string Emoji)[]
        {
            ("BR", "🇧🇷"),
            ("PT", "🇵🇹"),
            ("US", "🇺🇸"),
            ("CN", "🇨🇳"),
            ("IN", "🇮🇳"),
            ("ES", "🇪🇸"),
            ("AR", "🇸🇦")
        };

        foreach (var opt in opcoes)
        {
            string nomeTraduzido = InternationalDocumentStrings.GetCountryName(opt.Codigo, idioma);
            CmbPais.Items.Add(new ComboBoxItem
            {
                Content = $"{opt.Emoji}  {nomeTraduzido}",
                Tag = opt.Codigo
            });
        }

        // Sub-países árabes traduzidos no idioma do programa
        CmbSubPaisArabe.Items.Clear();
        foreach (var sub in CountryRegistry.PaisesArabes)
        {
            string subNomeTraduzido = InternationalDocumentStrings.GetCountryName(sub.Code, idioma);
            CmbSubPaisArabe.Items.Add(new ComboBoxItem
            {
                Content = $"{sub.FlagEmoji}  {subNomeTraduzido} ({sub.NativeName})",
                Tag = sub.Code
            });
        }
    }

    private void SelecionarPaisNoCombo(CountryInfo pais)
    {
        string codigoAlvo = pais.IsArabicSubCountry ? "AR" : pais.Code;

        for (int i = 0; i < CmbPais.Items.Count; i++)
        {
            if (CmbPais.Items[i] is ComboBoxItem item && (string)item.Tag == codigoAlvo)
            {
                CmbPais.SelectedIndex = i;
                break;
            }
        }

        if (pais.IsArabicSubCountry)
        {
            PnlSubPaisArabe.Visibility = Visibility.Visible;
            for (int j = 0; j < CmbSubPaisArabe.Items.Count; j++)
            {
                if (CmbSubPaisArabe.Items[j] is ComboBoxItem subItem && (string)subItem.Tag == pais.Code)
                {
                    CmbSubPaisArabe.SelectedIndex = j;
                    break;
                }
            }
        }
        else
        {
            PnlSubPaisArabe.Visibility = Visibility.Collapsed;
        }

        AtualizarDocumentosSuportados();
    }

    private void AtualizarDocumentosSuportados(string? selecionarDocId = null)
    {
        var idioma = LocalizationService.IdiomaAtual;
        string? docAlvoId = selecionarDocId ?? _documentoAtual?.Id ?? _paisAtual.DefaultDocumentTypeId;

        bool prevInicializando = _inicializando;
        _inicializando = true;
        try
        {
            CmbTipoDoc.Items.Clear();

            int selectedIdx = 0;
            int idx = 0;

            foreach (var doc in _paisAtual.SupportedDocuments)
            {
                string categoriaBadge = InternationalDocumentStrings.GetCategoryBadge(doc.Category, idioma);
                string nomeOficial = InternationalDocumentStrings.GetDocumentOfficialName(doc.Id, idioma);

                CmbTipoDoc.Items.Add(new ComboBoxItem
                {
                    Content = $"{doc.CodeName}  {categoriaBadge} - {nomeOficial}",
                    Tag = doc
                });

                if (doc.Id.Equals(docAlvoId, StringComparison.OrdinalIgnoreCase))
                {
                    selectedIdx = idx;
                }

                idx++;
            }

            if (CmbTipoDoc.Items.Count > 0)
            {
                CmbTipoDoc.SelectedIndex = selectedIdx;
                _documentoAtual = _paisAtual.SupportedDocuments[selectedIdx];
                AtualizarCamposDocumento();
            }
        }
        finally
        {
            _inicializando = prevInicializando;
        }
    }

    // ══════════════════════════════════════════════════════════════
    //  EVENTOS DE SELEÇÃO DE PAÍS E DOCUMENTO
    // ══════════════════════════════════════════════════════════════

    private void CmbPais_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (_inicializando) return;
        if (CmbPais.SelectedItem is not ComboBoxItem item) return;
        string codigo = (string)item.Tag;

        if (codigo == "AR")
        {
            // Grupo árabe: exige seleção do país específico
            PnlSubPaisArabe.Visibility = Visibility.Visible;
            if (CmbSubPaisArabe.SelectedIndex < 0)
                CmbSubPaisArabe.SelectedIndex = 0;

            if (CmbSubPaisArabe.SelectedItem is ComboBoxItem subItem)
            {
                _paisAtual = CountryRegistry.ObterPorCodigo((string)subItem.Tag);
            }
            else
            {
                _paisAtual = CountryRegistry.ArabiaSaudita;
            }
        }
        else
        {
            PnlSubPaisArabe.Visibility = Visibility.Collapsed;
            _paisAtual = CountryRegistry.ObterPorCodigo(codigo);
        }

        // 1. Atualiza lista de documentos disponíveis
        AtualizarDocumentosSuportados();

        // 2. Atualiza placeholders dos campos de endereço e telefone sem perder dados
        var idioma = LocalizationService.IdiomaAtual;
        TxtTelefone.PlaceholderText = _paisAtual.PhonePlaceholder;
        TxtTelefone.Header = $"{LocalizationService.Get("DlgPhone")} ({_paisAtual.PhoneDialCode})";
        TxtCep.PlaceholderText = _paisAtual.PostalCodePlaceholder;
        TxtCep.Header = _paisAtual.PostalCodeName;
        TxtUf.Header = _paisAtual.StateOrProvinceLabel;
        TxtNacionalidade.PlaceholderText = InternationalDocumentStrings.GetCountryName(_paisAtual.Code, idioma);

        // 3. Revalida o documento digitado com as regras do novo país
        ValidarDocumentoTempoReal();
    }

    private void CmbSubPaisArabe_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (_inicializando) return;
        if (CmbSubPaisArabe.SelectedItem is not ComboBoxItem subItem) return;
        string codigoSub = (string)subItem.Tag;
        _paisAtual = CountryRegistry.ObterPorCodigo(codigoSub);

        AtualizarDocumentosSuportados();
        ValidarDocumentoTempoReal();
    }

    private void CmbTipoDoc_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (_inicializando) return;
        if (CmbTipoDoc.SelectedItem is ComboBoxItem item && item.Tag is DocumentTypeInfo doc)
        {
            _documentoAtual = doc;
            AtualizarCamposDocumento();
            ValidarDocumentoTempoReal();
        }
    }

    // ══════════════════════════════════════════════════════════════
    //  TEXTOS E IDIOMA DINÂMICO
    // ══════════════════════════════════════════════════════════════

    private void AtualizarTextosIdioma()
    {
        var L = LocalizationService.Get;
        var idioma = LocalizationService.IdiomaAtual;

        Title = _original is null ? L("NewClient") : L("EditClient");
        PrimaryButtonText = L("Save");
        CloseButtonText = L("Cancel");

        LblPais.Text = InternationalDocumentStrings.GetCountryLabel(idioma);
        LblSubPaisArabe.Text = InternationalDocumentStrings.GetArabicSubCountryLabel(idioma);
        LblTipoDoc.Text = InternationalDocumentStrings.GetDocumentTypeLabel(idioma);

        TxtNome.Header = L("DlgName");
        DtNascimento.Header = L("DlgBirthdate");
        TxtNacionalidade.Header = InternationalDocumentStrings.GetNationalityLabel(idioma);
        TxtNacionalidade.PlaceholderText = InternationalDocumentStrings.GetCountryName(_paisAtual.Code, idioma);

        TxtEmail.Header = L("DlgEmail");
        TxtTelefone.Header = $"{L("DlgPhone")} ({_paisAtual.PhoneDialCode})";

        TxtLogradouro.Header = L("DlgAddress");
        TxtNumero.Header = L("DlgNumber");
        TxtBairro.Header = L("DlgNeighborhood");
        TxtCidade.Header = L("DlgCity");
        TxtUf.Header = _paisAtual.StateOrProvinceLabel;
        TxtCep.Header = _paisAtual.PostalCodeName;

        TxtObs.Header = L("DlgNotes");

        TxtDisclaimerLegal.Text = "⚖ " + InternationalDocumentStrings.GetMathDisclaimer(idioma) +
            " Conformidade com LGPD (Lei 13.709/18) e GDPR (Reg. UE 2016/679). Documentos protegidos com criptografia.";
    }

    private void AtualizarCamposDocumento()
    {
        if (_documentoAtual is null) return;
        var idioma = LocalizationService.IdiomaAtual;

        string nomeOficial = InternationalDocumentStrings.GetDocumentOfficialName(_documentoAtual.Id, idioma);
        LblDocOficial.Text = $"{_documentoAtual.CodeName}  •  {nomeOficial}";
        TxtBadgePais.Text = _paisAtual.FlagEmoji;
        TxtDocumento.PlaceholderText = _documentoAtual.Placeholder;

        string prefixoExemplo = InternationalDocumentStrings.GetExamplePrefix(idioma);
        string mascaraRotulo = InternationalDocumentStrings.GetMaskLabel(idioma);
        TxtExemploDoc.Text = $"{prefixoExemplo} {_documentoAtual.Placeholder}  ({mascaraRotulo} {_documentoAtual.Mask})";

        if (_documentoAtual.IsSensitive && !string.IsNullOrEmpty(_documentoAtual.SensitivityNote))
        {
            TxtAvisoDoc.Text = "🔒 " + _documentoAtual.SensitivityNote;
            TxtAvisoDoc.Visibility = Visibility.Visible;
        }
        else
        {
            TxtAvisoDoc.Visibility = Visibility.Collapsed;
        }

        // Aplica a máscara no valor atual do TextBox se já houver texto
        if (!string.IsNullOrEmpty(TxtDocumento.Text))
        {
            _formatandoDoc = true;
            TxtDocumento.Text = DocumentMaskService.AplicarMascara(TxtDocumento.Text, _documentoAtual);
            _formatandoDoc = false;
        }
    }

    // ══════════════════════════════════════════════════════════════
    //  MÁSCARA E VALIDAÇÃO EM TEMPO REAL
    // ══════════════════════════════════════════════════════════════

    private void TxtDocumento_BeforeTextChanging(TextBox sender, TextBoxBeforeTextChangingEventArgs args)
    {
        if (_documentoAtual is null) return;

        // Permite apenas caracteres válidos para o documento em questão
        foreach (char c in args.NewText)
        {
            if (!DocumentMaskService.CaractereValido(c, _documentoAtual) && !_documentoAtual.Mask.Contains(c))
            {
                args.Cancel = true;
                return;
            }
        }
    }

    private void TxtDocumento_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (_formatandoDoc || _documentoAtual is null) return;

        _formatandoDoc = true;
        try
        {
            string textoOriginal = TxtDocumento.Text;
            int cursorOriginal = TxtDocumento.SelectionStart;

            // Quantos caracteres válidos (raw) existiam antes da posição do cursor
            int rawCharsBeforeCursor = 0;
            for (int i = 0; i < Math.Min(cursorOriginal, textoOriginal.Length); i++)
            {
                if (DocumentMaskService.CaractereValido(textoOriginal[i], _documentoAtual))
                    rawCharsBeforeCursor++;
            }

            string raw = DocumentMaskService.LimparRaw(textoOriginal, _documentoAtual);
            string mascarado = DocumentMaskService.AplicarMascara(raw, _documentoAtual);

            TxtDocumento.Text = mascarado;

            // Se o usuário está digitando sequencialmente no final, mantém o cursor sempre no final
            if (cursorOriginal >= textoOriginal.Length)
            {
                TxtDocumento.SelectionStart = mascarado.Length;
            }
            else
            {
                // Mapeia os caracteres raw para a posição exata após a formatação da máscara
                int novoCursor = 0;
                int rawEncontrados = 0;
                for (int i = 0; i < mascarado.Length; i++)
                {
                    if (rawEncontrados == rawCharsBeforeCursor)
                    {
                        novoCursor = i;
                        break;
                    }
                    if (DocumentMaskService.CaractereValido(mascarado[i], _documentoAtual))
                        rawEncontrados++;
                    novoCursor = i + 1;
                }
                TxtDocumento.SelectionStart = Math.Min(novoCursor, mascarado.Length);
            }
        }
        finally
        {
            _formatandoDoc = false;
        }

        ValidarDocumentoTempoReal();
    }

    private void TxtCep_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (_formatandoCep) return;
        _formatandoCep = true;
        try
        {
            TxtCep.Text = DocumentMaskService.FormatarCodigoPostal(TxtCep.Text, _paisAtual.PostalCodeMask);
            TxtCep.SelectionStart = TxtCep.Text.Length;
        }
        finally
        {
            _formatandoCep = false;
        }
    }

    private void TxtTelefone_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (_formatandoTelefone) return;
        _formatandoTelefone = true;
        try
        {
            string texto = TxtTelefone.Text;
            if (!string.IsNullOrWhiteSpace(texto) && !texto.StartsWith(_paisAtual.PhoneDialCode) && texto.All(c => char.IsDigit(c) || c == ' ' || c == '-' || c == '(' || c == ')'))
            {
                // Deixa livre digitação mantendo o prefixo
            }
        }
        finally
        {
            _formatandoTelefone = false;
        }
    }

    private bool ValidarDocumentoTempoReal()
    {
        if (_documentoAtual is null) return false;
        var idioma = LocalizationService.IdiomaAtual;

        string texto = TxtDocumento.Text.Trim();
        string raw = DocumentMaskService.LimparRaw(texto, _documentoAtual);

        // Se o campo estiver em branco
        if (string.IsNullOrEmpty(raw))
        {
            if (_documentoAtual.IsOptional)
            {
                AtualizarStatusVisual(true, InternationalDocumentStrings.GetStatusOptional(idioma), Colors.Gray, "ℹ");
                TxtErroDoc.Visibility = Visibility.Collapsed;
                return true;
            }

            AtualizarStatusVisual(false, InternationalDocumentStrings.GetStatusRequired(idioma), Colors.Gray, "ℹ");
            TxtErroDoc.Visibility = Visibility.Collapsed;
            return false;
        }

        // Se o tamanho ainda não atingiu o mínimo
        if (raw.Length < _documentoAtual.MinRawLength)
        {
            string statusDig = InternationalDocumentStrings.GetStatusTyping(idioma, raw.Length, _documentoAtual.MinRawLength);
            AtualizarStatusVisual(false, statusDig, Colors.Gray, "ℹ");
            TxtErroDoc.Text = statusDig;
            TxtErroDoc.Visibility = Visibility.Visible;
            return false;
        }

        // Executa validador oficial
        var resultado = DocumentValidatorRegistry.Validar(raw, _documentoAtual.Id);

        if (!resultado.IsValid)
        {
            AtualizarStatusVisual(false, resultado.ErrorMessage ?? "Documento inválido", Colors.Red, "✕");
            TxtErroDoc.Text = resultado.ErrorMessage;
            TxtErroDoc.Visibility = Visibility.Visible;
            return false;
        }

        // Verifica unicidade no banco de dados para evitar duplicações
        if (_db.DocumentoExiste(raw, _paisAtual.Code, _original?.Id))
        {
            string erroDup = InternationalDocumentStrings.GetDuplicateDocError(idioma);
            AtualizarStatusVisual(false, erroDup, Colors.Red, "✕");
            TxtErroDoc.Text = erroDup;
            TxtErroDoc.Visibility = Visibility.Visible;
            return false;
        }

        // Válido!
        string sucessoMsg = InternationalDocumentStrings.GetStatusValid(idioma, resultado.AlgorithmName);
        AtualizarStatusVisual(true, sucessoMsg, Colors.Green, "✓");
        TxtErroDoc.Visibility = Visibility.Collapsed;
        return true;
    }

    private void AtualizarStatusVisual(bool valido, string mensagem, Windows.UI.Color cor, string icone)
    {
        TxtIconeValidacao.Text = icone;
        TxtIconeValidacao.Foreground = new SolidColorBrush(cor);
        BrdStatusValidacao.Background = new SolidColorBrush(
            valido ? Windows.UI.Color.FromArgb(30, 16, 185, 129) : Windows.UI.Color.FromArgb(30, 239, 68, 68));
        ToolTipService.SetToolTip(BrdStatusValidacao, mensagem);
    }

    // ══════════════════════════════════════════════════════════════
    //  PREENCHIMENTO E SALVAMENTO
    // ══════════════════════════════════════════════════════════════

    private void PreencherDados(Cliente c)
    {
        TxtNome.Text = c.Nome;
        TxtNacionalidade.Text = c.Nacionalidade ?? "";
        TxtEmail.Text = c.Email ?? "";
        TxtTelefone.Text = c.Telefone ?? "";
        TxtCep.Text = c.Cep ?? "";
        TxtLogradouro.Text = c.Logradouro ?? "";
        TxtNumero.Text = c.Numero ?? "";
        TxtBairro.Text = c.Bairro ?? "";
        TxtCidade.Text = c.Cidade ?? "";
        TxtUf.Text = c.Uf ?? "";
        TxtObs.Text = c.Observacoes ?? "";

        if (c.DataNascimento is { } nasc)
            DtNascimento.Date = new DateTimeOffset(nasc);

        // Atualiza tipo de documento selecionado e valor
        AtualizarDocumentosSuportados(c.TipoDocumento);

        if (_documentoAtual is not null)
        {
            TxtDocumento.Text = DocumentMaskService.AplicarMascara(c.Documento, _documentoAtual);
        }
        else
        {
            TxtDocumento.Text = c.Documento;
        }
    }

    private void OnSalvar(ContentDialog sender, ContentDialogButtonClickEventArgs args)
    {
        var erro = ValidarFormularioGeral();
        if (erro is not null)
        {
            TxtErro.Text = erro;
            TxtErro.Visibility = Visibility.Visible;
            args.Cancel = true;
            return;
        }

        var c = _original ?? new Cliente();
        c.PaisResidencia = _paisAtual.Code;
        c.Nome = TxtNome.Text.Trim();
        c.TipoDocumento = _documentoAtual?.Id ?? _paisAtual.DefaultDocumentTypeId;

        // Salva documento em formato limpo/raw
        string rawDoc = DocumentMaskService.LimparRaw(TxtDocumento.Text, _documentoAtual ?? CountryRegistry.BrCpf);
        c.Documento = rawDoc;

        c.Nacionalidade = string.IsNullOrWhiteSpace(TxtNacionalidade.Text) ? null : TxtNacionalidade.Text.Trim();
        c.Email = string.IsNullOrWhiteSpace(TxtEmail.Text) ? null : TxtEmail.Text.Trim();
        c.Telefone = string.IsNullOrWhiteSpace(TxtTelefone.Text) ? null : TxtTelefone.Text.Trim();
        c.Cep = string.IsNullOrWhiteSpace(TxtCep.Text) ? null : TxtCep.Text.Trim();
        c.Logradouro = string.IsNullOrWhiteSpace(TxtLogradouro.Text) ? null : TxtLogradouro.Text.Trim();
        c.Numero = string.IsNullOrWhiteSpace(TxtNumero.Text) ? null : TxtNumero.Text.Trim();
        c.Bairro = string.IsNullOrWhiteSpace(TxtBairro.Text) ? null : TxtBairro.Text.Trim();
        c.Cidade = string.IsNullOrWhiteSpace(TxtCidade.Text) ? null : TxtCidade.Text.Trim();
        c.Uf = string.IsNullOrWhiteSpace(TxtUf.Text) ? null : TxtUf.Text.Trim().ToUpperInvariant();
        c.DataNascimento = DtNascimento.Date?.DateTime.Date;
        c.Observacoes = string.IsNullOrWhiteSpace(TxtObs.Text) ? null : TxtObs.Text.Trim();

        try
        {
            if (_original is null) _db.Inserir(c);
            else _db.Atualizar(c);
        }
        catch (Exception ex)
        {
            TxtErro.Text = string.Format(LocalizationService.Get("SaveError"), ex.Message);
            TxtErro.Visibility = Visibility.Visible;
            args.Cancel = true;
        }
    }

    private string? ValidarFormularioGeral()
    {
        var L = LocalizationService.Get;

        if (TxtNome.Text.Trim().Length < 3)
            return L("ValName");

        if (_documentoAtual is null)
            return "Selecione o tipo de documento.";

        string rawDoc = DocumentMaskService.LimparRaw(TxtDocumento.Text, _documentoAtual);

        // Se for obrigatório e estiver vazio
        if (!_documentoAtual.IsOptional && string.IsNullOrEmpty(rawDoc))
            return $"{_documentoAtual.CodeName} é obrigatório.";

        // Validação do documento
        if (!string.IsNullOrEmpty(rawDoc))
        {
            var validacao = DocumentValidatorRegistry.Validar(rawDoc, _documentoAtual.Id);
            if (!validacao.IsValid)
                return validacao.ErrorMessage ?? "Documento inválido.";

            // Duplicação
            if (_db.DocumentoExiste(rawDoc, _paisAtual.Code, _original?.Id))
                return InternationalDocumentStrings.GetDuplicateDocError(LocalizationService.IdiomaAtual);
        }

        // Validação de e-mail opcional
        if (!string.IsNullOrWhiteSpace(TxtEmail.Text) && !EmailValido(TxtEmail.Text))
            return L("ValEmail");

        return null;
    }

    private static bool EmailValido(string email)
    {
        try { _ = new MailAddress(email.Trim()); return true; }
        catch { return false; }
    }
}
