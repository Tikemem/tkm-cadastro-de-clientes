using CadastroClientes.InternationalDocumentManager.DocumentTypes;
using CadastroClientes.Services;

namespace CadastroClientes.InternationalDocumentManager.Internationalization;

public static class InternationalDocumentStrings
{
    // ══════════════════════════════════════════════════════════════
    //  RÓTULOS DO FORMULÁRIO
    // ══════════════════════════════════════════════════════════════

    public static string GetCountryLabel(Idioma idioma) => idioma switch
    {
        Idioma.PtPt => "País de residência *",
        Idioma.En => "Country of Residence *",
        Idioma.ZhCn => "居住国家/地区 *",
        Idioma.Hi => "निवास का देश *",
        Idioma.Es => "País de residencia *",
        Idioma.Ar => "بلد الإقامة *",
        _ => "País de residência *"
    };

    public static string GetArabicSubCountryLabel(Idioma idioma) => idioma switch
    {
        Idioma.PtPt => "País específico (Árabe) *",
        Idioma.En => "Specific Country (Arabic) *",
        Idioma.ZhCn => "具体阿拉伯国家/地区 *",
        Idioma.Hi => "विशिष्ट अरब देश *",
        Idioma.Es => "País específico (Árabe) *",
        Idioma.Ar => "الدولة المحددة (عربي) *",
        _ => "País específico (Árabe) *"
    };

    public static string GetDocumentTypeLabel(Idioma idioma) => idioma switch
    {
        Idioma.PtPt => "Tipo de documento *",
        Idioma.En => "Document Type *",
        Idioma.ZhCn => "证件类型 *",
        Idioma.Hi => "दस्तावेज़ का प्रकार *",
        Idioma.Es => "Tipo de documento *",
        Idioma.Ar => "نوع الوثيقة *",
        _ => "Tipo de documento *"
    };

    public static string GetNationalityLabel(Idioma idioma) => idioma switch
    {
        Idioma.PtPt => "Nacionalidade",
        Idioma.En => "Nationality",
        Idioma.ZhCn => "国籍",
        Idioma.Hi => "राष्ट्रीयता",
        Idioma.Es => "Nacionalidad",
        Idioma.Ar => "الجنسية",
        _ => "Nacionalidade"
    };

    public static string GetMathDisclaimer(Idioma idioma) => idioma switch
    {
        Idioma.PtPt => "A validação matemática confirma o formato e dígito de controlo, mas não substitui a comprovação oficial de identidade.",
        Idioma.En => "Mathematical validation confirms formatting only, and does not constitute proof of identity ownership.",
        Idioma.ZhCn => "算法校验仅确认编码格式合规，不构成身份归属证明。",
        Idioma.Hi => "गणितीय सत्यापन केवल प्रारूप की पुष्टि करता है, पहचान के प्रमाण का दावा नहीं करता।",
        Idioma.Es => "La validación matemática confirma el formato, pero no constituye acreditación legal de titularidad.",
        Idioma.Ar => "التحقق الحسابي يؤكد صحة الهيكل والترميز فقط، ولا يعد إثباتاً قانونياً للملكية.",
        _ => "A validação matemática confirma o formato e dígito verificador, mas não substitui a comprovação de titularidade da identidade."
    };

    public static string GetDuplicateDocError(Idioma idioma) => idioma switch
    {
        Idioma.PtPt => "Já existe um cliente registado com este documento neste país.",
        Idioma.En => "A client with this document already exists in this country.",
        Idioma.ZhCn => "该国家/地区已存在具有相同证件号码的客户。",
        Idioma.Hi => "इस देश में इस दस्तावेज़ संख्या के साथ एक ग्राहक पहले से मौजूद है।",
        Idioma.Es => "Ya existe un cliente registrado con este documento en este país.",
        Idioma.Ar => "يوجد عميل مسجل بالفعل برقم الوثيقة هذا في هذا البلد.",
        _ => "Já existe um cliente cadastrado com este documento neste país."
    };

    // ══════════════════════════════════════════════════════════════
    //  NOMES DOS PAÍSES TRADUZIDOS NO IDIOMA DO PROGRAMA
    // ══════════════════════════════════════════════════════════════

    public static string GetCountryName(string countryCode, Idioma idioma) => countryCode switch
    {
        "BR" => idioma switch
        {
            Idioma.PtPt => "Brasil",
            Idioma.En => "Brazil",
            Idioma.ZhCn => "巴西 (Brazil)",
            Idioma.Hi => "ब्राज़ील (Brazil)",
            Idioma.Es => "Brasil",
            Idioma.Ar => "البرازيل (Brazil)",
            _ => "Brasil"
        },
        "PT" => idioma switch
        {
            Idioma.PtPt => "Portugal",
            Idioma.En => "Portugal",
            Idioma.ZhCn => "葡萄牙 (Portugal)",
            Idioma.Hi => "पुर्तगाल (Portugal)",
            Idioma.Es => "Portugal",
            Idioma.Ar => "البرتغال (Portugal)",
            _ => "Portugal"
        },
        "US" => idioma switch
        {
            Idioma.PtPt => "Estados Unidos",
            Idioma.En => "United States",
            Idioma.ZhCn => "美国 (United States)",
            Idioma.Hi => "संयुक्त राज्य अमेरिका (United States)",
            Idioma.Es => "Estados Unidos",
            Idioma.Ar => "الولايات المتحدة (United States)",
            _ => "Estados Unidos"
        },
        "CN" => idioma switch
        {
            Idioma.PtPt => "China",
            Idioma.En => "China",
            Idioma.ZhCn => "中国 (China)",
            Idioma.Hi => "चीन (China)",
            Idioma.Es => "China",
            Idioma.Ar => "الصين (China)",
            _ => "China"
        },
        "IN" => idioma switch
        {
            Idioma.PtPt => "Índia",
            Idioma.En => "India",
            Idioma.ZhCn => "印度 (India)",
            Idioma.Hi => "भारत (India)",
            Idioma.Es => "India",
            Idioma.Ar => "الهند (India)",
            _ => "Índia"
        },
        "ES" => idioma switch
        {
            Idioma.PtPt => "Espanha",
            Idioma.En => "Spain",
            Idioma.ZhCn => "西班牙 (Spain)",
            Idioma.Hi => "स्पेन (Spain)",
            Idioma.Es => "España",
            Idioma.Ar => "إسبانيا (Spain)",
            _ => "Espanha"
        },
        "AR" => idioma switch
        {
            Idioma.PtPt => "Países de Língua Árabe",
            Idioma.En => "Arabic-Speaking Countries",
            Idioma.ZhCn => "阿拉伯语国家/地区",
            Idioma.Hi => "अरब देश (Arabic Countries)",
            Idioma.Es => "Países de Habla Árabe",
            Idioma.Ar => "البلدان الناطقة باللغة العربية",
            _ => "Países de Língua Árabe"
        },
        "AR_SA" => idioma switch
        {
            Idioma.PtPt => "Arábia Saudita",
            Idioma.En => "Saudi Arabia",
            Idioma.ZhCn => "沙特阿拉伯 (Saudi Arabia)",
            Idioma.Hi => "सऊदी अरब (Saudi Arabia)",
            Idioma.Es => "Arabia Saudita",
            Idioma.Ar => "المملكة العربية السعودية",
            _ => "Arábia Saudita"
        },
        "AR_AE" => idioma switch
        {
            Idioma.PtPt => "Emirados Árabes Unidos",
            Idioma.En => "United Arab Emirates",
            Idioma.ZhCn => "阿拉伯联合酋长国 (UAE)",
            Idioma.Hi => "संयुक्त अरब अमीरात (UAE)",
            Idioma.Es => "Emiratos Árabes Unidos",
            Idioma.Ar => "الإمارات العربية المتحدة",
            _ => "Emirados Árabes Unidos"
        },
        "AR_EG" => idioma switch
        {
            Idioma.PtPt => "Egito",
            Idioma.En => "Egypt",
            Idioma.ZhCn => "埃及 (Egypt)",
            Idioma.Hi => "मिस्र (Egypt)",
            Idioma.Es => "Egipto",
            Idioma.Ar => "جمهورية مصر العربية",
            _ => "Egito"
        },
        _ => countryCode
    };

    // ══════════════════════════════════════════════════════════════
    //  CATEGORIAS DE DOCUMENTOS NO IDIOMA DO PROGRAMA
    // ══════════════════════════════════════════════════════════════

    public static string GetCategoryBadge(DocumentCategory category, Idioma idioma) => category switch
    {
        DocumentCategory.PrimaryPersonal => idioma switch
        {
            Idioma.PtPt => "[Principal]",
            Idioma.En => "[Primary]",
            Idioma.ZhCn => "[主要证件]",
            Idioma.Hi => "[मुख्य दस्तावेज़]",
            Idioma.Es => "[Principal]",
            Idioma.Ar => "[رئيسي]",
            _ => "[Principal]"
        },
        DocumentCategory.SecondaryPersonal => idioma switch
        {
            Idioma.PtPt => "[Alternativo / Secundário]",
            Idioma.En => "[Secondary / Alternative]",
            Idioma.ZhCn => "[备选/次要证件]",
            Idioma.Hi => "[वैकल्पिक]",
            Idioma.Es => "[Secundario / Alternativo]",
            Idioma.Ar => "[بديل]",
            _ => "[Alternativo / Secundário]"
        },
        DocumentCategory.Business => idioma switch
        {
            Idioma.PtPt => "[Empresarial]",
            Idioma.En => "[Business / Company]",
            Idioma.ZhCn => "[企业/法人代码]",
            Idioma.Hi => "[व्यावसायिक]",
            Idioma.Es => "[Empresarial]",
            Idioma.Ar => "[تجاري]",
            _ => "[Empresarial]"
        },
        DocumentCategory.Passport => idioma switch
        {
            Idioma.PtPt => "[Passaporte]",
            Idioma.En => "[Passport]",
            Idioma.ZhCn => "[国际护照]",
            Idioma.Hi => "[पासपोर्ट]",
            Idioma.Es => "[Pasaporte]",
            Idioma.Ar => "[جواز سفر]",
            _ => "[Passaporte]"
        },
        _ => ""
    };

    // ══════════════════════════════════════════════════════════════
    //  NOMES OFICIAIS DE DOCUMENTOS NO IDIOMA DO PROGRAMA
    // ══════════════════════════════════════════════════════════════

    public static string GetDocumentOfficialName(string docId, Idioma idioma) => docId switch
    {
        "BR_CPF" => idioma switch
        {
            Idioma.PtPt => "Cadastro de Pessoas Físicas (Brasil)",
            Idioma.En => "Individual Taxpayer Registry (CPF - Brazil)",
            Idioma.ZhCn => "巴西个人纳税人登记号 (CPF)",
            Idioma.Hi => "ब्राज़ील व्यक्तिगत करदाता रजिस्ट्री (CPF)",
            Idioma.Es => "Registro de Personas Físicas (CPF - Brasil)",
            Idioma.Ar => "السجل الضريبي الفردي البرازيلي (CPF)",
            _ => "Cadastro de Pessoas Físicas (CPF)"
        },
        "BR_RG" => idioma switch
        {
            Idioma.PtPt => "Registo Geral / Carteira de Identidade (Brasil)",
            Idioma.En => "General Registry / State Identity Card (RG - Brazil)",
            Idioma.ZhCn => "巴西州立身份证明文件 (RG)",
            Idioma.Hi => "ब्राज़ील राज्य पहचान पत्र (RG)",
            Idioma.Es => "Registro General / Cédula de Identidad (RG - Brasil)",
            Idioma.Ar => "بطاقة الهوية الإقليمية البرازيلية (RG)",
            _ => "Registro Geral / Carteira de Identidade (RG)"
        },
        "BR_CNPJ" => idioma switch
        {
            Idioma.PtPt => "Registo Nacional de Pessoa Colectiva (Brasil)",
            Idioma.En => "National Registry of Legal Entities (CNPJ - Brazil Business ID)",
            Idioma.ZhCn => "巴西法人机构国家登记代码 (CNPJ 企业税号)",
            Idioma.Hi => "ब्राज़ील कानूनी संस्थाओं की राष्ट्रीय रजिस्ट्री (CNPJ)",
            Idioma.Es => "Registro Nacional de Personas Jurídicas (CNPJ - Brasil)",
            Idioma.Ar => "السجل الوطني للكيانات القانونية (CNPJ - البرازيل)",
            _ => "Cadastro Nacional da Pessoa Jurídica (CNPJ)"
        },
        "PT_NIF" => idioma switch
        {
            Idioma.PtPt => "Número de Identificação Fiscal (Contribuinte)",
            Idioma.En => "Tax Identification Number (NIF - Portugal)",
            Idioma.ZhCn => "葡萄牙纳税人识别号 (NIF)",
            Idioma.Hi => "पुर्तगाल कर पहचान संख्या (NIF)",
            Idioma.Es => "Número de Identificación Fiscal (NIF - Portugal)",
            Idioma.Ar => "رقم التعريف الضريبي البرتغالي (NIF)",
            _ => "Número de Identificação Fiscal (NIF - Portugal)"
        },
        "PT_CC" => idioma switch
        {
            Idioma.PtPt => "Cartão de Cidadão (Identificação Civil)",
            Idioma.En => "Citizen Card / Civil ID (Portugal)",
            Idioma.ZhCn => "葡萄牙公民卡 (NIC)",
            Idioma.Hi => "पुर्तगाल नागरिक पहचान कार्ड",
            Idioma.Es => "Tarjeta de Ciudadano (Identidad Civil - Portugal)",
            Idioma.Ar => "بطاقة المواطن البرتغالية (الهوية المدنية)",
            _ => "Cartão de Cidadão (Identificação Civil - Portugal)"
        },
        "PT_NIPC" => idioma switch
        {
            Idioma.PtPt => "Número de Identificação de Pessoa Colectiva",
            Idioma.En => "Corporate Tax Identification Number (NIPC - Portugal)",
            Idioma.ZhCn => "葡萄牙法人团体代码 (NIPC 企业税号)",
            Idioma.Hi => "पुर्तगाल कॉर्पोरेट कर पहचान संख्या (NIPC)",
            Idioma.Es => "Número de Identificación de Persona Colectiva (NIPC - Portugal)",
            Idioma.Ar => "رقم التعريف للشخصيات الاعتبارية (NIPC - البرتغال)",
            _ => "Número de Identificação de Pessoa Coletiva (NIPC - Portugal)"
        },
        "US_SSN" => idioma switch
        {
            Idioma.PtPt => "Número de Segurança Social (Estados Unidos)",
            Idioma.En => "Social Security Number (SSN - United States)",
            Idioma.ZhCn => "美国社会安全号码 (SSN)",
            Idioma.Hi => "संयुक्त राज्य सामाजिक सुरक्षा संख्या (SSN)",
            Idioma.Es => "Número de Seguro Social (SSN - Estados Unidos)",
            Idioma.Ar => "رقم الضمان الاجتماعي الأمريكي (SSN)",
            _ => "Social Security Number (SSN - Estados Unidos)"
        },
        "US_EIN" => idioma switch
        {
            Idioma.PtPt => "Número de Identificação de Empregador (EUA)",
            Idioma.En => "Employer Identification Number (EIN - US Federal Tax ID)",
            Idioma.ZhCn => "美国联邦雇主税号 (EIN)",
            Idioma.Hi => "अमेरिकी नियोक्ता पहचान संख्या (EIN)",
            Idioma.Es => "Número de Identificación del Empleador (EIN - EE. UU.)",
            Idioma.Ar => "رقم تعريف صاحب العمل الفيدرالي الأمريكي (EIN)",
            _ => "Employer Identification Number (EIN - EUA)"
        },
        "CN_RESIDENT_ID" => idioma switch
        {
            Idioma.PtPt => "Cartão de Identidade de Residente (China - 18 dígitos)",
            Idioma.En => "Resident Identity Card Number (China - 18 digits)",
            Idioma.ZhCn => "居民身份证号码 (GB 11643-1999 18位)",
            Idioma.Hi => "चीन निवासी पहचान पत्र संख्या (18 अंक)",
            Idioma.Es => "Documento de Identidad de Residente (China - 18 dígitos)",
            Idioma.Ar => "رقم بطاقة هوية المقيم الصينية (18 رقماً)",
            _ => "Documento de Identidade de Residente (China - 18 dígitos)"
        },
        "CN_USCC" => idioma switch
        {
            Idioma.PtPt => "Código Unificado de Crédito Social (China Empresas)",
            Idioma.En => "Unified Social Credit Code (USCC - China Business ID)",
            Idioma.ZhCn => "统一社会信用代码 (企业及组织机构代码)",
            Idioma.Hi => "चीन एकीकृत सामाजिक ऋण कोड (USCC)",
            Idioma.Es => "Código Unificado de Crédito Social (USCC - China)",
            Idioma.Ar => "رمز الائتمان الاجتماعي الموحد للشركات الصينية (USCC)",
            _ => "Código Unificado de Crédito Social (USCC - China)"
        },
        "IN_AADHAAR" => idioma switch
        {
            Idioma.PtPt => "Número Aadhaar (Identificação Única da Índia)",
            Idioma.En => "Aadhaar Number (UIDAI - India Unique ID)",
            Idioma.ZhCn => "印度唯一身份识别号码 (Aadhaar 12位)",
            Idioma.Hi => "आधार संख्या (UIDAI - भारत)",
            Idioma.Es => "Número Aadhaar (Identificación Única de India)",
            Idioma.Ar => "رقم الهوية الهندية الفريد (Aadhaar)",
            _ => "Número Aadhaar (Identificação Única da Índia)"
        },
        "IN_PAN" => idioma switch
        {
            Idioma.PtPt => "Número de Conta Permanente (Receita da Índia)",
            Idioma.En => "Permanent Account Number (PAN - India Tax ID)",
            Idioma.ZhCn => "印度永久账号代码 (PAN 税号)",
            Idioma.Hi => "स्थायी खाता संख्या (PAN - आयकर विभाग)",
            Idioma.Es => "Número de Cuenta Permanente (PAN - Impuestos de India)",
            Idioma.Ar => "رقم الحساب الدائم (PAN - مصلحة الضرائب الهندية)",
            _ => "Número de Conta Permanente (PAN - Índia)"
        },
        "IN_GSTIN" => idioma switch
        {
            Idioma.PtPt => "Identificação Fiscal de Bens e Serviços (Índia)",
            Idioma.En => "Goods and Services Tax ID Number (GSTIN - India)",
            Idioma.ZhCn => "印度商品及服务税代码 (GSTIN)",
            Idioma.Hi => "माल एवं सेवा कर पहचान संख्या (GSTIN)",
            Idioma.Es => "Identificación de Impuesto sobre Bienes y Servicios (GSTIN - India)",
            Idioma.Ar => "رقم التعريف لضريبة السلع والخدمات (GSTIN - الهند)",
            _ => "Identificação Fiscal de Bens e Serviços (GSTIN - Índia)"
        },
        "ES_DNI" => idioma switch
        {
            Idioma.PtPt => "Documento Nacional de Identidade (DNI - Espanha)",
            Idioma.En => "National Identity Document (DNI - Spain)",
            Idioma.ZhCn => "西班牙国家身份证明文件 (DNI)",
            Idioma.Hi => "स्पेन राष्ट्रीय पहचान दस्तावेज़ (DNI)",
            Idioma.Es => "Documento Nacional de Identidad (DNI - España)",
            Idioma.Ar => "وثيقة الهوية الوطنية الإسبانية (DNI)",
            _ => "Documento Nacional de Identidade (DNI - Espanha)"
        },
        "ES_NIE" => idioma switch
        {
            Idioma.PtPt => "Número de Identificação de Estrangeiro (Espanha)",
            Idioma.En => "Foreigner Identity Number (NIE - Spain)",
            Idioma.ZhCn => "西班牙外国人身份证号码 (NIE)",
            Idioma.Hi => "स्पेन विदेशी पहचान संख्या (NIE)",
            Idioma.Es => "Número de Identidad de Extranjero (NIE - España)",
            Idioma.Ar => "رقم هوية الأجنبي المقيم (NIE - إسبانيا)",
            _ => "Número de Identidade de Estrangeiro (NIE - Espanha)"
        },
        "ES_CIF" => idioma switch
        {
            Idioma.PtPt => "Código de Identificação Fiscal Empresarial (Espanha)",
            Idioma.En => "Corporate Tax Identification Code (CIF - Spain)",
            Idioma.ZhCn => "西班牙公司法人税号 (CIF)",
            Idioma.Hi => "स्पेन कॉर्पोरेट कर पहचान कोड (CIF)",
            Idioma.Es => "Código de Identificación Fiscal / NIF Empresa (España)",
            Idioma.Ar => "رمز الهوية الضريبية للشركات (CIF - إسبانيا)",
            _ => "Código de Identificação Fiscal Empresarial (CIF - Espanha)"
        },
        "AR_SA_NATIONAL_ID" => idioma switch
        {
            Idioma.PtPt => "Identidade Nacional Saudita / Iqama (10 dígitos)",
            Idioma.En => "Saudi Citizen National ID / Resident Iqama (10 digits)",
            Idioma.ZhCn => "沙特国民身份证 / 居留许可 (Iqama 10位)",
            Idioma.Hi => "सऊदी राष्ट्रीय पहचान पत्र / इकामा (10 अंक)",
            Idioma.Es => "Identidad Nacional Saudí / Residencia Iqama (10 dígitos)",
            Idioma.Ar => "رقم الهوية الوطنية للمواطن أو هوية مقيم (الإقامة)",
            _ => "Identidade Nacional Saudita / Iqama (10 dígitos)"
        },
        "AR_SA_CR" => idioma switch
        {
            Idioma.PtPt => "Registo Comercial / Imposto Saudita (CR)",
            Idioma.En => "Commercial Registration / Tax ID (CR - Saudi Arabia)",
            Idioma.ZhCn => "沙特商业登记代码 / 税务编号 (CR)",
            Idioma.Hi => "वाणिज्यिक पंजीकरण / कर पहचान (सऊदी अरब)",
            Idioma.Es => "Registro Comercial / Identificación Fiscal (CR - Arabia Saudita)",
            Idioma.Ar => "السجل التجاري / الرقم الضريبي (المملكة العربية السعودية)",
            _ => "Registro Comercial / Imposto Saudita (CR)"
        },
        "AR_AE_EMIRATES_ID" => idioma switch
        {
            Idioma.PtPt => "Cartão de Identidade dos Emirados Árabes (Emirates ID)",
            Idioma.En => "Emirates ID (United Arab Emirates 15 digits)",
            Idioma.ZhCn => "阿联酋国家身份证 (Emirates ID 15位)",
            Idioma.Hi => "अमीरात पहचान पत्र (Emirates ID 15 अंक)",
            Idioma.Es => "Emirates ID (Documento de Identidad de Emiratos Árabes)",
            Idioma.Ar => "بطاقة الهوية الإماراتية (الهيئة الاتحادية للهوية والجنسية)",
            _ => "Emirates ID (Cartão de Identidade dos Emirados Árabes)"
        },
        "AR_EG_NATIONAL_ID" => idioma switch
        {
            Idioma.PtPt => "Cartão de Cidadão do Egito (14 dígitos)",
            Idioma.En => "Egyptian National Identification Card (14 digits)",
            Idioma.ZhCn => "埃及国民身份证号码 (14位)",
            Idioma.Hi => "मिस्र राष्ट्रीय पहचान पत्र (14 अंक)",
            Idioma.Es => "Cédula de Identidad Nacional de Egipto (14 dígitos)",
            Idioma.Ar => "بطاقة الرقم القومي المصري (مصلحة الأحوال المدنية)",
            _ => "Identidade Nacional do Egito (14 dígitos)"
        },
        "PASSPORT" => idioma switch
        {
            Idioma.PtPt => "Passaporte Internacional de Viagem",
            Idioma.En => "International Travel Passport",
            Idioma.ZhCn => "国际旅行护照 (ICAO Doc 9303)",
            Idioma.Hi => "अंतर्राष्ट्रीय यात्रा पासपोर्ट",
            Idioma.Es => "Pasaporte Internacional de Viaje",
            Idioma.Ar => "جواز السفر الدولي (وثيقة السفر المعتمدة)",
            _ => "Passaporte Internacional de Viagem"
        },
        _ => docId
    };

    // ══════════════════════════════════════════════════════════════
    //  FEEDBACK DE DIGITAÇÃO E VALIDAÇÃO
    // ══════════════════════════════════════════════════════════════

    public static string GetExamplePrefix(Idioma idioma) => idioma switch
    {
        Idioma.PtPt => "Exemplo / Formato:",
        Idioma.En => "Example / Format:",
        Idioma.ZhCn => "示例 / 格式:",
        Idioma.Hi => "उदाहरण / प्रारूप:",
        Idioma.Es => "Ejemplo / Formato:",
        Idioma.Ar => "مثال / الصيغة:",
        _ => "Exemplo / Formato:"
    };

    public static string GetMaskLabel(Idioma idioma) => idioma switch
    {
        Idioma.PtPt => "Máscara:",
        Idioma.En => "Mask:",
        Idioma.ZhCn => "掩码:",
        Idioma.Hi => "मास्क:",
        Idioma.Es => "Máscara:",
        Idioma.Ar => "القناع:",
        _ => "Máscara:"
    };

    public static string GetStatusRequired(Idioma idioma) => idioma switch
    {
        Idioma.PtPt => "Preenchimento obrigatório",
        Idioma.En => "Required field",
        Idioma.ZhCn => "必填字段",
        Idioma.Hi => "अनिवार्य फ़ील्ड",
        Idioma.Es => "Campo obligatorio",
        Idioma.Ar => "حقل مطلوب",
        _ => "Preenchimento obrigatório"
    };

    public static string GetStatusOptional(Idioma idioma) => idioma switch
    {
        Idioma.PtPt => "Documento opcional não informado",
        Idioma.En => "Optional document not provided",
        Idioma.ZhCn => "未填写可选证件",
        Idioma.Hi => "वैकल्पिक दस्तावेज़ नहीं भरा गया",
        Idioma.Es => "Documento opcional no informado",
        Idioma.Ar => "وثيقة اختيارية لم يتم تقديمها",
        _ => "Documento opcional não informado"
    };

    public static string GetStatusTyping(Idioma idioma, int current, int required) => idioma switch
    {
        Idioma.PtPt => $"Formato em digitação ({current}/{required} dígitos)",
        Idioma.En => $"Typing format ({current}/{required} digits)",
        Idioma.ZhCn => $"正在输入 ({current}/{required} 位)",
        Idioma.Hi => $"टाइप किया जा रहा है ({current}/{required} अंक)",
        Idioma.Es => $"Formato en escritura ({current}/{required} dígitos)",
        Idioma.Ar => $"جارٍ الإدخال ({current}/{required} أرقام)",
        _ => $"Formato em digitação ({current}/{required} dígitos)"
    };

    public static string GetStatusValid(Idioma idioma, string algorithm) => idioma switch
    {
        Idioma.PtPt => $"Válido ✓ ({algorithm})",
        Idioma.En => $"Valid ✓ ({algorithm})",
        Idioma.ZhCn => $"有效 ✓ ({algorithm})",
        Idioma.Hi => $"मान्य ✓ ({algorithm})",
        Idioma.Es => $"Válido ✓ ({algorithm})",
        Idioma.Ar => $"صالح ✓ ({algorithm})",
        _ => $"Válido ✓ ({algorithm})"
    };
}
