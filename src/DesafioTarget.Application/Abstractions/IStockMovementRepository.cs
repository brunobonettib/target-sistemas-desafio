namespace DesafioTarget.Application.Abstractions;

public interface IStockMovementRepository
{
    bool IdentifierExists(int identifier);
    void RegisterIdentifier(int identifier);
}
