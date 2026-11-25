namespace CadastroClientes.InternationalDocumentManager.DocumentValidators;

public sealed class PtNifValidator : IIdDocumentValidator
{
    public string DocumentTypeId => "PT_NIF";

    public ValidationResult Validate(string? rawInput)
    {
        if (string.IsNullOrWhiteSpace(rawInput))
            return ValidationResult.Failure("NIF é obrigatório.");

        var d = new string(rawInput.Where(char.IsDigit).ToArray());
        if (d.Length != 9)
            return ValidationResult.Failure("O NIF deve conter exatamente 9 dígitos.");

        char primeiro = d[0];
        // 1, 2, 3: singular; 5: coletiva; 6: administração pública; 8: não residentes; 9: fundos/outros
        if (!"1235689".Contains(primeiro))
            return ValidationResult.Failure("Primeiro dígito do NIF inválido.", "Regras de Emissão AT");

        int soma = 0;
        for (int i = 0; i < 8; i++)
        {
            soma += (d[i] - '0') * (9 - i);
        }

        int resto = soma % 11;
        int digitoEsperado = resto <= 1 ? 0 : 11 - resto;

        if (d[8] - '0' != digitoEsperado)
            return ValidationResult.Failure("Dígito de controlo do NIF inválido.", "Módulo 11 (Autoridade Tributária)");

        return ValidationResult.Success("Algoritmo Oficial Módulo 11 (Autoridade Tributária e Aduaneira de Portugal)");
    }
}

public sealed class PtNipcValidator : IIdDocumentValidator
{
    public string DocumentTypeId => "PT_NIPC";

    public ValidationResult Validate(string? rawInput)
    {
        if (string.IsNullOrWhiteSpace(rawInput))
            return ValidationResult.Failure("NIPC é obrigatório.");

        var d = new string(rawInput.Where(char.IsDigit).ToArray());
        if (d.Length != 9)
            return ValidationResult.Failure("O NIPC deve conter exatamente 9 dígitos.");

        if (d[0] != '5' && d[0] != '6' && d[0] != '8' && d[0] != '9')
            return ValidationResult.Failure("O NIPC empresarial deve iniciar com 5 (ou 6, 8, 9 para entidades públicas/fundos).");

        int soma = 0;
        for (int i = 0; i < 8; i++)
        {
            soma += (d[i] - '0') * (9 - i);
        }

        int resto = soma % 11;
        int digitoEsperado = resto <= 1 ? 0 : 11 - resto;

        if (d[8] - '0' != digitoEsperado)
            return ValidationResult.Failure("Dígito de controlo do NIPC inválido.", "Módulo 11");

        return ValidationResult.Success("Algoritmo Oficial NIPC (Registo Nacional de Pessoas Colectivas)");
    }
}

public sealed class PtCcValidator : IIdDocumentValidator
{
    public string DocumentTypeId => "PT_CC";

    public ValidationResult Validate(string? rawInput)
    {
        if (string.IsNullOrWhiteSpace(rawInput))
            return ValidationResult.Success("Cartão de Cidadão não preenchido (opcional)", isMathematical: false);

        var clean = new string(rawInput.Where(char.IsLetterOrDigit).ToArray()).ToUpperInvariant();
        if (clean.Length != 12)
            return ValidationResult.Failure("O Cartão de Cidadão deve conter 12 caracteres (8 dígitos + controlo + 2 letras + 1 controlo).");

        // Validação dos 8 dígitos + 1º controlo (Módulo 11 do NIC)
        var nic = clean[..9];
        if (nic.All(char.IsDigit))
        {
            int somaNic = 0;
            for (int i = 0; i < 8; i++)
                somaNic += (nic[i] - '0') * (9 - i);
            int resto = somaNic % 11;
            int digitoEsperado = resto <= 1 ? 0 : 11 - resto;
            if (nic[8] - '0' != digitoEsperado)
                return ValidationResult.Failure("Dígito de controlo civil (NIC) inválido.");
        }

        // Validação global com algoritmo de Luhn Módulo 36
        int somaLuhn = 0;
        bool dobrar = false;

        for (int i = clean.Length - 1; i >= 0; i--)
        {
            char c = clean[i];
            int valor = char.IsDigit(c) ? (c - '0') : (c - 'A' + 10);

            if (dobrar)
            {
                valor *= 2;
                if (valor >= 36) valor -= 35;
            }

            somaLuhn += valor;
            dobrar = !dobrar;
        }

        if (somaLuhn % 36 != 0)
            return ValidationResult.Failure("Dígito de controlo geral do Cartão de Cidadão inválido.", "Luhn Modulo 36");

        return ValidationResult.Success("Algoritmo Oficial Luhn Módulo 36 (República Portuguesa)");
    }
}
