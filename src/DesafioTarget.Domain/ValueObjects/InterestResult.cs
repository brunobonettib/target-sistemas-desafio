namespace DesafioTarget.Domain.ValueObjects;

public sealed record InterestResult(
    decimal OriginalAmount,
    DateOnly DueDate,
    DateOnly ReferenceDate,
    int OverdueDays,
    decimal InterestAmount,
    decimal TotalAmount);
