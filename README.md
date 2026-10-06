# Desafio Target Sistemas

Aplicação de console em C# desenvolvida para o desafio técnico da Target Sistemas.

## O que o programa faz

- Lê um JSON de vendas e calcula a comissão de cada vendedor.
- Permite lançar entrada ou saída de estoque para os produtos do JSON.
- Calcula juros de 2,5% ao dia a partir de um valor e uma data de vencimento.

## Regras atendidas

### Comissão

- Vendas abaixo de R$ 100,00 não geram comissão.
- Vendas de R$ 100,00 até abaixo de R$ 500,00 geram 1% de comissão.
- Vendas a partir de R$ 500,00 geram 5% de comissão.

### Estoque

- Cada movimentação possui um identificador único.
- Cada movimentação possui uma descrição.
- Entradas somam ao estoque.
- Saídas subtraem do estoque.
- O sistema não permite saída maior que o estoque disponível.
- Ao final da movimentação, o estoque final do produto é exibido.

### Juros

- A multa é de 2,5% ao dia.
- Se a data de vencimento ainda não passou, os juros ficam zerados.

## Como executar

É necessário ter o .NET instalado.

```bash
dotnet run
```

Depois, escolha uma opção no menu:

```text
1 - Calcular comissão dos vendedores
2 - Lançar movimentação de estoque
3 - Calcular juros por atraso
4 - Executar demonstração completa
5 - Validar regras principais
0 - Sair
```

## Validação

Para conferir rapidamente as principais regras, execute o programa e escolha:

```text
5 - Validar regras principais
```

Saída esperada:

```text
Validações concluídas com sucesso.
```
