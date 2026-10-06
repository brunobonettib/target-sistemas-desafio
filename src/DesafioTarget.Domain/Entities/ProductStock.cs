using DesafioTarget.Domain.Enums;
using DesafioTarget.Domain.Exceptions;

namespace DesafioTarget.Domain.Entities;

public sealed class ProductStock
{
    public ProductStock(int code, string description, int quantity)
    {
        Code = code;
        Description = description;
        Quantity = quantity;
    }

    public int Code { get; }
    public string Description { get; }
    public int Quantity { get; private set; }

    public void ApplyMovement(StockMovementType movementType, int quantity)
    {
        if (quantity <= 0)
        {
            throw new BusinessRuleException("A quantidade movimentada deve ser maior que zero.");
        }

        if (!Enum.IsDefined(movementType))
        {
            throw new BusinessRuleException("Tipo de movimentação inválido.");
        }

        if (movementType == StockMovementType.Outbound && Quantity < quantity)
        {
            throw new BusinessRuleException("A saída não pode deixar o estoque negativo.");
        }

        Quantity += movementType == StockMovementType.Inbound ? quantity : -quantity;
    }

    public ProductStock Clone()
    {
        return new ProductStock(Code, Description, Quantity);
    }
}
