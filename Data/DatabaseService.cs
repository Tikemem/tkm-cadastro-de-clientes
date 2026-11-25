using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using CadastroClientes.InternationalDocumentManager.DocumentSecurity;
using CadastroClientes.Models;
using Microsoft.Data.Sqlite;

namespace CadastroClientes.Data;

public sealed class DatabaseService
{
    public static string PastaDados { get; } = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "CadastroClientes");

    public string CaminhoBanco { get; }
    private readonly string _connectionString;

    private const string Colunas =
        "PaisResidencia, Nome, TipoDocumento, Documento, DocumentoHash, Nacionalidade, Email, Telefone, Cep, Logradouro, Numero, Bairro, Cidade, Uf, DataNascimento, Observacoes";

    private const string Parametros =
        "@PaisResidencia, @Nome, @TipoDocumento, @Documento, @DocumentoHash, @Nacionalidade, @Email, @Telefone, @Cep, @Logradouro, @Numero, @Bairro, @Cidade, @Uf, @DataNascimento, @Observacoes";

    public DatabaseService(string? caminho = null)
    {
        CaminhoBanco = caminho ?? Path.Combine(PastaDados, "clientes.db");
        Directory.CreateDirectory(Path.GetDirectoryName(CaminhoBanco)!);

        _connectionString = new SqliteConnectionStringBuilder
        {
            DataSource = CaminhoBanco,
            Mode = SqliteOpenMode.ReadWriteCreate
        }.ToString();

        Inicializar();
    }

    private SqliteConnection Abrir()
    {
        var con = new SqliteConnection(_connectionString);
        con.Open();
        return con;
    }

    private void Inicializar()
    {
        using var con = Abrir();

        // 1. Cria tabela caso não exista
        using (var cmd = con.CreateCommand())
        {
            cmd.CommandText = """
                CREATE TABLE IF NOT EXISTS Clientes (
                    Id              INTEGER PRIMARY KEY AUTOINCREMENT,
                    PaisResidencia  TEXT NOT NULL DEFAULT 'BR',
                    Nome            TEXT NOT NULL,
                    TipoDocumento   TEXT NOT NULL DEFAULT 'BR_CPF',
                    Documento       TEXT NOT NULL,
                    DocumentoHash   TEXT,
                    Nacionalidade   TEXT,
                    Email           TEXT,
                    Telefone        TEXT,
                    Cep             TEXT,
                    Logradouro      TEXT,
                    Numero          TEXT,
                    Bairro          TEXT,
                    Cidade          TEXT,
                    Uf              TEXT,
                    DataNascimento  TEXT,
                    Observacoes     TEXT,
                    DataCadastro    TEXT NOT NULL,
                    DataAtualizacao TEXT NOT NULL
                );
                CREATE INDEX IF NOT EXISTS IX_Clientes_Nome ON Clientes (Nome COLLATE NOCASE);
                """;
            cmd.ExecuteNonQuery();
        }

        // 2. Migração automática e transparente de colunas para bancos existentes
        MigrarColunas(con);
    }

    private static void MigrarColunas(SqliteConnection con)
    {
        var colunas = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        using (var infoCmd = con.CreateCommand())
        {
            infoCmd.CommandText = "PRAGMA table_info(Clientes)";
            using var r = infoCmd.ExecuteReader();
            while (r.Read()) colunas.Add(r.GetString(1));
        }

        void AdicionarColuna(string nome, string definicao)
        {
            if (!colunas.Contains(nome))
            {
                using var cmd = con.CreateCommand();
                cmd.CommandText = $"ALTER TABLE Clientes ADD COLUMN {nome} {definicao}";
                cmd.ExecuteNonQuery();
            }
        }

        AdicionarColuna("PaisResidencia", "TEXT NOT NULL DEFAULT 'BR'");
        AdicionarColuna("TipoDocumento", "TEXT NOT NULL DEFAULT 'BR_CPF'");
        AdicionarColuna("DocumentoHash", "TEXT");
        AdicionarColuna("Nacionalidade", "TEXT");

        // Cria índice para DocumentoHash caso ainda não exista
        using (var idxCmd = con.CreateCommand())
        {
            idxCmd.CommandText = "CREATE INDEX IF NOT EXISTS IX_Clientes_DocHash ON Clientes (DocumentoHash);";
            idxCmd.ExecuteNonQuery();
        }

        // Atualiza DocumentoHash para registros existentes que ainda não possuam hash
        using (var updateHashCmd = con.CreateCommand())
        {
            updateHashCmd.CommandText = "SELECT Id, Documento FROM Clientes WHERE DocumentoHash IS NULL OR DocumentoHash = ''";
            var pendentes = new List<(int Id, string Doc)>();
            using (var r = updateHashCmd.ExecuteReader())
            {
                while (r.Read())
                {
                    pendentes.Add((r.GetInt32(0), r.GetString(1)));
                }
            }

            foreach (var (id, doc) in pendentes)
            {
                string raw = Validador.Digitos(doc);
                string hash = CalcularHash(raw);
                string enc = DocumentSecurityService.Criptografar(raw);

                using var upd = con.CreateCommand();
                upd.CommandText = "UPDATE Clientes SET Documento = @Enc, DocumentoHash = @Hash WHERE Id = @Id";
                upd.Parameters.AddWithValue("@Enc", enc);
                upd.Parameters.AddWithValue("@Hash", hash);
                upd.Parameters.AddWithValue("@Id", id);
                upd.ExecuteNonQuery();
            }
        }
    }

    public List<Cliente> Listar(string? termo = null)
    {
        var t = termo?.Trim() ?? "";
        using var con = Abrir();
        using var cmd = con.CreateCommand();
        cmd.CommandText = """
            SELECT * FROM Clientes
            WHERE @t = '' OR Nome LIKE @like OR Email LIKE @like OR Cidade LIKE @like OR Nacionalidade LIKE @like
            ORDER BY Nome COLLATE NOCASE
            """;
        cmd.Parameters.AddWithValue("@t", t);
        cmd.Parameters.AddWithValue("@like", $"%{t}%");

        var lista = new List<Cliente>();
        using var r = cmd.ExecuteReader();
        while (r.Read()) lista.Add(Mapear(r));

        // Se o termo digitado for numérico ou pesquisa por documento, filtra também pelo documento descriptografado
        if (!string.IsNullOrEmpty(t))
        {
            var termoLimpo = new string(t.Where(char.IsLetterOrDigit).ToArray());
            if (!string.IsNullOrEmpty(termoLimpo))
            {
                // Inclui clientes cujo documento contenha o termo buscado
                using var cmdTodos = con.CreateCommand();
                cmdTodos.CommandText = "SELECT * FROM Clientes";
                using var rTodos = cmdTodos.ExecuteReader();
                var setIds = new HashSet<int>(lista.Select(c => c.Id));
                while (rTodos.Read())
                {
                    var c = Mapear(rTodos);
                    if (!setIds.Contains(c.Id))
                    {
                        var docLimpo = new string(c.Documento.Where(char.IsLetterOrDigit).ToArray());
                        if (docLimpo.Contains(termoLimpo, StringComparison.OrdinalIgnoreCase))
                        {
                            lista.Add(c);
                        }
                    }
                }
            }
        }

        return lista.OrderBy(c => c.Nome, StringComparer.OrdinalIgnoreCase).ToList();
    }

    public Cliente? ObterPorId(int id)
    {
        using var con = Abrir();
        using var cmd = con.CreateCommand();
        cmd.CommandText = "SELECT * FROM Clientes WHERE Id = @Id";
        cmd.Parameters.AddWithValue("@Id", id);
        using var r = cmd.ExecuteReader();
        return r.Read() ? Mapear(r) : null;
    }

    public bool DocumentoExiste(string rawDocumento, string? pais, int? ignorarId)
    {
        string raw = rawDocumento.Trim();
        string hash = CalcularHash(raw);

        using var con = Abrir();
        using var cmd = con.CreateCommand();
        cmd.CommandText = """
            SELECT COUNT(1) FROM Clientes
            WHERE (DocumentoHash = @Hash OR Documento = @Raw)
              AND (@Pais IS NULL OR PaisResidencia = @Pais)
              AND (@Ignorar IS NULL OR Id <> @Ignorar)
            """;
        cmd.Parameters.AddWithValue("@Hash", hash);
        cmd.Parameters.AddWithValue("@Raw", raw);
        cmd.Parameters.AddWithValue("@Pais", string.IsNullOrWhiteSpace(pais) ? DBNull.Value : (object)pais.Trim());
        cmd.Parameters.AddWithValue("@Ignorar", (object?)ignorarId ?? DBNull.Value);
        return Convert.ToInt64(cmd.ExecuteScalar()) > 0;
    }

    public int Inserir(Cliente c)
    {
        using var con = Abrir();

        using (var cmd = con.CreateCommand())
        {
            cmd.CommandText = $"INSERT INTO Clientes ({Colunas}, DataCadastro, DataAtualizacao) " +
                              $"VALUES ({Parametros}, @Agora, @Agora)";
            Preencher(cmd, c);
            cmd.Parameters.AddWithValue("@Agora", FormatarData(DateTime.Now));
            cmd.ExecuteNonQuery();
        }

        using var idCmd = con.CreateCommand();
        idCmd.CommandText = "SELECT last_insert_rowid()";
        return Convert.ToInt32(idCmd.ExecuteScalar(), CultureInfo.InvariantCulture);
    }

    public void Atualizar(Cliente c)
    {
        using var con = Abrir();
        using var cmd = con.CreateCommand();
        cmd.CommandText = """
            UPDATE Clientes SET
                PaisResidencia = @PaisResidencia, Nome = @Nome, TipoDocumento = @TipoDocumento,
                Documento = @Documento, DocumentoHash = @DocumentoHash, Nacionalidade = @Nacionalidade,
                Email = @Email, Telefone = @Telefone, Cep = @Cep, Logradouro = @Logradouro,
                Numero = @Numero, Bairro = @Bairro, Cidade = @Cidade, Uf = @Uf,
                DataNascimento = @DataNascimento, Observacoes = @Observacoes, DataAtualizacao = @Agora
            WHERE Id = @Id
            """;
        Preencher(cmd, c);
        cmd.Parameters.AddWithValue("@Id", c.Id);
        cmd.Parameters.AddWithValue("@Agora", FormatarData(DateTime.Now));
        cmd.ExecuteNonQuery();
    }

    public void Excluir(int id)
    {
        using var con = Abrir();
        using var cmd = con.CreateCommand();
        cmd.CommandText = "DELETE FROM Clientes WHERE Id = @Id";
        cmd.Parameters.AddWithValue("@Id", id);
        cmd.ExecuteNonQuery();
    }

    public void CriarCopia(string destino)
    {
        if (File.Exists(destino)) File.Delete(destino);
        var caminhoSql = destino.Replace("'", "''");
        using var con = Abrir();
        using var cmd = con.CreateCommand();
        cmd.CommandText = $"VACUUM INTO '{caminhoSql}'";
        cmd.ExecuteNonQuery();
    }

    public void RestaurarDe(string arquivo)
    {
        var csValidacao = new SqliteConnectionStringBuilder
        {
            DataSource = arquivo,
            Mode = SqliteOpenMode.ReadOnly
        }.ToString();

        using (var con = new SqliteConnection(csValidacao))
        {
            con.Open();
            using var cmd = con.CreateCommand();
            cmd.CommandText = "SELECT COUNT(*) FROM Clientes";
            cmd.ExecuteScalar();
        }

        SqliteConnection.ClearAllPools();
        File.Copy(arquivo, CaminhoBanco, overwrite: true);
    }

    private static void Preencher(SqliteCommand cmd, Cliente c)
    {
        string rawDoc = c.Documento.Trim();

        cmd.Parameters.AddWithValue("@PaisResidencia", string.IsNullOrWhiteSpace(c.PaisResidencia) ? "BR" : c.PaisResidencia.Trim());
        cmd.Parameters.AddWithValue("@Nome", c.Nome.Trim());
        cmd.Parameters.AddWithValue("@TipoDocumento", string.IsNullOrWhiteSpace(c.TipoDocumento) ? "BR_CPF" : c.TipoDocumento.Trim());
        // Armazena criptografado para máxima segurança (LGPD/GDPR)
        cmd.Parameters.AddWithValue("@Documento", DocumentSecurityService.Criptografar(rawDoc));
        cmd.Parameters.AddWithValue("@DocumentoHash", CalcularHash(rawDoc));
        cmd.Parameters.AddWithValue("@Nacionalidade", Nulo(c.Nacionalidade));
        cmd.Parameters.AddWithValue("@Email", Nulo(c.Email));
        cmd.Parameters.AddWithValue("@Telefone", Nulo(c.Telefone));
        cmd.Parameters.AddWithValue("@Cep", Nulo(c.Cep));
        cmd.Parameters.AddWithValue("@Logradouro", Nulo(c.Logradouro));
        cmd.Parameters.AddWithValue("@Numero", Nulo(c.Numero));
        cmd.Parameters.AddWithValue("@Bairro", Nulo(c.Bairro));
        cmd.Parameters.AddWithValue("@Cidade", Nulo(c.Cidade));
        cmd.Parameters.AddWithValue("@Uf", Nulo(c.Uf));
        cmd.Parameters.AddWithValue("@DataNascimento",
            c.DataNascimento is { } d ? (object)d.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture) : DBNull.Value);
        cmd.Parameters.AddWithValue("@Observacoes", Nulo(c.Observacoes));
    }

    private static Cliente Mapear(SqliteDataReader r) => new()
    {
        Id = r.GetInt32(r.GetOrdinal("Id")),
        PaisResidencia = Texto(r, "PaisResidencia") ?? "BR",
        Nome = Texto(r, "Nome") ?? "",
        TipoDocumento = Texto(r, "TipoDocumento") ?? "BR_CPF",
        // Descriptografa com segurança para o modelo de domínio em memória
        Documento = DocumentSecurityService.Descriptografar(Texto(r, "Documento") ?? ""),
        Nacionalidade = Texto(r, "Nacionalidade"),
        Email = Texto(r, "Email"),
        Telefone = Texto(r, "Telefone"),
        Cep = Texto(r, "Cep"),
        Logradouro = Texto(r, "Logradouro"),
        Numero = Texto(r, "Numero"),
        Bairro = Texto(r, "Bairro"),
        Cidade = Texto(r, "Cidade"),
        Uf = Texto(r, "Uf"),
        DataNascimento = Data(r, "DataNascimento"),
        Observacoes = Texto(r, "Observacoes"),
        DataCadastro = Data(r, "DataCadastro") ?? DateTime.Now,
        DataAtualizacao = Data(r, "DataAtualizacao") ?? DateTime.Now
    };

    private static string CalcularHash(string valor)
    {
        if (string.IsNullOrEmpty(valor)) return "";
        byte[] hash = SHA256.HashData(Encoding.UTF8.GetBytes(valor.ToUpperInvariant() + ":TKM-DocSalt-2026"));
        return Convert.ToHexString(hash);
    }

    private static string? Texto(SqliteDataReader r, string coluna)
    {
        int i = r.GetOrdinal(coluna);
        return r.IsDBNull(i) ? null : r.GetString(i);
    }

    private static DateTime? Data(SqliteDataReader r, string coluna)
    {
        var texto = Texto(r, coluna);
        if (texto is null) return null;
        return DateTime.Parse(texto, CultureInfo.InvariantCulture);
    }

    private static object Nulo(string? valor) =>
        string.IsNullOrWhiteSpace(valor) ? DBNull.Value : (object)valor.Trim();

    private static string FormatarData(DateTime d) =>
        d.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
}
