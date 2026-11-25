using System.Text.Json;
using CadastroClientes.Data;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Google.Apis.Upload;
using Google.Apis.Util.Store;
using DriveFile = Google.Apis.Drive.v3.Data.File;

namespace CadastroClientes.Services;

public sealed record BackupRemoto(string Id, string Nome, DateTime Criado, long Tamanho);

/// <summary>
/// Envia e restaura backups do banco SQLite no Google Drive.
/// Usa o escopo drive.file: o app só enxerga arquivos que ele mesmo criou.
/// </summary>
public sealed class GoogleDriveBackupService : IDisposable
{
    private const string NomePasta = "CadastroClientes_Backups";
    private static readonly string[] Escopos = { DriveService.Scope.DriveFile };

    private readonly string _clientSecretPath;
    private readonly string _tokenFolder;
    private DriveService? _drive;
    private string? _pastaId;

    public GoogleDriveBackupService()
    {
        _clientSecretPath = Path.Combine(DatabaseService.PastaDados, "client_secret.json");
        _tokenFolder = Path.Combine(DatabaseService.PastaDados, "google_token");
    }

    public bool Conectado => _drive is not null;

    public bool ConfiguracaoExiste => File.Exists(_clientSecretPath);

    public string CaminhoClientSecret => _clientSecretPath;

    /// <summary>Valida o JSON de credenciais OAuth e o copia para a pasta de dados do app.</summary>
    public void ImportarClientSecret(string arquivoOrigem)
    {
        using (var doc = JsonDocument.Parse(File.ReadAllText(arquivoOrigem)))
        {
            var raiz = doc.RootElement;
            bool valido = raiz.TryGetProperty("installed", out _) || raiz.TryGetProperty("web", out _);
            if (!valido)
                throw new InvalidDataException(
                    "O arquivo não parece ser um client_secret do Google (esperado conter 'installed' ou 'web').");
        }

        Directory.CreateDirectory(DatabaseService.PastaDados);
        File.Copy(arquivoOrigem, _clientSecretPath, overwrite: true);

        // Credenciais trocadas: a próxima operação precisa reconectar.
        _drive?.Dispose();
        _drive = null;
        _pastaId = null;
    }

    public async Task ConectarAsync(CancellationToken ct = default)
    {
        if (!File.Exists(_clientSecretPath))
            throw new FileNotFoundException(
                "Arquivo client_secret.json não encontrado. Consulte o README para configurar o Google Drive.",
                _clientSecretPath);

        GoogleClientSecrets secrets;
        using (var stream = File.OpenRead(_clientSecretPath))
            secrets = await GoogleClientSecrets.FromStreamAsync(stream, ct);

        var store = new FileDataStore(_tokenFolder, fullPath: true);

        // Abre o navegador para o usuário autorizar na primeira vez; depois reutiliza o token salvo.
        var credencial = await GoogleWebAuthorizationBroker.AuthorizeAsync(
            secrets.Secrets, Escopos, "usuario", ct, store);

        _drive = new DriveService(new BaseClientService.Initializer
        {
            HttpClientInitializer = credencial,
            ApplicationName = "CadastroClientes"
        });

        _pastaId = await ObterOuCriarPastaAsync(ct);
    }

    public async Task<BackupRemoto> EnviarAsync(string arquivoLocal, CancellationToken ct = default)
    {
        Garantir();

        var nome = $"clientes_{DateTime.Now:yyyyMMdd_HHmmss}.db";
        var meta = new DriveFile
        {
            Name = nome,
            Parents = new List<string> { _pastaId! }
        };

        using var fs = File.OpenRead(arquivoLocal);
        var requisicao = _drive!.Files.Create(meta, fs, "application/octet-stream");
        requisicao.Fields = "id, name, createdTime, size";

        var progresso = await requisicao.UploadAsync(ct);
        if (progresso.Status == UploadStatus.Failed)
            throw progresso.Exception ?? new IOException("Falha desconhecida no upload para o Google Drive.");

        var f = requisicao.ResponseBody;
        return new BackupRemoto(
            f.Id,
            f.Name,
            f.CreatedTimeDateTimeOffset?.LocalDateTime ?? DateTime.Now,
            f.Size ?? 0);
    }

    public async Task<List<BackupRemoto>> ListarAsync(CancellationToken ct = default)
    {
        Garantir();

        var requisicao = _drive!.Files.List();
        requisicao.Q = $"'{_pastaId}' in parents and trashed = false";
        requisicao.Fields = "files(id, name, createdTime, size)";
        requisicao.OrderBy = "createdTime desc";
        requisicao.PageSize = 100;

        var resposta = await requisicao.ExecuteAsync(ct);
        return resposta.Files
            .Select(f => new BackupRemoto(
                f.Id,
                f.Name,
                f.CreatedTimeDateTimeOffset?.LocalDateTime ?? DateTime.MinValue,
                f.Size ?? 0))
            .ToList();
    }

    public async Task BaixarAsync(string idArquivo, string destinoLocal, CancellationToken ct = default)
    {
        Garantir();

        var requisicao = _drive!.Files.Get(idArquivo);
        using var fs = File.Create(destinoLocal);
        await requisicao.DownloadAsync(fs, ct);
    }

    /// <summary>Mantém somente os backups mais recentes, removendo os demais.</summary>
    public async Task RemoverAntigosAsync(int manter, CancellationToken ct = default)
    {
        var lista = await ListarAsync(ct);
        foreach (var antigo in lista.Skip(manter))
            await _drive!.Files.Delete(antigo.Id).ExecuteAsync(ct);
    }

    private async Task<string> ObterOuCriarPastaAsync(CancellationToken ct)
    {
        var busca = _drive!.Files.List();
        busca.Q = $"name = '{NomePasta}' and mimeType = 'application/vnd.google-apps.folder' and trashed = false";
        busca.Fields = "files(id, name)";

        var resultado = await busca.ExecuteAsync(ct);
        if (resultado.Files.Count > 0)
            return resultado.Files[0].Id;

        var pasta = new DriveFile
        {
            Name = NomePasta,
            MimeType = "application/vnd.google-apps.folder"
        };
        var criada = await _drive.Files.Create(pasta).ExecuteAsync(ct);
        return criada.Id;
    }

    private void Garantir()
    {
        if (_drive is null || _pastaId is null)
            throw new InvalidOperationException("Conecte-se ao Google Drive antes de continuar.");
    }

    public void Dispose() => _drive?.Dispose();
}
