using CadastroClientes.InternationalDocumentManager.CountryConfiguration;
using CadastroClientes.InternationalDocumentManager.DocumentMasks;
using CadastroClientes.InternationalDocumentManager.DocumentValidators;

namespace CadastroClientes.Models;

public static class Validador
{
    public static string Digitos(string? texto) =>
        new string((texto ?? "").Where(char.IsDigit).ToArray());

    public static string Alfanumerico(string? texto) =>
        new string((texto ?? "").Where(char.IsLetterOrDigit).ToArray());

    /// <summary>
    /// Valida documento conforme o tipo de documento internacional.
    /// Se o tipo não for especificado, detecta automaticamente CPF (11) ou CNPJ (14).
    /// </summary>
    public static bool DocumentoValido(string documento, string? docTypeId = null)
    {
        if (string.IsNullOrWhiteSpace(documento)) return false;

        if (!string.IsNullOrWhiteSpace(docTypeId))
        {
            var res = DocumentValidatorRegistry.Validar(documento, docTypeId);
            return res.IsValid;
        }

        // Detecção automática retrocompatível para Brasil (CPF/CNPJ)
        var d = Digitos(documento);
        return d.Length switch
        {
            11 => DocumentValidatorRegistry.Validar(d, "BR_CPF").IsValid,
            14 => DocumentValidatorRegistry.Validar(d, "BR_CNPJ").IsValid,
            _ => false
        };
    }

    /// <summary>
    /// Formata documento conforme máscara internacional ou padrão Brasil.
    /// </summary>
    public static string FormatarDocumento(string documento, string? docTypeId = null)
    {
        if (string.IsNullOrWhiteSpace(documento)) return "";

        if (!string.IsNullOrWhiteSpace(docTypeId))
        {
            var docInfo = CountryRegistry.ObterDocumentoPorId(docTypeId);
            if (docInfo is not null)
            {
                return DocumentMaskService.AplicarMascara(documento, docInfo);
            }
        }

        // Padrão Brasil por tamanho de dígitos
        var d = Digitos(documento);
        return d.Length switch
        {
            11 => $"{d[..3]}.{d[3..6]}.{d[6..9]}-{d[9..]}",
            14 => $"{d[..2]}.{d[2..5]}.{d[5..8]}/{d[8..12]}-{d[12..]}",
            _ => documento
        };
    }

    public static string FormatarCep(string cep)
    {
        var d = Digitos(cep);
        return d.Length == 8 ? $"{d[..5]}-{d[5..]}" : cep;
    }
}
