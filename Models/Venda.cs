using System.Text.Json.Serialization;

namespace DesafioTarget.Models;

public sealed class Venda
{
    [JsonPropertyName("vendedor")]
    public string Vendedor { get; set; } = string.Empty;

    [JsonPropertyName("valor")]
    public decimal Valor { get; set; }
}
