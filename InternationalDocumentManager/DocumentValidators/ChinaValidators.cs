using System.Globalization;

namespace CadastroClientes.InternationalDocumentManager.DocumentValidators;

public sealed class CnResidentIdValidator : IIdDocumentValidator
{
    public string DocumentTypeId => "CN_RESIDENT_ID";

    private static readonly HashSet<string> ValidProvinces = new()
    {
        "11", "12", "13", "14", "15",
        "21", "22", "23",
        "31", "32", "33", "34", "35", "36", "37",
        "41", "42", "43", "44", "45", "46",
        "50", "51", "52", "53", "54",
        "61", "62", "63", "64", "65",
        "71", "81", "82"
    };

    private static readonly int[] Weights = { 7, 9, 10, 5, 8, 4, 2, 1, 6, 3, 7, 9, 10, 5, 8, 4, 2 };
    private static readonly char[] CheckCodes = { '1', '0', 'X', '9', '8', '7', '6', '5', '4', '3', '2' };

    public ValidationResult Validate(string? rawInput)
    {
        if (string.IsNullOrWhiteSpace(rawInput))
            return ValidationResult.Failure("居民身份证号码为必填项 (O número de identidade é obrigatório).");

        var clean = new string(rawInput.Where(char.IsLetterOrDigit).ToArray()).ToUpperInvariant();
        if (clean.Length != 18)
            return ValidationResult.Failure("居民身份证号码必须为18位 (O documento deve conter 18 caracteres).");

        // 17 primeiros devem ser números
        for (int i = 0; i < 17; i++)
        {
            if (!char.IsDigit(clean[i]))
                return ValidationResult.Failure("居民身份证前17位必须是数字 (Os primeiros 17 caracteres devem ser números).");
        }

        // 1. Validar código de província
        string prov = clean[..2];
        if (!ValidProvinces.Contains(prov))
            return ValidationResult.Failure("无效的省份/行政区划代码 (Código de província/região administrativa inválido).", "GB 11643-1999");

        // 2. Validar data de nascimento (posições 6 a 13: YYYYMMDD)
        string dataStr = clean.Substring(6, 8);
        if (!DateTime.TryParseExact(dataStr, "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var dataNasc))
            return ValidationResult.Failure("身份证号码中的出生日期无效 (Data de nascimento inválida no documento).", "GB 11643-1999");

        if (dataNasc < new DateTime(1900, 1, 1) || dataNasc > DateTime.Today)
            return ValidationResult.Failure("出生日期超出合理范围 (Data de nascimento fora do intervalo válido).", "GB 11643-1999");

        // 3. Validar dígito verificador ISO 7064:1983.MOD 11-2
        int soma = 0;
        for (int i = 0; i < 17; i++)
        {
            soma += (clean[i] - '0') * Weights[i];
        }

        int resto = soma % 11;
        char checkEsperado = CheckCodes[resto];
        char checkInformado = clean[17];

        if (checkInformado != checkEsperado)
            return ValidationResult.Failure($"校验码错误，应为 '{checkEsperado}' (Dígito de controlo inválido).", "ISO 7064:1983.MOD 11-2");

        return ValidationResult.Success("GB 11643-1999 (ISO 7064:1983.MOD 11-2 中华人民共和国国家标准)");
    }
}

public sealed class CnUsccValidator : IIdDocumentValidator
{
    public string DocumentTypeId => "CN_USCC";

    private const string Base31 = "0123456789ABCDEFGHJKLMNPQRTUWXY";
    private static readonly int[] Weights = { 1, 3, 9, 27, 19, 26, 16, 17, 20, 29, 25, 13, 8, 24, 10, 30, 28 };

    public ValidationResult Validate(string? rawInput)
    {
        if (string.IsNullOrWhiteSpace(rawInput))
            return ValidationResult.Failure("统一社会信用代码为必填项 (O código de crédito social unificado é obrigatório).");

        var clean = new string(rawInput.Where(char.IsLetterOrDigit).ToArray()).ToUpperInvariant();
        if (clean.Length != 18)
            return ValidationResult.Failure("统一社会信用代码必须为18位 (O USCC deve conter 18 caracteres).");

        int soma = 0;
        for (int i = 0; i < 17; i++)
        {
            char c = clean[i];
            int val = Base31.IndexOf(c);
            if (val == -1)
                return ValidationResult.Failure($"统一社会信用代码包含非法字符 '{c}' (Caractere inválido no código).");
            soma += val * Weights[i];
        }

        int resto = soma % 31;
        int checkVal = resto == 0 ? 0 : 31 - resto;
        char checkEsperado = Base31[checkVal];

        if (clean[17] != checkEsperado)
            return ValidationResult.Failure($"统一社会信用代码校验码错误 (Dígito de verificação USCC inválido).", "GB 32100-2015");

        return ValidationResult.Success("GB 32100-2015 法人和其他组织统一社会信用代码编码规则");
    }
}
