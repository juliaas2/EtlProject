# Projeto ETL em F#

Este projeto implementa um sistema ETL (Extract, Transform, Load) completo em F# para processar dados de pedidos e itens de pedidos a partir de arquivos CSV, com suporte a carregamento via URL, transformação avançada, persistência em banco de dados SQLite, configuração flexível, logging estruturado e integração contínua.

## Resumo das Implementações

O projeto foi desenvolvido de forma completa, abrangendo todas as funcionalidades essenciais de um sistema ETL moderno:

- **Arquitetura Modular**: Separação clara entre extração, transformação, carregamento e testes.
- **Extração Robusta**: Suporte a múltiplas fontes (CSV local/URL) com tratamento de erros.
- **Transformação Eficiente**: Uso de sequências preguiçosas para performance otimizada.
- **Carregamento Flexível**: Saída para CSV e banco de dados relacional.
- **Qualidade de Código**: Testes unitários abrangentes, validação de dados e logging avançado.
- **Configuração e Deploy**: Suporte a configurações externas, containerização e CI/CD.

Todas as funcionalidades listadas foram implementadas e validadas através de testes automatizados.

## O que foi implementado

### Estrutura do Projeto
- **EtlApp**: Aplicação principal com módulos Extract, Load, Database e Program.
- **EtlCore**: Biblioteca com tipos de dados e lógica de transformação.
- **EtlTests**: Testes unitários usando xUnit.

### Funcionalidades Implementadas
- **Extração (Extract)**:
  - Carregamento de pedidos (`orders.csv`) e itens de pedidos (`order_items.csv`) de arquivos locais.
  - Suporte opcional para carregamento via URL (funções `loadOrdersFromUrl` e `loadOrderItemsFromUrl`).
  - Parsing de linhas CSV para estruturas de dados F# com validação.

- **Transformação (Transform)**:
  - Filtragem de pedidos por status e origem (implementação recursiva para evitar NullReferenceException).
  - Junção de pedidos com itens relacionados usando sequências preguiçosas.
  - Agregação de totais por pedido (receita total e impostos).
  - Cálculo de médias mensais de receita e impostos.
  - Formatação de dados para saída CSV.

- **Carregamento (Load)**:
  - Escrita de resumos de pedidos em `output_summary.csv`.
  - Escrita de resumos mensais em `output_monthly.csv`.
  - Persistência em banco de dados SQLite com tabelas estruturadas.

- **Tipos de Dados**:
  - `Order`: Representa um pedido com ID, cliente, data, status e origem.
  - `OrderItem`: Representa um item de pedido com ID do pedido, produto, quantidade, preço e taxa.
  - `OrderSummary`: Resumo por pedido com total de receita e impostos.
  - `MonthlySummary`: Médias mensais de receita e impostos.

- **Testes Unitários**:
  - Testes para parsing de CSV com validação.
  - Testes para filtragem, junção e agregação.
  - Testes para formatação de saída.
  - Cobertura completa das funções principais (20 testes passando).

### Como Executar
1. Certifique-se de ter o .NET SDK instalado (versão 9.0 ou superior).
2. Navegue para o diretório `EtlSolution`.
3. Execute `dotnet build` para compilar o projeto.
4. Execute `dotnet run --project EtlApp` para rodar a aplicação.
   - Parâmetros opcionais: `--status Complete --origin O` ou via `appsettings.json`.
   - Modo interativo: Execute sem argumentos para entrada via console.

### Dependências
- .NET 9.0+
- F# 9.0+
- Microsoft.Data.Sqlite
- Microsoft.Extensions.Configuration.Json
- Microsoft.Extensions.Logging
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