using System.Text.Json.Serialization;

namespace DesafioTarget.Infrastructure.DataTransfer;

internal sealed class ProductStockDto
{
    [JsonPropertyName("codigoProduto")]
    public int Code { get; set; }

    [JsonPropertyName("descricaoProduto")]
    public string Description { get; set; } = string.Empty;

    [JsonPropertyName("estoque")]
    public int Quantity { get; set; }
}
