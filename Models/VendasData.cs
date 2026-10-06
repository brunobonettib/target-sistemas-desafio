using System.Text.Json.Serialization;

namespace DesafioTarget.Models;

public sealed class VendasData
{
    [JsonPropertyName("vendas")]
    public List<Venda> Vendas { get; set; } = [];
}
