using System.Text.Json;
using DesafioTarget.Application.Abstractions;
using DesafioTarget.Domain.Entities;
using DesafioTarget.Infrastructure.DataTransfer;

namespace DesafioTarget.Infrastructure.Repositories;

public sealed class JsonSalesRepository : ISalesRepository
{
    private readonly string _filePath;

    public JsonSalesRepository(string dataDirectory)
    {
        _filePath = Path.Combine(dataDirectory, "vendas.json");
    }

    public IReadOnlyList<Sale> GetSales()
    {
        var data = JsonFileReader.Read<SalesDataDto>(_filePath);
        return data.Sales
            .Select(sale => new Sale(sale.Seller, sale.Amount))
            .ToList();
    }
}
