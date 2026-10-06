namespace DesafioTarget.Models;

public sealed record ResumoComissao(
    string Vendedor,
    int QuantidadeVendas,
    decimal TotalVendido,
    decimal TotalComissao);
