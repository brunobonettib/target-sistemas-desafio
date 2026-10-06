using System.Text.Json.Serialization;

namespace DesafioTarget.Infrastructure.DataTransfer;

internal sealed class SalesDataDto
{
    [JsonPropertyName("vendas")]
    public List<SaleDto> Sales { get; set; } = [];
}
