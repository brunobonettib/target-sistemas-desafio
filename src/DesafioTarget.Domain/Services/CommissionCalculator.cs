using DesafioTarget.Domain.Entities;
using DesafioTarget.Domain.ValueObjects;

namespace DesafioTarget.Domain.Services;

public static class CommissionCalculator
{
    public static decimal CalculateSaleCommission(decimal saleAmount)
    {
        // A comissão é calculada por venda, respeitando a faixa do valor vendido.
        if (saleAmount < 100m)
        {
            return 0m;
        }

        var percentage = saleAmount < 500m ? 0.01m : 0.05m;
        return MoneyRounder.Round(saleAmount * percentage);
    }

    public static IReadOnlyList<CommissionSummary> SummarizeBySeller(IEnumerable<Sale> sales)
    {
        return sales
            .GroupBy(sale => sale.Seller)
            .Select(group => new CommissionSummary(
                group.Key,
                group.Count(),
                MoneyRounder.Round(group.Sum(sale => sale.Amount)),
                MoneyRounder.Round(group.Sum(sale => CalculateSaleCommission(sale.Amount)))))
            .OrderBy(summary => summary.Seller)
            .ToList();
    }
}
