namespace CadastroClientes.InternationalDocumentManager.DocumentValidators;

public sealed class UsSsnValidator : IIdDocumentValidator
{
    public string DocumentTypeId => "US_SSN";

    public ValidationResult Validate(string? rawInput)
    {
        if (string.IsNullOrWhiteSpace(rawInput))
            return ValidationResult.Failure("SSN is required.");

        var d = new string(rawInput.Where(char.IsDigit).ToArray());
        if (d.Length != 9)
            return ValidationResult.Failure("SSN must contain exactly 9 digits.");

        if (d.Distinct().Count() == 1)
            return ValidationResult.Failure("Invalid SSN (all digits cannot be the same).");

        string area = d[..3];
        string group = d[3..5];
        string serial = d[5..];

        // Regra SSA: Area não pode ser 000, 666 ou no intervalo 900-999 (reservados ou ITIN)
        if (area == "000")
            return ValidationResult.Failure("Invalid SSN area number ('000' is not assigned).", "SSA Structural Rules");
        if (area == "666")
            return ValidationResult.Failure("Invalid SSN area number ('666' is never issued).", "SSA Structural Rules");
        if (int.Parse(area) >= 900)
            return ValidationResult.Failure("Invalid SSN area number (900-999 are reserved or ITINs).", "SSA Structural Rules");

        // Regra SSA: Group não pode ser 00
        if (group == "00")
            return ValidationResult.Failure("Invalid SSN group number ('00' is invalid).", "SSA Structural Rules");

        // Regra SSA: Serial não pode ser 0000
        if (serial == "0000")
            return ValidationResult.Failure("Invalid SSN serial number ('0000' is invalid).", "SSA Structural Rules");

        return ValidationResult.Success(
            "SSA Structural Validation Rules",
            isMathematical: false,
            warning: "Structural validation confirms the format, but does NOT prove identity ownership.");
    }
}

public sealed class UsEinValidator : IIdDocumentValidator
{
    public string DocumentTypeId => "US_EIN";

    private static readonly HashSet<string> ValidPrefixes = new()
    {
        "01", "02", "03", "04", "05", "06", "10", "11", "12", "13", "14", "15", "16",
        "20", "21", "22", "23", "24", "25", "26", "27", "30", "31", "32", "33", "34",
        "35", "36", "37", "38", "39", "40", "41", "42", "43", "44", "45", "46", "47",
        "48", "50", "51", "52", "53", "54", "55", "56", "57", "58", "59", "60", "61",
        "62", "63", "64", "65", "66", "67", "68", "71", "72", "73", "74", "75", "76",
        "77", "80", "81", "82", "83", "84", "85", "86", "87", "88", "90", "91", "92",
        "93", "94", "95", "98", "99"
    };

    public ValidationResult Validate(string? rawInput)
    {
        if (string.IsNullOrWhiteSpace(rawInput))
            return ValidationResult.Failure("EIN is required.");

        var d = new string(rawInput.Where(char.IsDigit).ToArray());
        if (d.Length != 9)
            return ValidationResult.Failure("EIN must contain exactly 9 digits.");

        string prefix = d[..2];
        if (!ValidPrefixes.Contains(prefix))
            return ValidationResult.Failure($"Invalid IRS Campus prefix '{prefix}' for EIN.", "IRS Prefix Rules");

        return ValidationResult.Success("IRS Employer Identification Number Prefix & Structural Verification");
    }
}
