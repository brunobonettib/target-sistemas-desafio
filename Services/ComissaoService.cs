using DesafioTarget.Models;

namespace DesafioTarget.Services;

public static class ComissaoService
{
    public static decimal CalcularComissaoDaVenda(decimal valor)
    {
        // A comissão é calculada individualmente, respeitando a faixa de valor de cada venda.
        if (valor < 100m)
        {
            return 0m;
        }

        var percentual = valor < 500m ? 0.01m : 0.05m;
        return ArredondarMoeda(valor * percentual);
    }

    public static IReadOnlyList<ResumoComissao> CalcularPorVendedor(IEnumerable<Venda> vendas)
    {
        return vendas
            .GroupBy(venda => venda.Vendedor)
            .Select(grupo => new ResumoComissao(
                grupo.Key,
                grupo.Count(),
                ArredondarMoeda(grupo.Sum(venda => venda.Valor)),
                ArredondarMoeda(grupo.Sum(venda => CalcularComissaoDaVenda(venda.Valor)))))
            .OrderBy(resultado => resultado.Vendedor)
            .ToList();
    }

    private static decimal ArredondarMoeda(decimal valor)
    {
        // Evita o arredondamento bancário padrão e usa o comportamento esperado para valores monetários.
        return decimal.Round(valor, 2, MidpointRounding.AwayFromZero);
    }
}
