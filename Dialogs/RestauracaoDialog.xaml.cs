using Microsoft.UI.Xaml.Controls;

namespace CadastroClientes.Dialogs;

public sealed partial class RestauracaoDialog : ContentDialog
{
    public RestauracaoDialog(IReadOnlyList<LinhaBackup> backups)
    {
        InitializeComponent();

        TxtInfo.Text = $"{backups.Count} backup(s) disponível(is) na pasta CadastroClientes_Backups do seu Google Drive.";
        Lista.ItemsSource = backups;
    }

    public LinhaBackup? Escolhido => Lista.SelectedItem as LinhaBackup;

    private void Lista_SelectionChanged(object sender, SelectionChangedEventArgs e) =>
        IsPrimaryButtonEnabled = Lista.SelectedItem is not null;
}

public sealed record LinhaBackup(string Id, string Nome, string Criado, string Tamanho)
{
    public string Resumo => $"{Criado}  •  {Tamanho}";
}
