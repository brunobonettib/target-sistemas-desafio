using DesafioTarget.Domain.Entities;

namespace DesafioTarget.Application.Abstractions;

public interface IStockRepository
{
    IReadOnlyList<ProductStock> ListProducts();
    ProductStock? GetByCode(int productCode);
    void Update(ProductStock product);
}
