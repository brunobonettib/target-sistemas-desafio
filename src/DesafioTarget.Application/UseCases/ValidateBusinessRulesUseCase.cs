using DesafioTarget.Application.Abstractions;
using DesafioTarget.Application.Requests;
using DesafioTarget.Domain.Entities;
using DesafioTarget.Domain.Enums;
using DesafioTarget.Domain.Exceptions;
using DesafioTarget.Domain.Services;

namespace DesafioTarget.Application.UseCases;

public sealed class ValidateBusinessRulesUseCase
{
    private readonly ISalesRepository _salesRepository;

    public ValidateBusinessRulesUseCase(ISalesRepository salesRepository)
    {
        _salesRepository = salesRepository;
    }

    public void Execute()
    {
        ValidateCommissions();
        ValidateStockMovements();
        ValidateInterest();
    }

    private void ValidateCommissions()
    {
        Ensure(CommissionCalculator.CalculateSaleCommission(99.99m) == 0m, "Venda abaixo de R$100,00 não deve gerar comissão.");
        Ensure(CommissionCalculator.CalculateSaleCommission(100m) == 1m, "Venda de R$100,00 deve gerar 1% de comissão.");
        Ensure(CommissionCalculator.CalculateSaleCommission(100.50m) == 1.01m, "Comissão deve arredondar meio centavo para cima.");
        Ensure(CommissionCalculator.CalculateSaleCommission(499.99m) == 5m, "Venda abaixo de R$500,00 deve gerar 1% de comissão.");
        Ensure(CommissionCalculator.CalculateSaleCommission(500m) == 25m, "Venda de R$500,00 deve gerar 5% de comissão.");

        var commissions = CommissionCalculator
            .SummarizeBySeller(_salesRepository.GetSales())
            .ToDictionary(item => item.Seller);

        Ensure(commissions["Ana Lima"].TotalCommission == 404.99m, "Comissão total da Ana Lima divergente.");
        Ensure(commissions["Carlos Oliveira"].TotalCommission == 379.38m, "Comissão total do Carlos Oliveira divergente.");
        Ensure(commissions["João Silva"].TotalCommission == 495.69m, "Comissão total do João Silva divergente.");
        Ensure(commissions["Maria Souza"].TotalCommission == 465.96m, "Comissão total da Maria Souza divergente.");
    }

    private static void ValidateStockMovements()
    {
        var product = new ProductStock(1, "Produto teste", 10);
        product.ApplyMovement(StockMovementType.Inbound, 5);
        Ensure(product.Quantity == 15, "Entrada deve somar ao estoque.");

        product.ApplyMovement(StockMovementType.Outbound, 3);
        Ensure(product.Quantity == 12, "Saída deve subtrair do estoque.");

        EnsureError<BusinessRuleException>(() => product.ApplyMovement(StockMovementType.Outbound, 99), "Saída maior que estoque deve ser bloqueada.");
    }

    private static void ValidateInterest()
    {
        var interest = InterestCalculator.Calculate(200m, new DateOnly(2026, 1, 1), new DateOnly(2026, 1, 3));
        Ensure(interest.OverdueDays == 2 && interest.InterestAmount == 10m && interest.TotalAmount == 210m, "Juros deve ser 2,5% ao dia.");

        var notOverdue = InterestCalculator.Calculate(200m, new DateOnly(2026, 1, 10), new DateOnly(2026, 1, 3));
        Ensure(notOverdue.OverdueDays == 0 && notOverdue.InterestAmount == 0m && notOverdue.TotalAmount == 200m, "Título sem atraso não deve gerar juros.");
        EnsureError<BusinessRuleException>(() => InterestCalculator.Calculate(-1m, new DateOnly(2026, 1, 1), new DateOnly(2026, 1, 3)), "Valor negativo deve ser bloqueado.");
    }

    private static void Ensure(bool condition, string message)
    {
        if (!condition)
        {
            throw new BusinessRuleException(message);
        }
    }

    private static void EnsureError<TException>(Action action, string message)
        where TException : Exception
    {
        try
        {
            action();
        }
        catch (TException)
        {
            return;
        }

        throw new BusinessRuleException(message);
    }
}
