# Projeto ETL em F#

Este projeto implementa um sistema ETL (Extract, Transform, Load) simples em F# para processar dados de pedidos e itens de pedidos a partir de arquivos CSV.

## O que foi implementado

### Estrutura do Projeto
- **EtlApp**: Aplicação principal com módulos Extract, Load e Program.
- **EtlCore**: Biblioteca com tipos de dados e lógica de transformação.
- **EtlTests**: Testes unitários usando xUnit.

### Funcionalidades Implementadas
- **Extração (Extract)**:
  - Carregamento de pedidos (`orders.csv`) e itens de pedidos (`order_items.csv`) de arquivos locais.
  - Suporte opcional para carregamento via URL (funções `loadOrdersFromUrl` e `loadOrderItemsFromUrl`).
  - Parsing de linhas CSV para estruturas de dados F#.

- **Transformação (Transform)**:
  - Filtragem de pedidos por status e origem.
  - Junção de pedidos com itens relacionados.
  - Agregação de totais por pedido (receita total e impostos).
  - Cálculo de médias mensais de receita e impostos.
  - Formatação de dados para saída CSV.

- **Carregamento (Load)**:
  - Escrita de resumos de pedidos em `output_summary.csv`.
  - Escrita de resumos mensais em `output_monthly.csv`.

- **Tipos de Dados**:
  - `Order`: Representa um pedido com ID, cliente, data, status e origem.
  - `OrderItem`: Representa um item de pedido com ID do pedido, produto, quantidade, preço e taxa.
  - `OrderSummary`: Resumo por pedido com total de receita e impostos.
  - `MonthlySummary`: Médias mensais de receita e impostos.

- **Testes Unitários**:
  - Testes para parsing de CSV.
  - Testes para filtragem, junção e agregação.
  - Testes para formatação de saída.

### Como Executar
1. Certifique-se de ter o .NET SDK instalado (versão 8.0 ou superior).
2. Navegue para o diretório `EtlSolution`.
3. Execute `dotnet build` para compilar o projeto.
4. Execute `dotnet run --project EtlApp` para rodar a aplicação.
   - Parâmetros opcionais: status e origem para filtrar (padrão: "Complete" e "O").

### Dependências
- .NET 8.0+
- F# 8.0+
- xUnit para testes

## O que não foi implementado

- Nenhuma funcionalidade adicional pendente.

## Melhorias Implementadas

- **Correção de List.filter**: Substituído por implementação recursiva para evitar NullReferenceException.
- **Tratamento de Erros**: Try-catch em carregamento de arquivos, parsing e requisições HTTP.
- **Validação de Dados**: Parsing retorna Option, com validações para IDs positivos, quantidades >0, preços >=0, taxas entre 0 e 1.
- **Persistência em Banco de Dados**: Adicionado módulo Database com salvamento em SQLite (tabelas para Orders, OrderItems, OrderSummaries, MonthlySummaries).
- **Performance Otimizada**: Otimizado para usar Seq em operações de junção e agregação para melhor performance com grandes volumes.
- **Interface Gráfica**: Adicionada interface interativa no console para seleção de filtros quando não fornecidos via argumentos.
- **Containerização**: Dockerfile adicionado para deploy via Docker.
- **Configuração Flexível**: Suporte a arquivos de configuração (appsettings.json), variáveis de ambiente e argumentos de linha de comando.
- **Logging**: Sistema de logging avançado usando Microsoft.Extensions.Logging com saída para console.
- **Integração Contínua**: Workflow GitHub Actions para build e testes automatizados.