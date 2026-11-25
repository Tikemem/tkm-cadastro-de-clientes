using System.Text;
using CadastroClientes.InternationalDocumentManager.DocumentTypes;

namespace CadastroClientes.InternationalDocumentManager.DocumentMasks;

public static class DocumentMaskService
{
    /// <summary>
    /// Limpa a string de entrada mantendo apenas os caracteres válidos de acordo com o tipo de documento.
    /// Converte letras para maiúsculo por padrão.
    /// </summary>
    public static string LimparRaw(string? entrada, DocumentTypeInfo doc)
    {
        if (string.IsNullOrEmpty(entrada)) return "";

        var allowed = new HashSet<char>(doc.AllowedChars);
        var sb = new StringBuilder();

        foreach (char ch in entrada)
        {
            char upper = char.ToUpperInvariant(ch);
            if (allowed.Contains(ch) || allowed.Contains(upper))
            {
                // Se o allowed tiver a versão maiúscula, normaliza para maiúsculo
                sb.Append(char.IsLetter(ch) ? upper : ch);
            }
        }

        string raw = sb.ToString();
        if (doc.MaxRawLength > 0 && raw.Length > doc.MaxRawLength)
        {
            raw = raw[..doc.MaxRawLength];
        }

        return raw;
    }

    /// <summary>
    /// Aplica a máscara correspondente ao texto cru (raw).
    /// </summary>
    public static string AplicarMascara(string? entrada, DocumentTypeInfo doc)
    {
        string raw = LimparRaw(entrada, doc);
        if (string.IsNullOrEmpty(raw)) return "";

        string mask = doc.Mask;
        if (string.IsNullOrEmpty(mask)) return raw;

        var sb = new StringBuilder();
        int rawIndex = 0;

        for (int i = 0; i < mask.Length && rawIndex < raw.Length; i++)
        {
            char m = mask[i];

            if (m is '0' or 'A')
            {
                sb.Append(raw[rawIndex++]);
            }
            else
            {
                // Separador fixo da máscara (ex: '.', '-', '/', ' ')
                sb.Append(m);
            }
        }

        return sb.ToString();
    }

    /// <summary>
    /// Valida se um caractere digitado é válido para o documento.
    /// </summary>
    public static bool CaractereValido(char c, DocumentTypeInfo doc)
    {
        char upper = char.ToUpperInvariant(c);
        return doc.AllowedChars.Contains(c) || doc.AllowedChars.Contains(upper);
    }

    /// <summary>
    /// Formata CEP / Código Postal conforme a máscara do país.
    /// </summary>
    public static string FormatarCodigoPostal(string? entrada, string mascara)
    {
        if (string.IsNullOrWhiteSpace(entrada) || string.IsNullOrWhiteSpace(mascara))
            return entrada ?? "";

        var raw = new string(entrada.Where(char.IsLetterOrDigit).ToArray());
        var sb = new StringBuilder();
        int rawIdx = 0;

        for (int i = 0; i < mascara.Length && rawIdx < raw.Length; i++)
        {
            if (mascara[i] is '0' or 'A')
            {
                sb.Append(raw[rawIdx++]);
            }
            else
            {
                sb.Append(mascara[i]);
            }
        }

        // Se houver caracteres restantes além da máscara
        while (rawIdx < raw.Length)
        {
            sb.Append(raw[rawIdx++]);
        }

        return sb.ToString();
    }

    /// <summary>
    /// Formata número de telefone conforme padrão internacional ou máscara simples.
    /// </summary>
    public static string FormatarTelefone(string? entrada, string prefixoPais)
    {
        if (string.IsNullOrWhiteSpace(entrada)) return "";

        var digits = new string(entrada.Where(char.IsDigit).ToArray());
        if (digits.Length == 0) return "";

        // Se o número já começar com o DDI do país
        string ddiDigits = new string(prefixoPais.Where(char.IsDigit).ToArray());
        if (digits.StartsWith(ddiDigits) && digits.Length > ddiDigits.Length)
        {
            string local = digits[ddiDigits.Length..];
            return FormatarTelefoneLocal(local, prefixoPais);
        }

        return FormatarTelefoneLocal(digits, prefixoPais);
    }

    private static string FormatarTelefoneLocal(string digits, string prefixoPais)
    {
        if (digits.Length == 11) // Brasil celular com DDD: (11) 98765-4321
            return $"{prefixoPais} ({digits[..2]}) {digits[2..7]}-{digits[7..]}";
        if (digits.Length == 10) // Brasil fixo ou US: (11) 3456-7890 ou (555) 123-4567
            return $"{prefixoPais} ({digits[..2]}) {digits[2..6]}-{digits[6..]}";
        if (digits.Length == 9)  // Portugal / Espanha: 912 345 678
            return $"{prefixoPais} {digits[..3]} {digits[3..6]} {digits[6..]}";

        return $"{prefixoPais} {digits}";
    }
}
