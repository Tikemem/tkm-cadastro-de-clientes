namespace CadastroClientes.InternationalDocumentManager.DocumentValidators;

public sealed record ValidationResult
{
    public required bool IsValid { get; init; }
    public string? ErrorMessage { get; init; }
    public string AlgorithmName { get; init; } = "";
    public bool IsMathematicalOnly { get; init; } = true;
    public string? WarningNotice { get; init; }

    public static ValidationResult Success(string algorithm, bool isMathematical = true, string? warning = null) =>
        new()
        {
            IsValid = true,
            AlgorithmName = algorithm,
            IsMathematicalOnly = isMathematical,
            WarningNotice = warning
        };

    public static ValidationResult Failure(string error, string algorithm = "") =>
        new()
        {
            IsValid = false,
            ErrorMessage = error,
            AlgorithmName = algorithm
        };
}

public interface IIdDocumentValidator
{
    string DocumentTypeId { get; }
    ValidationResult Validate(string? rawInput);
}
