using DesafioTarget.Domain.Services;
using DesafioTarget.Domain.ValueObjects;

namespace DesafioTarget.Application.UseCases;

public sealed class CalculateInterestUseCase
{
    public InterestResult Execute(decimal amount, DateOnly dueDate, DateOnly referenceDate)
    {
        return InterestCalculator.Calculate(amount, dueDate, referenceDate);
    }
}
