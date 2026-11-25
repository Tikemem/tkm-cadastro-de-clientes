namespace CadastroClientes.InternationalDocumentManager.DocumentValidators;

public sealed class EsDniValidator : IIdDocumentValidator
{
    public string DocumentTypeId => "ES_DNI";

    private const string ControlLetters = "TRWAGMYFPDXBNJZSQVHLCKE";

    public ValidationResult Validate(string? rawInput)
    {
        if (string.IsNullOrWhiteSpace(rawInput))
            return ValidationResult.Failure("El DNI es obligatorio.");

        var clean = new string(rawInput.Where(char.IsLetterOrDigit).ToArray()).ToUpperInvariant();
        if (clean.Length != 9)
            return ValidationResult.Failure("El DNI debe tener 8 dígitos y 1 letra de control.");

        string numPart = clean[..8];
        char letraInformada = clean[8];

        if (!numPart.All(char.IsDigit) || !char.IsLetter(letraInformada))
            return ValidationResult.Failure("Formato de DNI inválido. Debe contener 8 números y 1 letra final.");

        int num = int.Parse(numPart);
        char letraEsperada = ControlLetters[num % 23];

        if (letraInformada != letraEsperada)
            return ValidationResult.Failure($"Letra de control del DNI incorrecta. Debería ser '{letraEsperada}'.", "Módulo 23");

        return ValidationResult.Success("Algoritmo Oficial Módulo 23 (Ministerio del Interior de España)");
    }
}

public sealed class EsNieValidator : IIdDocumentValidator
{
    public string DocumentTypeId => "ES_NIE";

    private const string ControlLetters = "TRWAGMYFPDXBNJZSQVHLCKE";

    public ValidationResult Validate(string? rawInput)
    {
        if (string.IsNullOrWhiteSpace(rawInput))
            return ValidationResult.Success("NIE no informado (opcional)", isMathematical: false);

        var clean = new string(rawInput.Where(char.IsLetterOrDigit).ToArray()).ToUpperInvariant();
        if (clean.Length != 9)
            return ValidationResult.Failure("El NIE debe tener 1 letra inicial (X, Y, Z), 7 dígitos y 1 letra de control.");

        char primeira = clean[0];
        if (primeira != 'X' && primeira != 'Y' && primeira != 'Z')
            return ValidationResult.Failure("El NIE debe comenzar con X, Y o Z.");

        string numPart = clean.Substring(1, 7);
        char letraInformada = clean[8];

        if (!numPart.All(char.IsDigit) || !char.IsLetter(letraInformada))
            return ValidationResult.Failure("Estructura de NIE inválida (debe contener 7 dígitos centrales y 1 letra final).");

        int prefixNum = primeira == 'X' ? 0 : (primeira == 'Y' ? 1 : 2);
        int num = int.Parse($"{prefixNum}{numPart}");
        char letraEsperada = ControlLetters[num % 23];

        if (letraInformada != letraEsperada)
            return ValidationResult.Failure($"Letra de control del NIE incorrecta. Debería ser '{letraEsperada}'.", "Módulo 23");

        return ValidationResult.Success("Algoritmo Oficial NIE Módulo 23 (Dirección General de la Policía)");
    }
}

public sealed class EsCifValidator : IIdDocumentValidator
{
    public string DocumentTypeId => "ES_CIF";

    private const string ControlLetters = "JABCDEFGHI";
    private static readonly HashSet<char> ValidPrefixes = new("ABCDEFGHJNPQRSUVW");

    public ValidationResult Validate(string? rawInput)
    {
        if (string.IsNullOrWhiteSpace(rawInput))
            return ValidationResult.Failure("El CIF es obligatorio.");

        var clean = new string(rawInput.Where(char.IsLetterOrDigit).ToArray()).ToUpperInvariant();
        if (clean.Length != 9)
            return ValidationResult.Failure("El CIF debe tener 9 caracteres (1 letra inicial, 7 dígitos y 1 de control).");

        char prefix = clean[0];
        if (!ValidPrefixes.Contains(prefix))
            return ValidationResult.Failure($"Letra inicial '{prefix}' no válida para CIF empresarial.");

        string digits = clean.Substring(1, 7);
        if (!digits.All(char.IsDigit))
            return ValidationResult.Failure("Los 7 caracteres centrales del CIF deben ser números.");

        char control = clean[8];

        // Suma de posiciones pares (índices 1, 3, 5 em base 0)
        int sumaPares = (digits[1] - '0') + (digits[3] - '0') + (digits[5] - '0');

        // Suma de posiciones impares multiplicadas por 2
        int sumaImpares = 0;
        int[] posImpares = { 0, 2, 4, 6 };
        foreach (int idx in posImpares)
        {
            int mult = (digits[idx] - '0') * 2;
            sumaImpares += (mult / 10) + (mult % 10);
        }

        int sumaTotal = sumaPares + sumaImpares;
        int digitoControl = (10 - (sumaTotal % 10)) % 10;
        char letraControl = ControlLetters[digitoControl];

        bool controlValido;
        // Tipos donde el control debe ser obligatoriamente letra: N, P, Q, R, S, W
        if ("NPQRSW".Contains(prefix))
        {
            controlValido = control == letraControl;
        }
        // Tipos donde el control debe ser obligatoriamente número: A, B, E, H
        else if ("ABEH".Contains(prefix))
        {
            controlValido = control == (char)('0' + digitoControl);
        }
        else // Demás tipos: puede ser letra o número
        {
            controlValido = control == letraControl || control == (char)('0' + digitoControl);
        }

        if (!controlValido)
            return ValidationResult.Failure("Carácter de control del CIF incorrecto.", "Algoritmo Oficial CIF");

        return ValidationResult.Success("Algoritmo Oficial de Verificación de CIF (Agencia Tributaria)");
    }
}
