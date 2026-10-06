using DesafioTarget.Domain.Enums;

namespace DesafioTarget.Application.Requests;

public sealed record RegisterStockMovementRequest(
    int Identifier,
    string Description,
    int ProductCode,
    StockMovementType MovementType,
    int Quantity);
