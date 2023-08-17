namespace FunctionAppDadosEmpresas.Models;

public class Empresa
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public string? Nome { get; set; }
    public string? CNPJ { get; set; }
    public string? Cidade { get; set; }
    public DateTime InicioProcessamento { get; set; }
}
