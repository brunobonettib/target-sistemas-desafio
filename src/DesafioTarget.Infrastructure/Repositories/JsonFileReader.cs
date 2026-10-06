using System.Text;
using System.Text.Json;

namespace DesafioTarget.Infrastructure.Repositories;

internal static class JsonFileReader
{
    private static readonly JsonSerializerOptions Options = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public static T Read<T>(string filePath)
    {
        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException($"Arquivo de dados não encontrado: {filePath}");
        }

        var content = File.ReadAllText(filePath, Encoding.UTF8);
        return JsonSerializer.Deserialize<T>(content, Options)
            ?? throw new InvalidOperationException($"Não foi possível ler o arquivo: {filePath}");
    }
}
