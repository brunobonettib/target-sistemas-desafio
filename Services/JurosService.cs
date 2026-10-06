using DesafioTarget.Models;

namespace DesafioTarget.Services;

public static class JurosService
{
    private const decimal PercentualMultaDiaria = 0.025m;

    public static JurosCalculado Calcular(decimal valor, DateOnly dataVencimento, DateOnly dataReferencia)
    {
        if (valor < 0m)
        {
            throw new ArgumentException("O valor não pode ser negativo.");
        }

        // Se o vencimento ainda não passou, não há dias de atraso e os juros ficam zerados.
        var diasAtraso = Math.Max(0, dataReferencia.DayNumber - dataVencimento.DayNumber);
        var valorJuros = ArredondarMoeda(valor * PercentualMultaDiaria * diasAtraso);
        var valorTotal = ArredondarMoeda(valor + valorJuros);

        return new JurosCalculado(
            ArredondarMoeda(valor),
            dataVencimento,
            dataReferencia,
            diasAtraso,
            valorJuros,
            valorTotal);
    }

    private static decimal ArredondarMoeda(decimal valor)
    {
        // Mantém o mesmo critério de arredondamento usado no cálculo de comissão.
        return decimal.Round(valor, 2, MidpointRounding.AwayFromZero);
    }
}
