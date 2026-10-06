using System.Globalization;
using System.Text;
using System.Text.Json;
using DesafioTarget.Models;
using DesafioTarget.Services;

Console.OutputEncoding = Encoding.UTF8;
CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("pt-BR");
CultureInfo.DefaultThreadCurrentUICulture = new CultureInfo("pt-BR");

var vendasData = CarregarJson<VendasData>("vendas.json");
var estoqueData = CarregarJson<EstoqueData>("estoque.json");
var estoqueService = new EstoqueService(estoqueData.Estoque);

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

    var opcao = Console.ReadLine();
    Console.WriteLine();

    try
    {
        switch (opcao)
        {
            case "1":
                ExibirComissoes(vendasData.Vendas);
                break;
            case "2":
                LancarMovimentacaoInterativa(estoqueService);
                break;
            case "3":
                CalcularJurosInterativo();
                break;
            case "4":
                ExecutarDemonstracao(vendasData.Vendas, estoqueData.Estoque);
                break;
            case "5":
                ValidarRegrasPrincipais();
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

static void ExibirComissoes(IEnumerable<Venda> vendas)
{
    var resultados = ComissaoService.CalcularPorVendedor(vendas);

    Console.WriteLine("Comissões por vendedor");
    Console.WriteLine("---------------------------------------------------------------");
    Console.WriteLine($"{"Vendedor",-20} {"Vendas",6} {"Total vendido",16} {"Comissão",14}");
    Console.WriteLine("---------------------------------------------------------------");

    foreach (var resultado in resultados)
    {
        Console.WriteLine(
            $"{resultado.Vendedor,-20} {resultado.QuantidadeVendas,6} " +
            $"{FormatarMoeda(resultado.TotalVendido),16} {FormatarMoeda(resultado.TotalComissao),14}");
    }
}

static void LancarMovimentacaoInterativa(EstoqueService estoqueService)
{
    Console.WriteLine("Produtos disponíveis");
    foreach (var produto in estoqueService.ListarProdutos())
    {
        Console.WriteLine($"{produto.CodigoProduto} - {produto.DescricaoProduto} | Estoque atual: {produto.Estoque}");
    }

    var identificador = LerInteiro("Identificador único da movimentação: ");
    Console.Write("Descrição da movimentação: ");
    var descricao = Console.ReadLine() ?? string.Empty;
    var codigoProduto = LerInteiro("Código do produto: ");
    var tipo = LerTipoMovimentacao();
    var quantidade = LerInteiro("Quantidade: ");

    var movimentacao = estoqueService.LancarMovimentacao(
        identificador,
        descricao,
        codigoProduto,
        tipo,
        quantidade);

    ExibirMovimentacao(movimentacao);
}

static void CalcularJurosInterativo()
{
    var valor = LerDecimal("Valor do título: ");
    var dataVencimento = LerData("Data de vencimento (dd/MM/yyyy): ");
    var resultado = JurosService.Calcular(valor, dataVencimento, DateOnly.FromDateTime(DateTime.Today));

    ExibirJuros(resultado);
}

static void ExecutarDemonstracao(IEnumerable<Venda> vendas, IEnumerable<ProdutoEstoque> produtos)
{
    Console.WriteLine("1) Resultado das comissões");
    ExibirComissoes(vendas);

    Console.WriteLine();
    Console.WriteLine("2) Demonstração de movimentações de estoque");
    var estoqueDemonstracao = new EstoqueService(produtos);
    ExibirMovimentacao(estoqueDemonstracao.LancarMovimentacao(
        1,
        "Entrada por compra de reposição",
        101,
        TipoMovimentacao.Entrada,
        25));
    ExibirMovimentacao(estoqueDemonstracao.LancarMovimentacao(
        2,
        "Saída por venda ao cliente",
        102,
        TipoMovimentacao.Saida,
        10));

    Console.WriteLine();
    Console.WriteLine("3) Demonstração de juros");
    var hoje = DateOnly.FromDateTime(DateTime.Today);
    var vencimento = hoje.AddDays(-10);
    ExibirJuros(JurosService.Calcular(1000m, vencimento, hoje));
}

static void ValidarRegrasPrincipais()
{
    // Validação simples para facilitar a conferência das regras sem depender de testes externos.
    Garantir(ComissaoService.CalcularComissaoDaVenda(99.99m) == 0m, "Venda abaixo de R$100,00 não deve gerar comissão.");
    Garantir(ComissaoService.CalcularComissaoDaVenda(100m) == 1m, "Venda de R$100,00 deve gerar 1% de comissão.");
    Garantir(ComissaoService.CalcularComissaoDaVenda(100.50m) == 1.01m, "Comissão deve arredondar meio centavo para cima.");
    Garantir(ComissaoService.CalcularComissaoDaVenda(499.99m) == 5m, "Venda abaixo de R$500,00 deve gerar 1% de comissão.");
    Garantir(ComissaoService.CalcularComissaoDaVenda(500m) == 25m, "Venda de R$500,00 deve gerar 5% de comissão.");

    var vendas = CarregarJson<VendasData>("vendas.json").Vendas;
    var comissoes = ComissaoService.CalcularPorVendedor(vendas).ToDictionary(item => item.Vendedor);
    Garantir(comissoes["Ana Lima"].TotalComissao == 404.99m, "Comissão total da Ana Lima divergente.");
    Garantir(comissoes["Carlos Oliveira"].TotalComissao == 379.38m, "Comissão total do Carlos Oliveira divergente.");
    Garantir(comissoes["João Silva"].TotalComissao == 495.69m, "Comissão total do João Silva divergente.");
    Garantir(comissoes["Maria Souza"].TotalComissao == 465.96m, "Comissão total da Maria Souza divergente.");

    var estoque = new EstoqueService([
        new ProdutoEstoque { CodigoProduto = 1, DescricaoProduto = "Produto teste", Estoque = 10 }
    ]);

    var entrada = estoque.LancarMovimentacao(1, "Entrada teste", 1, TipoMovimentacao.Entrada, 5);
    Garantir(entrada.EstoqueFinal == 15, "Entrada deve somar ao estoque.");

    var saida = estoque.LancarMovimentacao(2, "Saída teste", 1, TipoMovimentacao.Saida, 3);
    Garantir(saida.EstoqueFinal == 12, "Saída deve subtrair do estoque.");

    GarantirErro<InvalidOperationException>(() => estoque.LancarMovimentacao(2, "ID repetido", 1, TipoMovimentacao.Entrada, 1), "Identificador repetido deve ser bloqueado.");
    GarantirErro<InvalidOperationException>(() => estoque.LancarMovimentacao(3, "Saída maior que estoque", 1, TipoMovimentacao.Saida, 99), "Saída maior que estoque deve ser bloqueada.");
    GarantirErro<ArgumentException>(() => estoque.LancarMovimentacao(4, " ", 1, TipoMovimentacao.Entrada, 1), "Descrição vazia deve ser bloqueada.");
    GarantirErro<KeyNotFoundException>(() => estoque.LancarMovimentacao(5, "Produto inexistente", 999, TipoMovimentacao.Entrada, 1), "Produto inexistente deve ser bloqueado.");
    var idReutilizadoDepoisDeErro = estoque.LancarMovimentacao(4, "Entrada após erro de descrição", 1, TipoMovimentacao.Entrada, 1);
    Garantir(idReutilizadoDepoisDeErro.EstoqueFinal == 13, "Identificador de movimentação inválida não deve ser consumido.");

    var juros = JurosService.Calcular(200m, new DateOnly(2026, 1, 1), new DateOnly(2026, 1, 3));
    Garantir(juros.DiasAtraso == 2 && juros.ValorJuros == 10m && juros.ValorTotal == 210m, "Juros deve ser 2,5% ao dia.");
    var semAtraso = JurosService.Calcular(200m, new DateOnly(2026, 1, 10), new DateOnly(2026, 1, 3));
    Garantir(semAtraso.DiasAtraso == 0 && semAtraso.ValorJuros == 0m && semAtraso.ValorTotal == 200m, "Título sem atraso não deve gerar juros.");
    GarantirErro<ArgumentException>(() => JurosService.Calcular(-1m, new DateOnly(2026, 1, 1), new DateOnly(2026, 1, 3)), "Valor negativo deve ser bloqueado.");

    Console.WriteLine("Validações concluídas com sucesso.");
}

static T CarregarJson<T>(string arquivo)
{
    var caminho = Path.Combine(AppContext.BaseDirectory, "Data", arquivo);

    if (!File.Exists(caminho))
    {
        caminho = Path.Combine("Data", arquivo);
    }

    var conteudo = File.ReadAllText(caminho, Encoding.UTF8);
    return JsonSerializer.Deserialize<T>(conteudo, new JsonSerializerOptions
    {
        PropertyNameCaseInsensitive = true
    }) ?? throw new InvalidOperationException($"Não foi possível carregar o arquivo {arquivo}.");
}

static int LerInteiro(string mensagem)
{
    while (true)
    {
        Console.Write(mensagem);
        if (int.TryParse(Console.ReadLine(), out var valor))
        {
            return valor;
        }

        Console.WriteLine("Informe um número inteiro válido.");
    }
}

static decimal LerDecimal(string mensagem)
{
    while (true)
    {
        Console.Write(mensagem);
        var entrada = Console.ReadLine();

        if (decimal.TryParse(entrada, NumberStyles.Number, CultureInfo.CurrentCulture, out var valor) ||
            decimal.TryParse(entrada, NumberStyles.Number, CultureInfo.InvariantCulture, out valor))
        {
            return valor;
        }

        Console.WriteLine("Informe um valor numérico válido.");
    }
}

static DateOnly LerData(string mensagem)
{
    while (true)
    {
        Console.Write(mensagem);
        if (DateOnly.TryParseExact(Console.ReadLine(), "dd/MM/yyyy", CultureInfo.CurrentCulture, DateTimeStyles.None, out var data))
        {
            return data;
        }

        Console.WriteLine("Informe uma data válida no formato dd/MM/yyyy.");
    }
}

static TipoMovimentacao LerTipoMovimentacao()
{
    while (true)
    {
        Console.Write("Tipo da movimentação (1 - Entrada, 2 - Saída): ");
        var entrada = Console.ReadLine();

        if (entrada == "1")
        {
            return TipoMovimentacao.Entrada;
        }

        if (entrada == "2")
        {
            return TipoMovimentacao.Saida;
        }

        Console.WriteLine("Informe 1 para entrada ou 2 para saída.");
    }
}

static void ExibirMovimentacao(MovimentacaoEstoque movimentacao)
{
    Console.WriteLine("Movimentação registrada");
    Console.WriteLine($"Identificador: {movimentacao.Identificador}");
    Console.WriteLine($"Descrição: {movimentacao.Descricao}");
    Console.WriteLine($"Produto: {movimentacao.CodigoProduto} - {movimentacao.DescricaoProduto}");
    Console.WriteLine($"Tipo: {FormatarTipoMovimentacao(movimentacao.Tipo)}");
    Console.WriteLine($"Quantidade: {movimentacao.Quantidade}");
    Console.WriteLine($"Estoque final: {movimentacao.EstoqueFinal}");
}

static void ExibirJuros(JurosCalculado resultado)
{
    Console.WriteLine($"Valor original: {FormatarMoeda(resultado.ValorOriginal)}");
    Console.WriteLine($"Data de vencimento: {resultado.DataVencimento:dd/MM/yyyy}");
    Console.WriteLine($"Data de referência: {resultado.DataReferencia:dd/MM/yyyy}");
    Console.WriteLine($"Dias em atraso: {resultado.DiasAtraso}");
    Console.WriteLine($"Valor dos juros: {FormatarMoeda(resultado.ValorJuros)}");
    Console.WriteLine($"Valor total: {FormatarMoeda(resultado.ValorTotal)}");
}

static string FormatarMoeda(decimal valor)
{
    return valor.ToString("C", CultureInfo.CurrentCulture);
}

static string FormatarTipoMovimentacao(TipoMovimentacao tipo)
{
    return tipo == TipoMovimentacao.Entrada ? "Entrada" : "Saída";
}

static void Garantir(bool condicao, string mensagem)
{
    if (!condicao)
    {
        throw new InvalidOperationException(mensagem);
    }
}

static void GarantirErro<TException>(Action acao, string mensagem)
    where TException : Exception
{
    try
    {
        acao();
    }
    catch (TException)
    {
        return;
    }

    throw new InvalidOperationException(mensagem);
}
