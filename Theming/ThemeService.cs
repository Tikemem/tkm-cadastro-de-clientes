using System.Text.Json;
using CadastroClientes.Data;
using CadastroClientes.Services;

namespace CadastroClientes.Theming;

public enum TemaTipo { Claro, Escuro }

public sealed record Configuracao(string Tema);

/// <summary>
/// Guarda a escolha de tema. A aplicação visual é feita pela janela,
/// que define RequestedTheme; as cores vêm dos ThemeDictionaries do App.xaml.
/// </summary>
public static class ThemeService
{
    public static TemaTipo Tipo { get; private set; } = TemaTipo.Claro;

    private static string ArquivoConfig => Path.Combine(DatabaseService.PastaDados, "config.json");

    public static void Alternar()
    {
        Tipo = Tipo == TemaTipo.Claro ? TemaTipo.Escuro : TemaTipo.Claro;
        Salvar();
    }

    public static void CarregarPreferencia()
    {
        try
        {
            if (!File.Exists(ArquivoConfig)) return;
            var cfg = JsonSerializer.Deserialize<Configuracao>(File.ReadAllText(ArquivoConfig));
            if (cfg is not null && Enum.TryParse(cfg.Tema, out TemaTipo tipo))
                Tipo = tipo;
        }
        catch
        {
            // Arquivo corrompido: mantém o tema padrão.
        }
    }

    private static void Salvar()
    {
        Directory.CreateDirectory(DatabaseService.PastaDados);
        File.WriteAllText(ArquivoConfig, JsonSerializer.Serialize(new Configuracao(Tipo.ToString())));
    }
}
