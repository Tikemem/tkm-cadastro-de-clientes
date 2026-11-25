using System;
using System.IO;
using Microsoft.UI.Xaml;

namespace CadastroClientes;

public partial class App : Application
{
    private Window? _janela;

    public App()
    {
        UnhandledException += (sender, e) =>
        {
            RegistrarErro("UnhandledException", e.Exception);
        };

        AppDomain.CurrentDomain.UnhandledException += (sender, e) =>
        {
            RegistrarErro("AppDomain", e.ExceptionObject as Exception);
        };

        InitializeComponent();
    }

    protected override void OnLaunched(LaunchActivatedEventArgs args)
    {
        try
        {
            _janela = new MainWindow();
            _janela.Activate();
        }
        catch (Exception ex)
        {
            RegistrarErro("OnLaunched", ex);
            throw;
        }
    }

    private static void RegistrarErro(string origem, Exception? ex)
    {
        try
        {
            var logPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "app_debug.log");
            File.AppendAllText(logPath, $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] [{origem}] {ex}\n");
        }
        catch { }
    }
}
