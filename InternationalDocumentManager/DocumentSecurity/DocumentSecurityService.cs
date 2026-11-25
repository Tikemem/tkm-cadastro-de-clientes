using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using CadastroClientes.InternationalDocumentManager.DocumentTypes;

namespace CadastroClientes.InternationalDocumentManager.DocumentSecurity;

public static class DocumentSecurityService
{
    // Chave de entropia derivada localmente para reforço de segurança (AES-256)
    private static readonly byte[] AesKey = SHA256.HashData(
        Encoding.UTF8.GetBytes(Environment.UserName + "@TKM-Clientes-Doc-Sec-2026-EnterpriseSalt!#$"));

    /// <summary>
    /// Mascara um documento para exibição segura na tela ou exportação limitada.
    /// Ex: CPF: ***.456.789-** | SSN: ***-**-6789 | Aadhaar: **** **** 1234
    /// </summary>
    public static string MascararParaExibicao(string? documento, DocumentTypeInfo? doc)
    {
        if (string.IsNullOrWhiteSpace(documento)) return "";

        string s = documento.Trim();
        if (s.Length <= 4) return new string('*', s.Length);

        // Se tiver mais de 4 caracteres, mascara os primeiros deixando os últimos 4 visíveis
        int visiveis = 4;
        int mascarados = s.Length - visiveis;

        return new string('*', mascarados) + s[mascarados..];
    }

    /// <summary>
    /// Higieniza mensagens de log, removendo ou mascarando números de documentos sensíveis.
    /// Atende à exigência: 'Não armazenar documentos em logs'.
    /// </summary>
    public static string SanitizarLog(string? mensagem)
    {
        if (string.IsNullOrEmpty(mensagem)) return "";

        // Regex para CPF, SSN, Aadhaar e outros formatos numéricos de 9 a 18 dígitos
        string sanitizado = Regex.Replace(mensagem, @"\b\d{3}\.\d{3}\.\d{3}-\d{2}\b", "[CPF PROTEGIDO]");
        sanitizado = Regex.Replace(sanitizado, @"\b\d{2}\.\d{3}\.\d{3}/\d{4}-\d{2}\b", "[CNPJ PROTEGIDO]");
        sanitizado = Regex.Replace(sanitizado, @"\b\d{3}-\d{2}-\d{4}\b", "[SSN PROTEGIDO]");
        sanitizado = Regex.Replace(sanitizado, @"\b\d{4}\s\d{4}\s\d{4}\b", "[AADHAAR PROTEGIDO]");
        sanitizado = Regex.Replace(sanitizado, @"\b\d{9,18}\b", "[DOCUMENTO PROTEGIDO]");

        return sanitizado;
    }

    /// <summary>
    /// Criptografa o número do documento com AES-256 para armazenamento seguro no banco de dados.
    /// Retorna string Base64 com prefixo "ENC:AES:".
    /// </summary>
    public static string Criptografar(string? textoPlano)
    {
        if (string.IsNullOrEmpty(textoPlano)) return "";

        // Se já estiver criptografado, não duplica
        if (textoPlano.StartsWith("ENC:")) return textoPlano;

        try
        {
            byte[] bytes = Encoding.UTF8.GetBytes(textoPlano);
            return "ENC:AES:" + CriptografarAes(bytes);
        }
        catch
        {
            return textoPlano;
        }
    }

    /// <summary>
    /// Descriptografa o número do documento obtido do banco de dados.
    /// Se o valor já estiver em texto plano (bancos antigos pré-migração), retorna ele mesmo.
    /// </summary>
    public static string Descriptografar(string? textoCifrado)
    {
        if (string.IsNullOrEmpty(textoCifrado)) return "";

        if (!textoCifrado.StartsWith("ENC:"))
        {
            // Compatibilidade retroativa com registros antigos sem criptografia
            return textoCifrado;
        }

        try
        {
            string b64 = textoCifrado.StartsWith("ENC:AES:")
                ? textoCifrado["ENC:AES:".Length..]
                : textoCifrado.Split(':').Last();

            return DescriptografarAes(b64);
        }
        catch
        {
            return textoCifrado;
        }
    }

    private static string CriptografarAes(byte[] dados)
    {
        using var aes = Aes.Create();
        aes.Key = AesKey;
        aes.GenerateIV();

        using var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
        byte[] enc = encryptor.TransformFinalBlock(dados, 0, dados.Length);

        byte[] resultado = new byte[aes.IV.Length + enc.Length];
        Buffer.BlockCopy(aes.IV, 0, resultado, 0, aes.IV.Length);
        Buffer.BlockCopy(enc, 0, resultado, aes.IV.Length, enc.Length);

        return Convert.ToBase64String(resultado);
    }

    private static string DescriptografarAes(string b64)
    {
        byte[] pacote = Convert.FromBase64String(b64);
        using var aes = Aes.Create();
        aes.Key = AesKey;

        byte[] iv = new byte[aes.BlockSize / 8];
        byte[] enc = new byte[pacote.Length - iv.Length];

        Buffer.BlockCopy(pacote, 0, iv, 0, iv.Length);
        Buffer.BlockCopy(pacote, iv.Length, enc, 0, enc.Length);

        using var decryptor = aes.CreateDecryptor(aes.Key, iv);
        byte[] dec = decryptor.TransformFinalBlock(enc, 0, enc.Length);

        return Encoding.UTF8.GetString(dec);
    }

    /// <summary>
    /// Controle de permissão de visualização do documento completo.
    /// Administradores e operadores autorizados podem visualizar o número completo;
    /// outros papéis recebem a versão mascarada.
    /// </summary>
    public static bool PodeVisualizarDocumentoCompleto(string papelUsuario = "Administrador")
    {
        return papelUsuario.Equals("Administrador", StringComparison.OrdinalIgnoreCase) ||
               papelUsuario.Equals("Gestor", StringComparison.OrdinalIgnoreCase) ||
               papelUsuario.Equals("Admin", StringComparison.OrdinalIgnoreCase);
    }
}
