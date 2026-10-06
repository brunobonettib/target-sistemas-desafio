using DesafioTarget.Application.Abstractions;
using DesafioTarget.Domain.Entities;
using DesafioTarget.Domain.Exceptions;
using DesafioTarget.Infrastructure.DataTransfer;

namespace DesafioTarget.Infrastructure.Repositories;

public sealed class JsonStockRepository : IStockRepository
{
    private readonly Dictionary<int, ProductStock> _products;

    public JsonStockRepository(string dataDirectory)
    {
        var filePath = Path.Combine(dataDirectory, "estoque.json");
        var data = JsonFileReader.Read<StockDataDto>(filePath);

        // Os produtos ficam em memória para simular o estoque durante a execução do console.
        _products = data.Products.ToDictionary(
            product => product.Code,
            product => new ProductStock(product.Code, product.Description, product.Quantity));
    }

    public IReadOnlyList<ProductStock> ListProducts()
    {
        return _products.Values
            .OrderBy(product => product.Code)
            .Select(product => product.Clone())
            .ToList();
    }

    public ProductStock? GetByCode(int productCode)
    {
        return _products.TryGetValue(productCode, out var product)
            ? product.Clone()
            : null;
    }

    public void Update(ProductStock product)
    {
        if (!_products.ContainsKey(product.Code))
        {
            throw new BusinessRuleException("Produto não encontrado no estoque.");
        }

        _products[product.Code] = product.Clone();
    }
}
