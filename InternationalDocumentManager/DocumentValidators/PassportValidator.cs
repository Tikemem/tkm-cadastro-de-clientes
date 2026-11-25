namespace CadastroClientes.InternationalDocumentManager.DocumentValidators;

public sealed class PassportValidator : IIdDocumentValidator
{
    public string DocumentTypeId => "PASSPORT";

    public ValidationResult Validate(string? rawInput)
    {
        if (string.IsNullOrWhiteSpace(rawInput))
            return ValidationResult.Success("Passaporte não informado (opcional)", isMathematical: false);

        var clean = new string(rawInput.Where(char.IsLetterOrDigit).ToArray()).ToUpperInvariant();
        if (clean.Length < 6 || clean.Length > 12)
            return ValidationResult.Failure("O Passaporte Internacional deve conter entre 6 e 12 caracteres alfanuméricos.");

        if (clean.Distinct().Count() == 1)
            return ValidationResult.Failure("Número de passaporte inválido (todos os caracteres iguais).");

        return ValidationResult.Success("Padrão ICAO Doc 9303 (Passaporte Internacional)", isMathematical: false);
    }
}
