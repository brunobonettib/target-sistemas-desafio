using System.Globalization;
using System.Text;
using DesafioTarget.Application.Requests;
using DesafioTarget.Application.UseCases;
using DesafioTarget.Domain.Enums;
using DesafioTarget.Domain.ValueObjects;
using DesafioTarget.Infrastructure.Repositories;

Console.OutputEncoding = Encoding.UTF8;
CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("pt-BR");
CultureInfo.DefaultThreadCurrentUICulture = new CultureInfo("pt-BR");

var dataDirectory = ResolveDataDirectory();

var salesRepository = new JsonSalesRepository(dataDirectory);
var stockRepository = new JsonStockRepository(dataDirectory);
var stockMovementRepository = new InMemoryStockMovementRepository();

var calculateCommissions = new CalculateCommissionsUseCase(salesRepository);
var listStockProducts = new ListStockProductsUseCase(stockRepository);
var registerStockMovement = new RegisterStockMovementUseCase(stockRepository, stockMovementRepository);
var calculateInterest = new CalculateInterestUseCase();
var validateBusinessRules = new ValidateBusinessRulesUseCase(salesRepository);

while (true)
{
    Console.WriteLine();
    Console.WriteLine("=== Desafio Target Sistemas ===");
    Console.WriteLine("1 - Calcular comissão dos vendedores");
    Console.WriteLine("2 - Lançar movimentação de estoque");
    Console.WriteLine("3 - Calcular juros por atraso");
    Console.WriteLine("4 - Executar demonstração completa");
    Console.WriteLine("5 - Validar regras principais");
    Console.WriteLine("0 - Sair");
    Console.Write("Escolha uma opção: ");

    var option = Console.ReadLine();
    Console.WriteLine();

    try
    {
        switch (option)
        {
            case "1":
                ShowCommissions(calculateCommissions);
                break;
            case "2":
                RegisterStockMovementInteractively(listStockProducts, registerStockMovement);
                break;
            case "3":
                CalculateInterestInteractively(calculateInterest);
                break;
            case "4":
                RunDemo(dataDirectory);
                break;
            case "5":
                validateBusinessRules.Execute();
                Console.WriteLine("Validações concluídas com sucesso.");
                break;
            case "0":
                return;
            default:
                Console.WriteLine("Opção inválida.");
                break;
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Erro: {ex.Message}");
    }
}

static void ShowCommissions(CalculateCommissionsUseCase useCase)
{
    var results = useCase.Execute();

    Console.WriteLine("Comissões por vendedor");
    Console.WriteLine("---------------------------------------------------------------");
    Console.WriteLine($"{"Vendedor",-20} {"Vendas",6} {"Total vendido",16} {"Comissão",14}");
    Console.WriteLine("---------------------------------------------------------------");

    foreach (var result in results)
    {
        Console.WriteLine(
            $"{result.Seller,-20} {result.SalesQuantity,6} " +
            $"{FormatMoney(result.TotalSold),16} {FormatMoney(result.TotalCommission),14}");
    }
}

static void RegisterStockMovementInteractively(
    ListStockProductsUseCase listStockProducts,
    RegisterStockMovementUseCase registerStockMovement)
{
    Console.WriteLine("Produtos disponíveis");
    foreach (var product in listStockProducts.Execute())
    {
        Console.WriteLine($"{product.Code} - {product.Description} | Estoque atual: {product.Quantity}");
    }

    var identifier = ReadInteger("Identificador único da movimentação: ");
    Console.Write("Descrição da movimentação: ");
    var description = Console.ReadLine() ?? string.Empty;
    var productCode = ReadInteger("Código do produto: ");
    var movementType = ReadMovementType();
    var quantity = ReadInteger("Quantidade: ");

    var result = registerStockMovement.Execute(new RegisterStockMovementRequest(
        identifier,
        description,
        productCode,
        movementType,
        quantity));

    ShowStockMovement(result);
}

static void CalculateInterestInteractively(CalculateInterestUseCase useCase)
{
    var amount = ReadDecimal("Valor do título: ");
    var dueDate = ReadDate("Data de vencimento (dd/MM/yyyy): ");
    var result = useCase.Execute(amount, dueDate, DateOnly.FromDateTime(DateTime.Today));

    ShowInterest(result);
}

static void RunDemo(string dataDirectory)
{
    var salesRepository = new JsonSalesRepository(dataDirectory);
    var stockRepository = new JsonStockRepository(dataDirectory);
    var movementRepository = new InMemoryStockMovementRepository();

    Console.WriteLine("1) Resultado das comissões");
    ShowCommissions(new CalculateCommissionsUseCase(salesRepository));

    Console.WriteLine();
    Console.WriteLine("2) Demonstração de movimentações de estoque");
    var registerStockMovement = new RegisterStockMovementUseCase(stockRepository, movementRepository);
    ShowStockMovement(registerStockMovement.Execute(new RegisterStockMovementRequest(
        1,
        "Entrada por compra de reposição",
        101,
        StockMovementType.Inbound,
        25)));
    ShowStockMovement(registerStockMovement.Execute(new RegisterStockMovementRequest(
        2,
        "Saída por venda ao cliente",
        102,
        StockMovementType.Outbound,
        10)));

    Console.WriteLine();
    Console.WriteLine("3) Demonstração de juros");
    var today = DateOnly.FromDateTime(DateTime.Today);
    var dueDate = today.AddDays(-10);
    ShowInterest(new CalculateInterestUseCase().Execute(1000m, dueDate, today));
}

static string ResolveDataDirectory()
{
    var outputDataDirectory = Path.Combine(AppContext.BaseDirectory, "Data");
    if (Directory.Exists(outputDataDirectory))
    {
        return outputDataDirectory;
    }

    return Path.Combine("Data");
}

static int ReadInteger(string message)
{
    while (true)
    {
        Console.Write(message);
        if (int.TryParse(Console.ReadLine(), out var value))
        {
            return value;
        }

        Console.WriteLine("Informe um número inteiro válido.");
    }
}

static decimal ReadDecimal(string message)
{
    while (true)
    {
        Console.Write(message);
        var input = Console.ReadLine();

        if (decimal.TryParse(input, NumberStyles.Number, CultureInfo.CurrentCulture, out var value) ||
            decimal.TryParse(input, NumberStyles.Number, CultureInfo.InvariantCulture, out value))
        {
            return value;
        }

        Console.WriteLine("Informe um valor numérico válido.");
    }
}

static DateOnly ReadDate(string message)
{
    while (true)
    {
        Console.Write(message);
        if (DateOnly.TryParseExact(Console.ReadLine(), "dd/MM/yyyy", CultureInfo.CurrentCulture, DateTimeStyles.None, out var date))
        {
            return date;
        }

        Console.WriteLine("Informe uma data válida no formato dd/MM/yyyy.");
    }
}

static StockMovementType ReadMovementType()
{
    while (true)
    {
        Console.Write("Tipo da movimentação (1 - Entrada, 2 - Saída): ");
        var input = Console.ReadLine();

        if (input == "1")
        {
            return StockMovementType.Inbound;
        }

        if (input == "2")
        {
            return StockMovementType.Outbound;
        }

        Console.WriteLine("Informe 1 para entrada ou 2 para saída.");
    }
}

static void ShowStockMovement(StockMovementResult movement)
{
    Console.WriteLine("Movimentação registrada");
    Console.WriteLine($"Identificador: {movement.Identifier}");
    Console.WriteLine($"Descrição: {movement.Description}");
    Console.WriteLine($"Produto: {movement.ProductCode} - {movement.ProductDescription}");
    Console.WriteLine($"Tipo: {FormatMovementType(movement.MovementType)}");
    Console.WriteLine($"Quantidade: {movement.Quantity}");
    Console.WriteLine($"Estoque final: {movement.FinalStock}");
}

static void ShowInterest(InterestResult result)
{
    Console.WriteLine($"Valor original: {FormatMoney(result.OriginalAmount)}");
    Console.WriteLine($"Data de vencimento: {result.DueDate:dd/MM/yyyy}");
    Console.WriteLine($"Data de referência: {result.ReferenceDate:dd/MM/yyyy}");
    Console.WriteLine($"Dias em atraso: {result.OverdueDays}");
    Console.WriteLine($"Valor dos juros: {FormatMoney(result.InterestAmount)}");
    Console.WriteLine($"Valor total: {FormatMoney(result.TotalAmount)}");
}

static string FormatMoney(decimal value)
{
    return value.ToString("C", CultureInfo.CurrentCulture);
}

static string FormatMovementType(StockMovementType movementType)
{
    return movementType == StockMovementType.Inbound ? "Entrada" : "Saída";
}
