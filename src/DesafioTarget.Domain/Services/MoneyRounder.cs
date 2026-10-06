namespace DesafioTarget.Domain.Services;

public static class MoneyRounder
{
    public static decimal Round(decimal value)
    {
        // Evita arredondamento bancário e mantém o comportamento esperado para dinheiro.
        return decimal.Round(value, 2, MidpointRounding.AwayFromZero);
    }
}
