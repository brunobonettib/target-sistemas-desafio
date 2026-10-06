using DesafioTarget.Domain.Exceptions;
using DesafioTarget.Domain.ValueObjects;

namespace DesafioTarget.Domain.Services;

public static class InterestCalculator
{
    private const decimal DailyFinePercentage = 0.025m;

    public static InterestResult Calculate(decimal amount, DateOnly dueDate, DateOnly referenceDate)
    {
        if (amount < 0m)
        {
            throw new BusinessRuleException("O valor não pode ser negativo.");
        }

        // Se o vencimento ainda não passou, não há atraso e os juros ficam zerados.
        var overdueDays = Math.Max(0, referenceDate.DayNumber - dueDate.DayNumber);
        var interestAmount = MoneyRounder.Round(amount * DailyFinePercentage * overdueDays);
        var totalAmount = MoneyRounder.Round(amount + interestAmount);

        return new InterestResult(
            MoneyRounder.Round(amount),
            dueDate,
            referenceDate,
            overdueDays,
            interestAmount,
            totalAmount);
    }
}
