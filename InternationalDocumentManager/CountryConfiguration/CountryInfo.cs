using CadastroClientes.InternationalDocumentManager.DocumentTypes;
using CadastroClientes.Services;

namespace CadastroClientes.InternationalDocumentManager.CountryConfiguration;

public sealed record CountryInfo
{
    public required string Code { get; init; }
    public required string GroupCode { get; init; }
    public required string FlagEmoji { get; init; }
    public required string DisplayName { get; init; }
    public required string NativeName { get; init; }
    public required Idioma AssociatedLanguage { get; init; }
    public required string PhoneDialCode { get; init; }
    public required string PhonePlaceholder { get; init; }
    public required string PostalCodeName { get; init; }
    public required string PostalCodeMask { get; init; }
    public required string PostalCodePlaceholder { get; init; }
    public required string StateOrProvinceLabel { get; init; }
    public required string DefaultDocumentTypeId { get; init; }
    public required IReadOnlyList<DocumentTypeInfo> SupportedDocuments { get; init; }
    public bool IsArabicSubCountry { get; init; }
}
