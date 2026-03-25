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

- **Tratamento de Erros**: Não há tratamento robusto de erros para arquivos inexistentes, dados malformados ou falhas de rede.
- **Validação de Dados**: Não há validação adicional além do parsing básico.
- **Persistência em Banco de Dados**: Os dados são apenas processados e salvos em CSV; não há integração com bancos de dados.
- **Configuração Flexível**: Caminhos de arquivos são hardcoded; não há suporte a configuração externa.
- **Logging**: Não há sistema de logging para rastrear operações.
- **Performance Otimizada**: Para grandes volumes de dados, pode haver problemas de performance devido ao uso de listas em F#.
- **Interface Gráfica**: Apenas console-based.
- **Deploy e Empacotamento**: Não há scripts para deploy ou empacotamento.
- **Integração Contínua**: Não há configuração de CI/CD.

## Problemas Conhecidos
- Houve um problema com `List.filter` causando `NullReferenceException` em algumas versões do F#. Como workaround, foi considerado usar recursão, mas no código atual ainda usa `List.filter`. Se o erro persistir, substituir por implementação recursiva conforme documentado em notas pessoais.