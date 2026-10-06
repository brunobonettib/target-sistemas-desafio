using DesafioTarget.Domain.Entities;

namespace DesafioTarget.Application.Abstractions;

public interface ISalesRepository
{
    IReadOnlyList<Sale> GetSales();
}
