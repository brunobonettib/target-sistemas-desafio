using DesafioTarget.Application.Abstractions;
using DesafioTarget.Application.Requests;
using DesafioTarget.Domain.Exceptions;
using DesafioTarget.Domain.ValueObjects;

namespace DesafioTarget.Application.UseCases;

public sealed class RegisterStockMovementUseCase
{
    private readonly IStockRepository _stockRepository;
    private readonly IStockMovementRepository _movementRepository;

    public RegisterStockMovementUseCase(
        IStockRepository stockRepository,
        IStockMovementRepository movementRepository)
    {
        _stockRepository = stockRepository;
        _movementRepository = movementRepository;
    }

    public StockMovementResult Execute(RegisterStockMovementRequest request)
    {
        ValidateRequest(request);

        var product = _stockRepository.GetByCode(request.ProductCode)
            ?? throw new BusinessRuleException("Produto não encontrado no estoque.");

        product.ApplyMovement(request.MovementType, request.Quantity);

        // O identificador só é consumido depois de toda validação da movimentação.
        _movementRepository.RegisterIdentifier(request.Identifier);
        _stockRepository.Update(product);

        return new StockMovementResult(
            request.Identifier,
            request.Description.Trim(),
            product.Code,
            product.Description,
            request.MovementType,
            request.Quantity,
            product.Quantity);
    }

    private void ValidateRequest(RegisterStockMovementRequest request)
    {
        if (request.Identifier <= 0)
        {
            throw new BusinessRuleException("O identificador da movimentação deve ser maior que zero.");
        }

        if (string.IsNullOrWhiteSpace(request.Description))
        {
            throw new BusinessRuleException("Informe uma descrição para a movimentação.");
        }

        if (_movementRepository.IdentifierExists(request.Identifier))
        {
            throw new BusinessRuleException("Já existe uma movimentação com esse identificador.");
        }
    }
}
