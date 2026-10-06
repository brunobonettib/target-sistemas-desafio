using System.Text.Json.Serialization;

namespace DesafioTarget.Models;

public sealed class EstoqueData
{
    [JsonPropertyName("estoque")]
    public List<ProdutoEstoque> Estoque { get; set; } = [];
}
