using System.Text.RegularExpressions;

namespace CadastroClientes.InternationalDocumentManager.DocumentValidators;

public sealed class BrCpfValidator : IIdDocumentValidator
{
    public string DocumentTypeId => "BR_CPF";

    public ValidationResult Validate(string? rawInput)
    {
        if (string.IsNullOrWhiteSpace(rawInput))
            return ValidationResult.Failure("CPF é obrigatório.");

        var d = new string(rawInput.Where(char.IsDigit).ToArray());
        if (d.Length != 11)
            return ValidationResult.Failure("O CPF deve conter exatamente 11 dígitos.");

        if (d.Distinct().Count() == 1)
            return ValidationResult.Failure("CPF inválido (todos os dígitos são iguais).", "Módulo 11 Oficial");

        for (int t = 9; t < 11; t++)
        {
            int soma = 0;
            for (int i = 0; i < t; i++)
                soma += (d[i] - '0') * (t + 1 - i);

            int digito = ((soma * 10) % 11) % 10;
            if (d[t] - '0' != digito)
                return ValidationResult.Failure("Dígito verificador do CPF inválido.", "Módulo 11 Oficial");
        }

        return ValidationResult.Success("Algoritmo Oficial Módulo 11 (Receita Federal do Brasil)");
    }
}

public sealed class BrCnpjValidator : IIdDocumentValidator
{
    public string DocumentTypeId => "BR_CNPJ";

    public ValidationResult Validate(string? rawInput)
    {
        if (string.IsNullOrWhiteSpace(rawInput))
            return ValidationResult.Failure("CNPJ é obrigatório.");

        var d = new string(rawInput.Where(char.IsDigit).ToArray());
        if (d.Length != 14)
            return ValidationResult.Failure("O CNPJ deve conter exatamente 14 dígitos.");

        if (d.Distinct().Count() == 1)
            return ValidationResult.Failure("CNPJ inválido (todos os dígitos são iguais).", "Módulo 11 Oficial");

        static int Digito(string s, int[] pesos)
        {
            int soma = 0;
            for (int i = 0; i < pesos.Length; i++)
                soma += (s[i] - '0') * pesos[i];
            int resto = soma % 11;
            return resto < 2 ? 0 : 11 - resto;
        }

        int[] pesos1 = { 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };
        int[] pesos2 = { 6, 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };

        if (Digito(d, pesos1) != d[12] - '0' || Digito(d[..13], pesos2) != d[13] - '0')
            return ValidationResult.Failure("Dígito verificador do CNPJ inválido.", "Módulo 11 Oficial");

        return ValidationResult.Success("Algoritmo Oficial Módulo 11 (Receita Federal do Brasil)");
    }
}

public sealed class BrRgValidator : IIdDocumentValidator
{
    public string DocumentTypeId => "BR_RG";

    public ValidationResult Validate(string? rawInput)
    {
        if (string.IsNullOrWhiteSpace(rawInput))
            return ValidationResult.Success("RG não preenchido (opcional)", isMathematical: false);

        var clean = new string(rawInput.Where(char.IsLetterOrDigit).ToArray());
        if (clean.Length < 7 || clean.Length > 10)
            return ValidationResult.Failure("O RG deve conter entre 7 e 10 caracteres.");

        return ValidationResult.Success("Verificação Estrutural de Registro Geral Estadual", isMathematical: false);
    }
}
