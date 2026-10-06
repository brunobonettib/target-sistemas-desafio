namespace DesafioTarget.Domain.ValueObjects;

public sealed record CommissionSummary(
    string Seller,
    int SalesQuantity,
    decimal TotalSold,
    decimal TotalCommission);
