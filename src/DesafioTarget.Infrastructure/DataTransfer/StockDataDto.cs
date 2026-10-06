using System.Text.Json.Serialization;

namespace DesafioTarget.Infrastructure.DataTransfer;

internal sealed class StockDataDto
{
    [JsonPropertyName("estoque")]
    public List<ProductStockDto> Products { get; set; } = [];
}
