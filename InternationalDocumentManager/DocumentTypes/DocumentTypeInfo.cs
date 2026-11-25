namespace CadastroClientes.InternationalDocumentManager.DocumentTypes;

public enum DocumentCategory
{
    PrimaryPersonal,   // Documento principal de pessoa física (CPF, NIF, SSN, 身份证, Aadhaar, DNI, National ID)
    SecondaryPersonal, // Documento secundário / alternativo (RG, Cartão de Cidadão, PAN, NIE)
    Business,          // Documento de pessoa jurídica / empresa (CNPJ, NIPC, EIN, USCC, GSTIN, CIF)
    Passport           // Passaporte universal para estrangeiros
}

public sealed record DocumentTypeInfo
{
    public required string Id { get; init; }
    public required string CountryCode { get; init; }
    public required DocumentCategory Category { get; init; }
    public required string CodeName { get; init; }
    public required string OfficialName { get; init; }
    public required string Mask { get; init; }
    public required string Placeholder { get; init; }
    public required string AllowedChars { get; init; }
    public required int RawLength { get; init; }
    public bool IsLengthVariable { get; init; }
    public int MinRawLength { get; init; }
    public int MaxRawLength { get; init; }
    public bool IsSensitive { get; init; }
    public bool IsOptional { get; init; }
    public string? SensitivityNote { get; init; }
    public string? Description { get; init; }
}
