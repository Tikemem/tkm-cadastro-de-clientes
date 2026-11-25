namespace CadastroClientes.Models;

public class Cliente
{
    public int Id { get; set; }
    public string PaisResidencia { get; set; } = "BR";   // Código do país (ex: BR, PT, US, CN, IN, ES, AR_SA)
    public string Nome { get; set; } = "";
    public string TipoDocumento { get; set; } = "BR_CPF"; // ID do tipo de documento
    public string Documento { get; set; } = "";          // Número do documento (armazenado com criptografia)
    public string? Nacionalidade { get; set; }           // Nacionalidade do cliente
    public DateTime? DataNascimento { get; set; }
    public string? Email { get; set; }
    public string? Telefone { get; set; }
    public string? Cep { get; set; }                     // Código postal / CEP
    public string? Logradouro { get; set; }              // Endereço
    public string? Numero { get; set; }
    public string? Bairro { get; set; }
    public string? Cidade { get; set; }
    public string? Uf { get; set; }                      // Estado / Província
    public string? Observacoes { get; set; }
    public DateTime DataCadastro { get; set; } = DateTime.Now;
    public DateTime DataAtualizacao { get; set; } = DateTime.Now;
}
