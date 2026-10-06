using DesafioTarget.Application.Abstractions;
using DesafioTarget.Domain.Services;
using DesafioTarget.Domain.ValueObjects;

namespace DesafioTarget.Application.UseCases;

public sealed class CalculateCommissionsUseCase
{
    private readonly ISalesRepository _salesRepository;

    public CalculateCommissionsUseCase(ISalesRepository salesRepository)
    {
        _salesRepository = salesRepository;
    }

    public IReadOnlyList<CommissionSummary> Execute()
    {
        var sales = _salesRepository.GetSales();
        return CommissionCalculator.SummarizeBySeller(sales);
    }
}
