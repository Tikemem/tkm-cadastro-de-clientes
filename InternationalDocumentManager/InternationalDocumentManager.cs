using CadastroClientes.InternationalDocumentManager.CountryConfiguration;
using CadastroClientes.InternationalDocumentManager.DocumentMasks;
using CadastroClientes.InternationalDocumentManager.DocumentSecurity;
using CadastroClientes.InternationalDocumentManager.DocumentTypes;
using CadastroClientes.InternationalDocumentManager.DocumentValidators;
using CadastroClientes.InternationalDocumentManager.Internationalization;
using CadastroClientes.Services;

namespace CadastroClientes.InternationalDocumentManager;

/// <summary>
/// Módulo Central Independente e Expansível de Gerenciamento Internacional de Documentos.
/// Orquestra Configuração de Países, Tipos de Documentos, Máscaras Dinâmicas,
/// Validadores Oficiais, Internacionalização e Segurança Criptográfica / LGPD / GDPR.
/// </summary>
public static class InternationalDocumentManager
{
    // ── ACESSO AOS PAÍSES E CONFIGURAÇÕES ──

    public static IReadOnlyList<CountryInfo> PaisesDisponiveis => CountryRegistry.TodosPaises;
    public static IReadOnlyList<CountryInfo> PaisesPrincipais => CountryRegistry.PaisesPrincipais;
    public static IReadOnlyList<CountryInfo> PaisesArabes => CountryRegistry.PaisesArabes;

    public static CountryInfo ObterPais(string? codigoPais) =>
        CountryRegistry.ObterPorCodigo(codigoPais);

    public static DocumentTypeInfo? ObterTipoDocumento(string? docId) =>
        CountryRegistry.ObterDocumentoPorId(docId);

    // ── MÁSCARAS E FORMATAÇÃO DINÂMICA ──

    public static string AplicarMascara(string? entrada, string? docId)
    {
        if (string.IsNullOrWhiteSpace(docId)) return entrada ?? "";
        var doc = ObterTipoDocumento(docId);
        return doc is null ? (entrada ?? "") : DocumentMaskService.AplicarMascara(entrada, doc);
    }

    public static string LimparRaw(string? entrada, string? docId)
    {
        if (string.IsNullOrWhiteSpace(docId)) return entrada ?? "";
        var doc = ObterTipoDocumento(docId);
        return doc is null ? (entrada ?? "") : DocumentMaskService.LimparRaw(entrada, doc);
    }

    public static bool CaracterePermitido(char c, string? docId)
    {
        if (string.IsNullOrWhiteSpace(docId)) return true;
        var doc = ObterTipoDocumento(docId);
        return doc is null || DocumentMaskService.CaractereValido(c, doc);
    }

    public static string FormatarCodigoPostal(string? entrada, string? codigoPais)
    {
        var pais = ObterPais(codigoPais);
        return DocumentMaskService.FormatarCodigoPostal(entrada, pais.PostalCodeMask);
    }

    public static string FormatarTelefone(string? entrada, string? codigoPais)
    {
        var pais = ObterPais(codigoPais);
        return DocumentMaskService.FormatarTelefone(entrada, pais.PhoneDialCode);
    }

    // ── VALIDAÇÃO OFICIAL ──

    public static ValidationResult ValidarDocumento(string? rawInput, string? docId)
    {
        if (string.IsNullOrWhiteSpace(docId))
            return ValidationResult.Failure("Tipo de documento não informado.");

        return DocumentValidatorRegistry.Validar(rawInput, docId);
    }

    // ── SEGURANÇA, CRIPTOGRAFIA E LGPD/GDPR ──

    public static string MascararParaExibicao(string? doc, string? docId)
    {
        var info = ObterTipoDocumento(docId);
        return DocumentSecurityService.MascararParaExibicao(doc, info);
    }

    public static string CriptografarParaBanco(string? textoPlano) =>
        DocumentSecurityService.Criptografar(textoPlano);

    public static string DescriptografarDoBanco(string? textoCifrado) =>
        DocumentSecurityService.Descriptografar(textoCifrado);

    public static string SanitizarLog(string? mensagem) =>
        DocumentSecurityService.SanitizarLog(mensagem);

    // ── STRINGS INTERNACIONAIS E DISCLAIMERS ──

    public static string ObterRotuloPais(Idioma idioma) =>
        InternationalDocumentStrings.GetCountryLabel(idioma);

    public static string ObterRotuloTipoDocumento(Idioma idioma) =>
        InternationalDocumentStrings.GetDocumentTypeLabel(idioma);

    public static string ObterRotuloNacionalidade(Idioma idioma) =>
        InternationalDocumentStrings.GetNationalityLabel(idioma);

    public static string ObterDisclaimerMatematico(Idioma idioma) =>
        InternationalDocumentStrings.GetMathDisclaimer(idioma);

    public static string ObterErroDocumentoDuplicado(Idioma idioma) =>
        InternationalDocumentStrings.GetDuplicateDocError(idioma);
}
