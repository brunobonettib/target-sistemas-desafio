using DesafioTarget.Application.Abstractions;
using DesafioTarget.Application.Requests;
using DesafioTarget.Domain.Entities;
using DesafioTarget.Domain.Enums;
using DesafioTarget.Domain.Exceptions;
using DesafioTarget.Domain.Services;

namespace DesafioTarget.Application.UseCases;

public sealed class ValidateBusinessRulesUseCase
{
    private readonly ISalesRepository _salesRepository;

    public ValidateBusinessRulesUseCase(ISalesRepository salesRepository)
    {
        _salesRepository = salesRepository;
    }

    public void Execute()
    {
        ValidateCommissions();
        ValidateStockMovements();
        ValidateInterest();
    }

    private void ValidateCommissions()
    {
        Ensure(CommissionCalculator.CalculateSaleCommission(99.99m) == 0m, "Venda abaixo de R$100,00 não deve gerar comissão.");
        Ensure(CommissionCalculator.CalculateSaleCommission(100m) == 1m, "Venda de R$100,00 deve gerar 1% de comissão.");
        Ensure(CommissionCalculator.CalculateSaleCommission(100.50m) == 1.01m, "Comissão deve arredondar meio centavo para cima.");
        Ensure(CommissionCalculator.CalculateSaleCommission(499.99m) == 5m, "Venda abaixo de R$500,00 deve gerar 1% de comissão.");
        Ensure(CommissionCalculator.CalculateSaleCommission(500m) == 25m, "Venda de R$500,00 deve gerar 5% de comissão.");

        var commissions = CommissionCalculator
            .SummarizeBySeller(_salesRepository.GetSales())
            .ToDictionary(item => item.Seller);

        Ensure(commissions["Ana Lima"].TotalCommission == 404.99m, "Comissão total da Ana Lima divergente.");
        Ensure(commissions["Carlos Oliveira"].TotalCommission == 379.38m, "Comissão total do Carlos Oliveira divergente.");
        Ensure(commissions["João Silva"].TotalCommission == 495.69m, "Comissão total do João Silva divergente.");
        Ensure(commissions["Maria Souza"].TotalCommission == 465.96m, "Comissão total da Maria Souza divergente.");
    }

    private static void ValidateStockMovements()
    {
        var stockRepository = new ValidationStockRepository([
            new ProductStock(1, "Produto teste", 10)
        ]);
        var movementRepository = new ValidationStockMovementRepository();
        var useCase = new RegisterStockMovementUseCase(stockRepository, movementRepository);

        var inbound = useCase.Execute(new RegisterStockMovementRequest(1, "Entrada teste", 1, StockMovementType.Inbound, 5));
        Ensure(inbound.FinalStock == 15, "Entrada deve somar ao estoque.");

        var outbound = useCase.Execute(new RegisterStockMovementRequest(2, "Saída teste", 1, StockMovementType.Outbound, 3));
        Ensure(outbound.FinalStock == 12, "Saída deve subtrair do estoque.");

        EnsureError<BusinessRuleException>(() => useCase.Execute(new RegisterStockMovementRequest(2, "ID repetido", 1, StockMovementType.Inbound, 1)), "Identificador repetido deve ser bloqueado.");
        EnsureError<BusinessRuleException>(() => useCase.Execute(new RegisterStockMovementRequest(3, "Saída maior que estoque", 1, StockMovementType.Outbound, 99)), "Saída maior que estoque deve ser bloqueada.");
        EnsureError<BusinessRuleException>(() => useCase.Execute(new RegisterStockMovementRequest(4, " ", 1, StockMovementType.Inbound, 1)), "Descrição vazia deve ser bloqueada.");
        EnsureError<BusinessRuleException>(() => useCase.Execute(new RegisterStockMovementRequest(5, "Produto inexistente", 999, StockMovementType.Inbound, 1)), "Produto inexistente deve ser bloqueado.");
        EnsureError<BusinessRuleException>(() => useCase.Execute(new RegisterStockMovementRequest(6, "Quantidade inválida", 1, StockMovementType.Inbound, 0)), "Quantidade zerada deve ser bloqueada.");

        var reusedIdentifier = useCase.Execute(new RegisterStockMovementRequest(4, "Entrada após erro", 1, StockMovementType.Inbound, 1));
        Ensure(reusedIdentifier.FinalStock == 13, "Identificador de movimentação inválida não deve ser consumido.");
    }

    private static void ValidateInterest()
    {
        var interest = InterestCalculator.Calculate(200m, new DateOnly(2026, 1, 1), new DateOnly(2026, 1, 3));
        Ensure(interest.OverdueDays == 2 && interest.InterestAmount == 10m && interest.TotalAmount == 210m, "Juros deve ser 2,5% ao dia.");

        var notOverdue = InterestCalculator.Calculate(200m, new DateOnly(2026, 1, 10), new DateOnly(2026, 1, 3));
        Ensure(notOverdue.OverdueDays == 0 && notOverdue.InterestAmount == 0m && notOverdue.TotalAmount == 200m, "Título sem atraso não deve gerar juros.");
        EnsureError<BusinessRuleException>(() => InterestCalculator.Calculate(-1m, new DateOnly(2026, 1, 1), new DateOnly(2026, 1, 3)), "Valor negativo deve ser bloqueado.");
    }

    private static void Ensure(bool condition, string message)
    {
        if (!condition)
        {
            throw new BusinessRuleException(message);
        }
    }

    private static void EnsureError<TException>(Action action, string message)
        where TException : Exception
    {
        try
        {
            action();
        }
        catch (TException)
        {
            return;
        }

        throw new BusinessRuleException(message);
    }

    private sealed class ValidationStockRepository : IStockRepository
    {
        private readonly Dictionary<int, ProductStock> _products;

        public ValidationStockRepository(IEnumerable<ProductStock> products)
        {
            _products = products.ToDictionary(product => product.Code, product => product.Clone());
        }

        public IReadOnlyList<ProductStock> ListProducts()
        {
            return _products.Values.Select(product => product.Clone()).ToList();
        }

        public ProductStock? GetByCode(int productCode)
        {
            return _products.TryGetValue(productCode, out var product)
                ? product.Clone()
                : null;
        }

        public void Update(ProductStock product)
        {
            _products[product.Code] = product.Clone();
        }
    }

    private sealed class ValidationStockMovementRepository : IStockMovementRepository
    {
        private readonly HashSet<int> _identifiers = [];

        public bool IdentifierExists(int identifier)
        {
            return _identifiers.Contains(identifier);
        }

        public void RegisterIdentifier(int identifier)
        {
            if (!_identifiers.Add(identifier))
            {
                throw new BusinessRuleException("Já existe uma movimentação com esse identificador.");
            }
        }
    }
}
