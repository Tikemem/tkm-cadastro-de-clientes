using System.Globalization;

namespace CadastroClientes.InternationalDocumentManager.DocumentValidators;

public sealed class ArSaNationalIdValidator : IIdDocumentValidator
{
    public string DocumentTypeId => "AR_SA_NATIONAL_ID";

    public ValidationResult Validate(string? rawInput)
    {
        if (string.IsNullOrWhiteSpace(rawInput))
            return ValidationResult.Failure("رقم الهوية الوطنية أو الإقامة مطلوب (Número do documento é obrigatório).");

        var d = new string(rawInput.Where(char.IsDigit).ToArray());
        if (d.Length != 10)
            return ValidationResult.Failure("يجب أن يتكون رقم الهوية أو الإقامة من 10 أرقام (Deve conter exatamente 10 dígitos).");

        char primeiro = d[0];
        if (primeiro != '1' && primeiro != '2')
            return ValidationResult.Failure("يجب أن يبدأ رقم الهوية بالرقم 1 للمواطنين أو 2 للمقيمين (Deve iniciar com 1 para Cidadãos ou 2 para Iqama/Residentes).", "Saudi NIC Rules");

        // Algoritmo de Luhn oficial
        int soma = 0;
        for (int i = 0; i < 9; i++)
        {
            int val = d[i] - '0';
            if (i % 2 == 0) // posições ímpares (0, 2, 4, 6, 8) multiplicam por 2
            {
                int dobro = val * 2;
                soma += (dobro / 10) + (dobro % 10);
            }
            else
            {
                soma += val;
            }
        }

        int digitoEsperado = (10 - (soma % 10)) % 10;
        if (d[9] - '0' != digitoEsperado)
            return ValidationResult.Failure("رقم التحقق من الهوية غير صحيح (Dígito verificador da identidade saudita inválido).", "Luhn Mod 10 (Saudi NIC)");

        string tipo = primeiro == '1' ? "الهوية الوطنية السعودية (Saudi National ID)" : "هوية مقيم / الإقامة (Saudi Iqama)";
        return ValidationResult.Success($"خوارزمية لُوهن المعتمدة - {tipo}");
    }
}

public sealed class ArSaCrValidator : IIdDocumentValidator
{
    public string DocumentTypeId => "AR_SA_CR";

    public ValidationResult Validate(string? rawInput)
    {
        if (string.IsNullOrWhiteSpace(rawInput))
            return ValidationResult.Failure("رقم السجل التجاري مطلوب (O número do registro comercial é obrigatório).");

        var d = new string(rawInput.Where(char.IsDigit).ToArray());
        if (d.Length < 10 || d.Length > 15)
            return ValidationResult.Failure("يجب أن يتكون السجل التجاري من 10 إلى 15 رقماً (O CR deve conter entre 10 e 15 dígitos).");

        return ValidationResult.Success("وزارة التجارة بالمملكة العربية السعودية (Ministry of Commerce)", isMathematical: false);
    }
}

public sealed class ArAeEmiratesIdValidator : IIdDocumentValidator
{
    public string DocumentTypeId => "AR_AE_EMIRATES_ID";

    public ValidationResult Validate(string? rawInput)
    {
        if (string.IsNullOrWhiteSpace(rawInput))
            return ValidationResult.Failure("رقم الهوية الإماراتية مطلوب (O Emirates ID é obrigatório).");

        var d = new string(rawInput.Where(char.IsDigit).ToArray());
        if (d.Length != 15)
            return ValidationResult.Failure("يجب أن يتكون رقم الهوية الإماراتية من 15 رقماً (O Emirates ID deve conter 15 dígitos).");

        if (!d.StartsWith("784"))
            return ValidationResult.Failure("يجب أن تبدأ الهوية الإماراتية بالرمز 784 (Deve iniciar com 784).", "ICP UAE");

        string anoStr = d.Substring(3, 4);
        if (!int.TryParse(anoStr, out int ano) || ano < 1920 || ano > DateTime.Now.Year)
            return ValidationResult.Failure("سنة الميلاد في الهوية الإماراتية غير صحيحة (Ano de nascimento inválido no Emirates ID).");

        // Verificação Luhn no 15º dígito
        int soma = 0;
        for (int i = 0; i < 14; i++)
        {
            int val = d[i] - '0';
            if (i % 2 == 1) // alternado
            {
                int dobro = val * 2;
                soma += (dobro / 10) + (dobro % 10);
            }
            else
            {
                soma += val;
            }
        }

        int digitoEsperado = (10 - (soma % 10)) % 10;
        if (d[14] - '0' != digitoEsperado)
            return ValidationResult.Failure("رقم التحقق للهوية الإماراتية غير صحيح (Dígito verificador do Emirates ID inválido).", "Luhn Checksum (ICP UAE)");

        return ValidationResult.Success("الهيئة الاتحادية للهوية والجنسية والجمارك وأمن المنافذ (ICP UAE)");
    }
}

public sealed class ArEgNationalIdValidator : IIdDocumentValidator
{
    public string DocumentTypeId => "AR_EG_NATIONAL_ID";

    private static readonly HashSet<string> Governorates = new()
    {
        "01", "02", "03", "04", "11", "12", "13", "14", "15", "16", "17", "18", "19",
        "21", "22", "23", "24", "25", "26", "27", "28", "29", "31", "32", "33", "34", "35", "88"
    };

    public ValidationResult Validate(string? rawInput)
    {
        if (string.IsNullOrWhiteSpace(rawInput))
            return ValidationResult.Failure("الرقم القومي المصري مطلوب (O número de identidade egípcio é obrigatório).");

        var d = new string(rawInput.Where(char.IsDigit).ToArray());
        if (d.Length != 14)
            return ValidationResult.Failure("يجب أن يتكون الرقم القومي من 14 رقماً (Deve conter exatamente 14 dígitos).");

        char seculoChar = d[0];
        if (seculoChar != '2' && seculoChar != '3')
            return ValidationResult.Failure("رمز القرن غير صحيح (Deve iniciar com 2 para séc. XX ou 3 para séc. XXI).");

        int anoPrefixo = seculoChar == '2' ? 1900 : 2000;
        string dataStr = $"{anoPrefixo + int.Parse(d.Substring(1, 2))}{d.Substring(3, 4)}";

        if (!DateTime.TryParseExact(dataStr, "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out _))
            return ValidationResult.Failure("تاريخ الميلاد في الرقم القومي غير صحيح (Data de nascimento inválida no documento egípcio).");

        string gov = d.Substring(7, 2);
        if (!Governorates.Contains(gov))
            return ValidationResult.Failure("كود المحافظة غير صحيح (Código da província egípcia inválido).");

        return ValidationResult.Success("مصلحة الأحوال المدنية بجمهورية مصر العربية (Civil Status Organization)");
    }
}
