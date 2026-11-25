using System.Text.RegularExpressions;

namespace CadastroClientes.InternationalDocumentManager.DocumentValidators;

public sealed class InAadhaarValidator : IIdDocumentValidator
{
    public string DocumentTypeId => "IN_AADHAAR";

    // ── Tabelas do Algoritmo Verhoeff Oficial (UIDAI) ──
    private static readonly int[,] D = {
        {0, 1, 2, 3, 4, 5, 6, 7, 8, 9},
        {1, 2, 3, 4, 0, 6, 7, 8, 9, 5},
        {2, 3, 4, 0, 1, 7, 8, 9, 5, 6},
        {3, 4, 0, 1, 2, 8, 9, 5, 6, 7},
        {4, 0, 1, 2, 3, 9, 5, 6, 7, 8},
        {5, 9, 8, 7, 6, 0, 4, 3, 2, 1},
        {6, 5, 9, 8, 7, 1, 0, 4, 3, 2},
        {7, 6, 5, 9, 8, 2, 1, 0, 4, 3},
        {8, 7, 6, 5, 9, 3, 2, 1, 0, 4},
        {9, 8, 7, 6, 5, 4, 3, 2, 1, 0}
    };

    private static readonly int[,] P = {
        {0, 1, 2, 3, 4, 5, 6, 7, 8, 9},
        {1, 5, 7, 6, 2, 8, 3, 0, 9, 4},
        {5, 8, 0, 3, 7, 9, 6, 1, 4, 2},
        {8, 9, 1, 6, 0, 4, 3, 5, 2, 7},
        {9, 4, 5, 3, 1, 2, 6, 8, 7, 0},
        {4, 2, 8, 6, 5, 7, 3, 9, 0, 1},
        {2, 7, 9, 3, 8, 0, 6, 4, 1, 5},
        {7, 0, 4, 6, 9, 1, 3, 2, 5, 8}
    };

    public ValidationResult Validate(string? rawInput)
    {
        if (string.IsNullOrWhiteSpace(rawInput))
            return ValidationResult.Success("Aadhaar não preenchido (opcional por lei)", isMathematical: false);

        var d = new string(rawInput.Where(char.IsDigit).ToArray());
        if (d.Length != 12)
            return ValidationResult.Failure("Aadhaar deve conter exatamente 12 dígitos.");

        // Regra UIDAI: Primeiro dígito não pode ser 0 ou 1
        if (d[0] == '0' || d[0] == '1')
            return ValidationResult.Failure("Aadhaar não pode começar com 0 ou 1.", "UIDAI Rules");

        if (d.Distinct().Count() == 1)
            return ValidationResult.Failure("Aadhaar inválido (todos os dígitos repetidos).", "Verhoeff Checksum");

        // Execução do algoritmo Verhoeff
        int c = 0;
        int len = d.Length;
        for (int i = 0; i < len; i++)
        {
            int digito = d[len - 1 - i] - '0';
            c = D[c, P[i % 8, digito]];
        }

        if (c != 0)
            return ValidationResult.Failure("Dígito de verificação Verhoeff do Aadhaar incorreto.", "Verhoeff Checksum (UIDAI)");

        return ValidationResult.Success(
            "Algoritmo Oficial Verhoeff (UIDAI - Government of India)",
            isMathematical: true,
            warning: "Aadhaar é um identificador sensível. O fornecimento é facultativo para entidades privadas conforme a legislação.");
    }
}

public sealed class InPanValidator : IIdDocumentValidator
{
    public string DocumentTypeId => "IN_PAN";

    private static readonly Regex PanRegex = new(@"^[A-Z]{5}[0-9]{4}[A-Z]$", RegexOptions.Compiled);
    private static readonly HashSet<char> StatusCodes = new("PCHABGJLFT");

    public ValidationResult Validate(string? rawInput)
    {
        if (string.IsNullOrWhiteSpace(rawInput))
            return ValidationResult.Failure("PAN is required.");

        var clean = new string(rawInput.Where(char.IsLetterOrDigit).ToArray()).ToUpperInvariant();
        if (clean.Length != 10)
            return ValidationResult.Failure("PAN must contain exactly 10 alphanumeric characters (AAAAA0000A).");

        if (!PanRegex.IsMatch(clean))
            return ValidationResult.Failure("Invalid PAN format. Must be 5 uppercase letters, 4 digits, and 1 uppercase letter.");

        char status = clean[3];
        if (!StatusCodes.Contains(status))
            return ValidationResult.Failure($"Invalid 4th character '{status}' in PAN (must represent entity type, e.g. 'P' for Individual).", "Income Tax Dept Rules");

        return ValidationResult.Success("Indian Income Tax Department PAN Structural Validation");
    }
}

public sealed class InGstinValidator : IIdDocumentValidator
{
    public string DocumentTypeId => "IN_GSTIN";

    private static readonly Regex GstinRegex = new(@"^[0-9]{2}[A-Z]{5}[0-9]{4}[A-Z][1-9A-Z]Z[0-9A-Z]$", RegexOptions.Compiled);

    public ValidationResult Validate(string? rawInput)
    {
        if (string.IsNullOrWhiteSpace(rawInput))
            return ValidationResult.Failure("GSTIN is required.");

        var clean = new string(rawInput.Where(char.IsLetterOrDigit).ToArray()).ToUpperInvariant();
        if (clean.Length != 15)
            return ValidationResult.Failure("GSTIN must contain exactly 15 alphanumeric characters.");

        if (!GstinRegex.IsMatch(clean))
            return ValidationResult.Failure("Invalid GSTIN structure (2-digit state code + 10-character PAN + entity number + 'Z' + checksum).", "GST Council");

        int stateCode = int.Parse(clean[..2]);
        if (stateCode < 1 || (stateCode > 38 && stateCode != 97 && stateCode != 99))
            return ValidationResult.Failure($"Invalid state code '{stateCode:D2}' in GSTIN.", "GST State Codes");

        return ValidationResult.Success("Goods and Services Tax Network (GSTN) Official Structure");
    }
}
