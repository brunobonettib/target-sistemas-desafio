using DesafioTarget.Application.Abstractions;
using DesafioTarget.Domain.Exceptions;

namespace DesafioTarget.Infrastructure.Repositories;

public sealed class InMemoryStockMovementRepository : IStockMovementRepository
{
    private readonly HashSet<int> _registeredIdentifiers = [];

    public bool IdentifierExists(int identifier)
    {
        return _registeredIdentifiers.Contains(identifier);
    }

    public void RegisterIdentifier(int identifier)
    {
        if (!_registeredIdentifiers.Add(identifier))
        {
            throw new BusinessRuleException("Já existe uma movimentação com esse identificador.");
        }
    }
}
