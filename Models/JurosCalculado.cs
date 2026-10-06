namespace DesafioTarget.Models;

public sealed record JurosCalculado(
    decimal ValorOriginal,
    DateOnly DataVencimento,
    DateOnly DataReferencia,
    int DiasAtraso,
    decimal ValorJuros,
    decimal ValorTotal);
