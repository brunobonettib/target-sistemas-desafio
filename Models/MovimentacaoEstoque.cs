namespace DesafioTarget.Models;

public sealed record MovimentacaoEstoque(
    int Identificador,
    string Descricao,
    int CodigoProduto,
    string DescricaoProduto,
    TipoMovimentacao Tipo,
    int Quantidade,
    int EstoqueFinal);
