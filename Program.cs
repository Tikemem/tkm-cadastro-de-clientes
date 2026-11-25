using System;
using System.IO;

namespace CadastroClientes;

public static class Program
{
    [STAThread]
    public static void Main(string[] args)
    {
        try
        {
            WinRT.ComWrappersSupport.InitializeComWrappers();
            Microsoft.UI.Xaml.Application.Start((p) =>
            {
                var context = new Microsoft.UI.Dispatching.DispatcherQueueSynchronizationContext(
                    Microsoft.UI.Dispatching.DispatcherQueue.GetForCurrentThread());
                System.Threading.SynchronizationContext.SetSynchronizationContext(context);
                new App();
            });
        }
        catch (Exception ex)
        {
            try
            {
                var logPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "app_debug.log");
                File.AppendAllText(logPath, $"[Main] {ex}\n");
            }
            catch { }
            throw;
        }
    }
}
