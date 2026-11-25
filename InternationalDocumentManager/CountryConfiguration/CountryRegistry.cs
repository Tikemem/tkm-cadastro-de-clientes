using CadastroClientes.InternationalDocumentManager.DocumentTypes;
using CadastroClientes.Services;

namespace CadastroClientes.InternationalDocumentManager.CountryConfiguration;

public static class CountryRegistry
{
    // Documentos do Brasil
    public static readonly DocumentTypeInfo BrCpf = new()
    {
        Id = "BR_CPF",
        CountryCode = "BR",
        Category = DocumentCategory.PrimaryPersonal,
        CodeName = "CPF",
        OfficialName = "Cadastro de Pessoas Físicas",
        Mask = "000.000.000-00",
        Placeholder = "123.456.789-01",
        AllowedChars = "0123456789",
        RawLength = 11,
        IsLengthVariable = false,
        MinRawLength = 11,
        MaxRawLength = 11,
        IsSensitive = false,
        IsOptional = false,
        Description = "Documento oficial de identificação fiscal do cidadão brasileiro."
    };

    public static readonly DocumentTypeInfo BrRg = new()
    {
        Id = "BR_RG",
        CountryCode = "BR",
        Category = DocumentCategory.SecondaryPersonal,
        CodeName = "RG",
        OfficialName = "Registro Geral / Carteira de Identidade",
        Mask = "00.000.000-A",
        Placeholder = "12.345.678-9",
        AllowedChars = "0123456789Xx",
        RawLength = 9,
        IsLengthVariable = true,
        MinRawLength = 7,
        MaxRawLength = 10,
        IsSensitive = false,
        IsOptional = true,
        Description = "Documento de identidade civil estadual emitido pela SSP."
    };

    public static readonly DocumentTypeInfo BrCnpj = new()
    {
        Id = "BR_CNPJ",
        CountryCode = "BR",
        Category = DocumentCategory.Business,
        CodeName = "CNPJ",
        OfficialName = "Cadastro Nacional da Pessoa Jurídica",
        Mask = "00.000.000/0000-00",
        Placeholder = "12.345.678/0001-90",
        AllowedChars = "0123456789",
        RawLength = 14,
        IsLengthVariable = false,
        MinRawLength = 14,
        MaxRawLength = 14,
        IsSensitive = false,
        IsOptional = false,
        Description = "Identificador único de empresas e entidades no Brasil."
    };

    public static readonly DocumentTypeInfo GenericPassport = new()
    {
        Id = "PASSPORT",
        CountryCode = "GLOBAL",
        Category = DocumentCategory.Passport,
        CodeName = "Passaporte",
        OfficialName = "Passaporte Internacional / International Passport",
        Mask = "AAAAAAAAAAAA",
        Placeholder = "YA1234567",
        AllowedChars = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz",
        RawLength = 9,
        IsLengthVariable = true,
        MinRawLength = 6,
        MaxRawLength = 12,
        IsSensitive = false,
        IsOptional = true,
        Description = "Documento de viagem internacional emitido pelo governo."
    };

    // Documentos de Portugal
    public static readonly DocumentTypeInfo PtNif = new()
    {
        Id = "PT_NIF",
        CountryCode = "PT",
        Category = DocumentCategory.PrimaryPersonal,
        CodeName = "NIF",
        OfficialName = "Número de Identificação Fiscal (Número de Contribuinte)",
        Mask = "000 000 000",
        Placeholder = "123 456 789",
        AllowedChars = "0123456789",
        RawLength = 9,
        IsLengthVariable = false,
        MinRawLength = 9,
        MaxRawLength = 9,
        IsSensitive = false,
        IsOptional = false,
        Description = "Número emitido pela Autoridade Tributária e Aduaneira de Portugal."
    };

    public static readonly DocumentTypeInfo PtCc = new()
    {
        Id = "PT_CC",
        CountryCode = "PT",
        Category = DocumentCategory.SecondaryPersonal,
        CodeName = "Cartão de Cidadão",
        OfficialName = "Número de Identificação Civil (Cartão de Cidadão)",
        Mask = "00000000 0 AA0",
        Placeholder = "12345678 9 ZZ4",
        AllowedChars = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz",
        RawLength = 12,
        IsLengthVariable = false,
        MinRawLength = 12,
        MaxRawLength = 12,
        IsSensitive = false,
        IsOptional = true,
        Description = "Documento de identificação física e digital do cidadão português."
    };

    public static readonly DocumentTypeInfo PtNipc = new()
    {
        Id = "PT_NIPC",
        CountryCode = "PT",
        Category = DocumentCategory.Business,
        CodeName = "NIPC",
        OfficialName = "Número de Identificação de Pessoa Coletiva",
        Mask = "000 000 000",
        Placeholder = "501 234 567",
        AllowedChars = "0123456789",
        RawLength = 9,
        IsLengthVariable = false,
        MinRawLength = 9,
        MaxRawLength = 9,
        IsSensitive = false,
        IsOptional = false,
        Description = "Número de identificação de empresas e entidades coletivas em Portugal."
    };

    // Documentos dos Estados Unidos
    public static readonly DocumentTypeInfo UsSsn = new()
    {
        Id = "US_SSN",
        CountryCode = "US",
        Category = DocumentCategory.PrimaryPersonal,
        CodeName = "SSN",
        OfficialName = "Social Security Number",
        Mask = "000-00-0000",
        Placeholder = "123-45-6789",
        AllowedChars = "0123456789",
        RawLength = 9,
        IsLengthVariable = false,
        MinRawLength = 9,
        MaxRawLength = 9,
        IsSensitive = true,
        IsOptional = false,
        SensitivityNote = "Sensível: Dado pessoal protegido sob o Privacy Act. Não presumir titularidade sem verificação formal.",
        Description = "United States Social Security Number issued by the SSA."
    };

    public static readonly DocumentTypeInfo UsEin = new()
    {
        Id = "US_EIN",
        CountryCode = "US",
        Category = DocumentCategory.Business,
        CodeName = "EIN",
        OfficialName = "Employer Identification Number (Federal Tax ID)",
        Mask = "00-0000000",
        Placeholder = "12-3456789",
        AllowedChars = "0123456789",
        RawLength = 9,
        IsLengthVariable = false,
        MinRawLength = 9,
        MaxRawLength = 9,
        IsSensitive = false,
        IsOptional = false,
        Description = "Federal tax identification number for businesses in the US."
    };

    // Documentos da China
    public static readonly DocumentTypeInfo CnResidentId = new()
    {
        Id = "CN_RESIDENT_ID",
        CountryCode = "CN",
        Category = DocumentCategory.PrimaryPersonal,
        CodeName = "身份证号码",
        OfficialName = "居民身份证号码 (Resident Identity Card Number)",
        Mask = "000000 00000000 000A",
        Placeholder = "110101 19900307 234X",
        AllowedChars = "0123456789Xx",
        RawLength = 18,
        IsLengthVariable = false,
        MinRawLength = 18,
        MaxRawLength = 18,
        IsSensitive = false,
        IsOptional = false,
        Description = "中国大陆公民18位居民身份证号码 (GB 11643-1999)."
    };

    public static readonly DocumentTypeInfo CnUscc = new()
    {
        Id = "CN_USCC",
        CountryCode = "CN",
        Category = DocumentCategory.Business,
        CodeName = "统一社会信用代码",
        OfficialName = "统一社会信用代码 (Unified Social Credit Code)",
        Mask = "AAAAAAAAAAAAAAAAAA",
        Placeholder = "91110000000000000Y",
        AllowedChars = "0123456789ABCDEFGHJKLMNPQRTUWXYabcdefghjklmnpqrtuwxy",
        RawLength = 18,
        IsLengthVariable = false,
        MinRawLength = 18,
        MaxRawLength = 18,
        IsSensitive = false,
        IsOptional = false,
        Description = "中国企业及组织法人机构18位社会信用代码."
    };

    // Documentos da Índia
    public static readonly DocumentTypeInfo InAadhaar = new()
    {
        Id = "IN_AADHAAR",
        CountryCode = "IN",
        Category = DocumentCategory.PrimaryPersonal,
        CodeName = "Aadhaar",
        OfficialName = "आधार संख्या (Aadhaar Number)",
        Mask = "0000 0000 0000",
        Placeholder = "2345 6789 0123",
        AllowedChars = "0123456789",
        RawLength = 12,
        IsLengthVariable = false,
        MinRawLength = 12,
        MaxRawLength = 12,
        IsSensitive = true,
        IsOptional = true,
        SensitivityNote = "Dado sensível conforme lei indiana. O preenchimento NÃO é obrigatório para cadastros privados. Utilize o PAN ou Passaporte como alternativa.",
        Description = "12-digit unique identity number issued by UIDAI with Verhoeff checksum."
    };

    public static readonly DocumentTypeInfo InPan = new()
    {
        Id = "IN_PAN",
        CountryCode = "IN",
        Category = DocumentCategory.SecondaryPersonal,
        CodeName = "PAN",
        OfficialName = "स्थायी खाता संख्या (Permanent Account Number)",
        Mask = "AAAAA 0000 A",
        Placeholder = "ABCDE 1234 F",
        AllowedChars = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz",
        RawLength = 10,
        IsLengthVariable = false,
        MinRawLength = 10,
        MaxRawLength = 10,
        IsSensitive = false,
        IsOptional = false,
        Description = "10-character alphanumeric tax identifier issued by the Income Tax Department."
    };

    public static readonly DocumentTypeInfo InGstin = new()
    {
        Id = "IN_GSTIN",
        CountryCode = "IN",
        Category = DocumentCategory.Business,
        CodeName = "GSTIN",
        OfficialName = "माल एवं सेवा कर पहचान संख्या (GSTIN)",
        Mask = "00 AAAAA 0000 A 0 Z A",
        Placeholder = "22 AAAAA 0000 A 1 Z 5",
        AllowedChars = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz",
        RawLength = 15,
        IsLengthVariable = false,
        MinRawLength = 15,
        MaxRawLength = 15,
        IsSensitive = false,
        IsOptional = false,
        Description = "15-character Goods and Services Tax Identification Number for businesses in India."
    };

    // Documentos da Espanha
    public static readonly DocumentTypeInfo EsDni = new()
    {
        Id = "ES_DNI",
        CountryCode = "ES",
        Category = DocumentCategory.PrimaryPersonal,
        CodeName = "DNI",
        OfficialName = "Documento Nacional de Identidad / NIF Persona Física",
        Mask = "00000000-A",
        Placeholder = "12345678-Z",
        AllowedChars = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz",
        RawLength = 9,
        IsLengthVariable = false,
        MinRawLength = 9,
        MaxRawLength = 9,
        IsSensitive = false,
        IsOptional = false,
        Description = "Documento oficial de identidad para ciudadanos españoles."
    };

    public static readonly DocumentTypeInfo EsNie = new()
    {
        Id = "ES_NIE",
        CountryCode = "ES",
        Category = DocumentCategory.SecondaryPersonal,
        CodeName = "NIE",
        OfficialName = "Número de Identidad de Extranjero",
        Mask = "A-0000000-A",
        Placeholder = "X-1234567-L",
        AllowedChars = "0123456789XYZxyzABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz",
        RawLength = 9,
        IsLengthVariable = false,
        MinRawLength = 9,
        MaxRawLength = 9,
        IsSensitive = false,
        IsOptional = true,
        Description = "Identificador para residentes extranjeros en España."
    };

    public static readonly DocumentTypeInfo EsCif = new()
    {
        Id = "ES_CIF",
        CountryCode = "ES",
        Category = DocumentCategory.Business,
        CodeName = "CIF / NIF Entidad",
        OfficialName = "Código de Identificación Fiscal / NIF Empresarial",
        Mask = "A-0000000-A",
        Placeholder = "B-1234567-8",
        AllowedChars = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz",
        RawLength = 9,
        IsLengthVariable = false,
        MinRawLength = 9,
        MaxRawLength = 9,
        IsSensitive = false,
        IsOptional = false,
        Description = "Identificación fiscal de empresas y entidades jurídicas en España."
    };

    // Documentos dos Países Árabes
    public static readonly DocumentTypeInfo ArSaNationalId = new()
    {
        Id = "AR_SA_NATIONAL_ID",
        CountryCode = "AR_SA",
        Category = DocumentCategory.PrimaryPersonal,
        CodeName = "الهوية الوطنية / الإقامة",
        OfficialName = "رقم الهوية الوطنية / الإقامة (Saudi National ID / Iqama)",
        Mask = "0000000000",
        Placeholder = "1012345678",
        AllowedChars = "0123456789",
        RawLength = 10,
        IsLengthVariable = false,
        MinRawLength = 10,
        MaxRawLength = 10,
        IsSensitive = false,
        IsOptional = false,
        Description = "رقم الهوية الوطنية للمواطن أو الإقامة للمقيم (10 أرقام مع خوارزمية لُوهن)."
    };

    public static readonly DocumentTypeInfo ArSaCr = new()
    {
        Id = "AR_SA_CR",
        CountryCode = "AR_SA",
        Category = DocumentCategory.Business,
        CodeName = "السجل التجاري",
        OfficialName = "السجل التجاري / الرقم الضريبي (Commercial Registration / Tax ID)",
        Mask = "0000000000",
        Placeholder = "1010123456",
        AllowedChars = "0123456789",
        RawLength = 10,
        IsLengthVariable = true,
        MinRawLength = 10,
        MaxRawLength = 15,
        IsSensitive = false,
        IsOptional = false,
        Description = "رقم السجل التجاري للشركات بالمملكة العربية السعودية."
    };

    public static readonly DocumentTypeInfo ArAeEmiratesId = new()
    {
        Id = "AR_AE_EMIRATES_ID",
        CountryCode = "AR_AE",
        Category = DocumentCategory.PrimaryPersonal,
        CodeName = "بطاقة الهوية الإماراتية",
        OfficialName = "بطاقة الهوية الإماراتية (Emirates ID)",
        Mask = "000-0000-0000000-0",
        Placeholder = "784-1990-1234567-1",
        AllowedChars = "0123456789",
        RawLength = 15,
        IsLengthVariable = false,
        MinRawLength = 15,
        MaxRawLength = 15,
        IsSensitive = false,
        IsOptional = false,
        Description = "الهوية الوطنية الصادرة من الهيئة الاتحادية للهوية والجنسية بدولة الإمارات."
    };

    public static readonly DocumentTypeInfo ArEgNationalId = new()
    {
        Id = "AR_EG_NATIONAL_ID",
        CountryCode = "AR_EG",
        Category = DocumentCategory.PrimaryPersonal,
        CodeName = "الرقم القومي المصري",
        OfficialName = "بطاقة الرقم القومي المصري (Egyptian National ID)",
        Mask = "00000000000000",
        Placeholder = "29003070101234",
        AllowedChars = "0123456789",
        RawLength = 14,
        IsLengthVariable = false,
        MinRawLength = 14,
        MaxRawLength = 14,
        IsSensitive = false,
        IsOptional = false,
        Description = "الرقم القومي المكون من 14 رقماً موضحاً به القرن وتاريخ الميلاد والمحافظة."
    };

    // Definições dos Países
    public static readonly CountryInfo Brasil = new()
    {
        Code = "BR",
        GroupCode = "BR",
        FlagEmoji = "🇧🇷",
        DisplayName = "Brasil",
        NativeName = "Brasil",
        AssociatedLanguage = Idioma.PtBr,
        PhoneDialCode = "+55",
        PhonePlaceholder = "+55 (11) 98765-4321",
        PostalCodeName = "CEP",
        PostalCodeMask = "00000-000",
        PostalCodePlaceholder = "01310-100",
        StateOrProvinceLabel = "UF / Estado",
        DefaultDocumentTypeId = "BR_CPF",
        SupportedDocuments = new[] { BrCpf, BrRg, BrCnpj, GenericPassport }
    };

    public static readonly CountryInfo Portugal = new()
    {
        Code = "PT",
        GroupCode = "PT",
        FlagEmoji = "🇵🇹",
        DisplayName = "Portugal",
        NativeName = "Portugal",
        AssociatedLanguage = Idioma.PtPt,
        PhoneDialCode = "+351",
        PhonePlaceholder = "+351 912 345 678",
        PostalCodeName = "Código Postal",
        PostalCodeMask = "0000-000",
        PostalCodePlaceholder = "1000-001",
        StateOrProvinceLabel = "Distrito / Região",
        DefaultDocumentTypeId = "PT_NIF",
        SupportedDocuments = new[] { PtNif, PtCc, PtNipc, GenericPassport }
    };

    public static readonly CountryInfo EstadosUnidos = new()
    {
        Code = "US",
        GroupCode = "US",
        FlagEmoji = "🇺🇸",
        DisplayName = "Estados Unidos",
        NativeName = "United States",
        AssociatedLanguage = Idioma.En,
        PhoneDialCode = "+1",
        PhonePlaceholder = "+1 (555) 123-4567",
        PostalCodeName = "ZIP Code",
        PostalCodeMask = "00000-0000",
        PostalCodePlaceholder = "90210 or 10001-1234",
        StateOrProvinceLabel = "State",
        DefaultDocumentTypeId = "US_SSN",
        SupportedDocuments = new[] { UsSsn, UsEin, GenericPassport }
    };

    public static readonly CountryInfo China = new()
    {
        Code = "CN",
        GroupCode = "CN",
        FlagEmoji = "🇨🇳",
        DisplayName = "China",
        NativeName = "中国 (China)",
        AssociatedLanguage = Idioma.ZhCn,
        PhoneDialCode = "+86",
        PhonePlaceholder = "+86 138 0013 8000",
        PostalCodeName = "邮政编码 (Postal Code)",
        PostalCodeMask = "000000",
        PostalCodePlaceholder = "100000",
        StateOrProvinceLabel = "省份 / 直辖市 (Province)",
        DefaultDocumentTypeId = "CN_RESIDENT_ID",
        SupportedDocuments = new[] { CnResidentId, CnUscc, GenericPassport }
    };

    public static readonly CountryInfo India = new()
    {
        Code = "IN",
        GroupCode = "IN",
        FlagEmoji = "🇮🇳",
        DisplayName = "Índia",
        NativeName = "भारत (India)",
        AssociatedLanguage = Idioma.Hi,
        PhoneDialCode = "+91",
        PhonePlaceholder = "+91 98765 43210",
        PostalCodeName = "PIN Code",
        PostalCodeMask = "000 000",
        PostalCodePlaceholder = "110 001",
        StateOrProvinceLabel = "State / Union Territory",
        DefaultDocumentTypeId = "IN_PAN", // Aadhaar é sensível, sugerir PAN como default prático ou permitir Aadhaar
        SupportedDocuments = new[] { InPan, InAadhaar, InGstin, GenericPassport }
    };

    public static readonly CountryInfo Espanha = new()
    {
        Code = "ES",
        GroupCode = "ES",
        FlagEmoji = "🇪🇸",
        DisplayName = "Espanha",
        NativeName = "España",
        AssociatedLanguage = Idioma.Es,
        PhoneDialCode = "+34",
        PhonePlaceholder = "+34 612 345 678",
        PostalCodeName = "Código Postal",
        PostalCodeMask = "00000",
        PostalCodePlaceholder = "28001",
        StateOrProvinceLabel = "Provincia / Comunidad",
        DefaultDocumentTypeId = "ES_DNI",
        SupportedDocuments = new[] { EsDni, EsNie, EsCif, GenericPassport }
    };

    // Sub-países de Língua Árabe
    public static readonly CountryInfo ArabiaSaudita = new()
    {
        Code = "AR_SA",
        GroupCode = "AR",
        FlagEmoji = "🇸🇦",
        DisplayName = "Arábia Saudita",
        NativeName = "المملكة العربية السعودية (Saudi Arabia)",
        AssociatedLanguage = Idioma.Ar,
        PhoneDialCode = "+966",
        PhonePlaceholder = "+966 50 123 4567",
        PostalCodeName = "الرمز البريدي (Postal Code)",
        PostalCodeMask = "00000",
        PostalCodePlaceholder = "11564",
        StateOrProvinceLabel = "المنطقة / المحافظة (Region)",
        DefaultDocumentTypeId = "AR_SA_NATIONAL_ID",
        SupportedDocuments = new[] { ArSaNationalId, ArSaCr, GenericPassport },
        IsArabicSubCountry = true
    };

    public static readonly CountryInfo EmiradosArabes = new()
    {
        Code = "AR_AE",
        GroupCode = "AR",
        FlagEmoji = "🇦🇪",
        DisplayName = "Emirados Árabes Unidos",
        NativeName = "الإمارات العربية المتحدة (UAE)",
        AssociatedLanguage = Idioma.Ar,
        PhoneDialCode = "+971",
        PhonePlaceholder = "+971 50 123 4567",
        PostalCodeName = "صندوق بريد / الرمز (P.O. Box)",
        PostalCodeMask = "00000",
        PostalCodePlaceholder = "12345",
        StateOrProvinceLabel = "الإمارة (Emirate)",
        DefaultDocumentTypeId = "AR_AE_EMIRATES_ID",
        SupportedDocuments = new[] { ArAeEmiratesId, GenericPassport },
        IsArabicSubCountry = true
    };

    public static readonly CountryInfo Egito = new()
    {
        Code = "AR_EG",
        GroupCode = "AR",
        FlagEmoji = "🇪🇬",
        DisplayName = "Egito",
        NativeName = "مصر (Egypt)",
        AssociatedLanguage = Idioma.Ar,
        PhoneDialCode = "+20",
        PhonePlaceholder = "+20 10 1234 5678",
        PostalCodeName = "الرمز البريدي (Postal Code)",
        PostalCodeMask = "00000",
        PostalCodePlaceholder = "11511",
        StateOrProvinceLabel = "المحافظة (Governorate)",
        DefaultDocumentTypeId = "AR_EG_NATIONAL_ID",
        SupportedDocuments = new[] { ArEgNationalId, GenericPassport },
        IsArabicSubCountry = true
    };

    private static readonly Dictionary<string, CountryInfo> _paises = new(StringComparer.OrdinalIgnoreCase)
    {
        { Brasil.Code, Brasil },
        { Portugal.Code, Portugal },
        { EstadosUnidos.Code, EstadosUnidos },
        { China.Code, China },
        { India.Code, India },
        { Espanha.Code, Espanha },
        { ArabiaSaudita.Code, ArabiaSaudita },
        { EmiradosArabes.Code, EmiradosArabes },
        { Egito.Code, Egito }
    };

    public static IReadOnlyList<CountryInfo> TodosPaises => _paises.Values.ToList();

    public static IReadOnlyList<CountryInfo> PaisesPrincipais => new[]
    {
        Brasil,
        Portugal,
        EstadosUnidos,
        China,
        India,
        Espanha,
        ArabiaSaudita // Representante padrão árabe com seletor específico
    };

    public static IReadOnlyList<CountryInfo> PaisesArabes => new[]
    {
        ArabiaSaudita,
        EmiradosArabes,
        Egito
    };

    public static CountryInfo ObterPorCodigo(string? codigo)
    {
        if (string.IsNullOrWhiteSpace(codigo)) return Brasil;
        if (_paises.TryGetValue(codigo.Trim(), out var p)) return p;
        if (codigo.Equals("AR", StringComparison.OrdinalIgnoreCase)) return ArabiaSaudita;
        return Brasil;
    }

    public static DocumentTypeInfo? ObterDocumentoPorId(string? docId)
    {
        if (string.IsNullOrWhiteSpace(docId)) return null;
        foreach (var c in _paises.Values)
        {
            var d = c.SupportedDocuments.FirstOrDefault(x => x.Id.Equals(docId, StringComparison.OrdinalIgnoreCase));
            if (d is not null) return d;
        }
        if (docId.Equals("PASSPORT", StringComparison.OrdinalIgnoreCase)) return GenericPassport;
        return null;
    }
}
