namespace CadastroClientes.InternationalDocumentManager.DocumentValidators;

public static class DocumentValidatorRegistry
{
    private static readonly Dictionary<string, IIdDocumentValidator> _validators = new(StringComparer.OrdinalIgnoreCase);

    static DocumentValidatorRegistry()
    {
        // Brasil
        Registrar(new BrCpfValidator());
        Registrar(new BrCnpjValidator());
        Registrar(new BrRgValidator());

        // Portugal
        Registrar(new PtNifValidator());
        Registrar(new PtNipcValidator());
        Registrar(new PtCcValidator());

        // Estados Unidos
        Registrar(new UsSsnValidator());
        Registrar(new UsEinValidator());

        // China
        Registrar(new CnResidentIdValidator());
        Registrar(new CnUsccValidator());

        // Índia
        Registrar(new InAadhaarValidator());
        Registrar(new InPanValidator());
        Registrar(new InGstinValidator());

        // Espanha
        Registrar(new EsDniValidator());
        Registrar(new EsNieValidator());
        Registrar(new EsCifValidator());

        // Países Árabes
        Registrar(new ArSaNationalIdValidator());
        Registrar(new ArSaCrValidator());
        Registrar(new ArAeEmiratesIdValidator());
        Registrar(new ArEgNationalIdValidator());

        // Passaporte Universal
        Registrar(new PassportValidator());
    }

    public static void Registrar(IIdDocumentValidator validator)
    {
        _validators[validator.DocumentTypeId] = validator;
    }

    public static ValidationResult Validar(string? rawInput, string documentTypeId)
    {
        if (string.IsNullOrWhiteSpace(documentTypeId))
            return ValidationResult.Failure("Tipo de documento não informado.");

        if (_validators.TryGetValue(documentTypeId, out var validator))
        {
            return validator.Validate(rawInput);
        }

        // Se for passaporte
        if (documentTypeId.Contains("PASSPORT", StringComparison.OrdinalIgnoreCase))
        {
            return _validators["PASSPORT"].Validate(rawInput);
        }

        // Validador padrão básico se for um novo documento sem validador customizado registrado
        if (string.IsNullOrWhiteSpace(rawInput))
            return ValidationResult.Failure("Documento obrigatório.");

        return ValidationResult.Success("Validação Genérica de Formato");
    }
}
