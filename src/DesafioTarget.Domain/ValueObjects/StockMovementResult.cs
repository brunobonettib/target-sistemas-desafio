using DesafioTarget.Domain.Enums;

namespace DesafioTarget.Domain.ValueObjects;

public sealed record StockMovementResult(
    int Identifier,
    string Description,
    int ProductCode,
    string ProductDescription,
    StockMovementType MovementType,
    int Quantity,
    int FinalStock);
