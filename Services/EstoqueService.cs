using DesafioTarget.Models;

namespace DesafioTarget.Services;

public sealed class EstoqueService
{
    private readonly Dictionary<int, ProdutoEstoque> _produtos;
    private readonly HashSet<int> _identificadoresUsados = [];

    public EstoqueService(IEnumerable<ProdutoEstoque> produtos)
    {
        // Clona os produtos para que as movimentações não alterem diretamente a lista carregada do JSON.
        _produtos = produtos.ToDictionary(produto => produto.CodigoProduto, Clone);
    }

    public IReadOnlyCollection<ProdutoEstoque> ListarProdutos()
    {
        return _produtos.Values
            .OrderBy(produto => produto.CodigoProduto)
            .Select(Clone)
            .ToList();
    }

    public MovimentacaoEstoque LancarMovimentacao(
        int identificador,
        string descricao,
        int codigoProduto,
        TipoMovimentacao tipo,
        int quantidade)
    {
        if (identificador <= 0)
        {
            throw new ArgumentException("O identificador da movimentação deve ser maior que zero.");
        }

        if (string.IsNullOrWhiteSpace(descricao))
        {
            throw new ArgumentException("Informe uma descrição para a movimentação.");
        }

        if (quantidade <= 0)
        {
            throw new ArgumentException("A quantidade movimentada deve ser maior que zero.");
        }

        if (!Enum.IsDefined(tipo))
        {
            throw new ArgumentException("Tipo de movimentação inválido.");
        }

        // O identificador só é consumido depois das validações básicas da movimentação.
        if (_identificadoresUsados.Contains(identificador))
        {
            throw new InvalidOperationException("Já existe uma movimentação com esse identificador.");
        }

        if (!_produtos.TryGetValue(codigoProduto, out var produto))
        {
            throw new KeyNotFoundException("Produto não encontrado no estoque.");
        }

        if (tipo == TipoMovimentacao.Saida && produto.Estoque < quantidade)
        {
            throw new InvalidOperationException("A saída não pode deixar o estoque negativo.");
        }

        // A partir daqui a movimentação é considerada válida e pode alterar o estoque.
        _identificadoresUsados.Add(identificador);
        produto.Estoque += tipo == TipoMovimentacao.Entrada ? quantidade : -quantidade;

        return new MovimentacaoEstoque(
            identificador,
            descricao.Trim(),
            produto.CodigoProduto,
            produto.DescricaoProduto,
            tipo,
            quantidade,
            produto.Estoque);
    }

    private static ProdutoEstoque Clone(ProdutoEstoque produto)
    {
        return new ProdutoEstoque
        {
            CodigoProduto = produto.CodigoProduto,
            DescricaoProduto = produto.DescricaoProduto,
            Estoque = produto.Estoque
        };
    }
}
