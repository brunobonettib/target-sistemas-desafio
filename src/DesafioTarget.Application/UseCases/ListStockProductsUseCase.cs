using DesafioTarget.Application.Abstractions;
using DesafioTarget.Domain.Entities;

namespace DesafioTarget.Application.UseCases;

public sealed class ListStockProductsUseCase
{
    private readonly IStockRepository _stockRepository;

    public ListStockProductsUseCase(IStockRepository stockRepository)
    {
        _stockRepository = stockRepository;
    }

    public IReadOnlyList<ProductStock> Execute()
    {
        return _stockRepository.ListProducts();
    }
}
