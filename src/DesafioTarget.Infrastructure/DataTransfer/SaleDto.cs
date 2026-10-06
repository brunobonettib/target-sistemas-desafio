using System.Text.Json.Serialization;

namespace DesafioTarget.Infrastructure.DataTransfer;

internal sealed class SaleDto
{
    [JsonPropertyName("vendedor")]
    public string Seller { get; set; } = string.Empty;

    [JsonPropertyName("valor")]
    public decimal Amount { get; set; }
}
