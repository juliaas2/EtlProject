/// Testes completos das funções puras de EtlCore.
module Tests

open Xunit
open Types
open Transform

// ── Dados de teste ────────────────────────────────────────────────────────────

let sampleOrders = [
    { Id = 1; ClientId = 101; OrderDate = "2024-01-15T00:00:00"; Status = "Complete"; Origin = "O" }
    { Id = 2; ClientId = 102; OrderDate = "2024-01-20T00:00:00"; Status = "Pending";  Origin = "P" }
    { Id = 3; ClientId = 103; OrderDate = "2024-02-05T00:00:00"; Status = "Complete"; Origin = "O" }
    { Id = 4; ClientId = 104; OrderDate = "2024-02-10T00:00:00"; Status = "Cancelled";Origin = "P" }
    { Id = 5; ClientId = 105; OrderDate = "2024-02-18T00:00:00"; Status = "Complete"; Origin = "P" }
]

let sampleItems = [
    { OrderId = 1; ProductId = 201; Quantity = 2; Price = 100.0; Tax = 0.10 }
    { OrderId = 1; ProductId = 202; Quantity = 1; Price = 200.0; Tax = 0.05 }
    { OrderId = 2; ProductId = 203; Quantity = 3; Price =  50.0; Tax = 0.12 }
    { OrderId = 3; ProductId = 204; Quantity = 4; Price =  75.0; Tax = 0.08 }
    { OrderId = 5; ProductId = 205; Quantity = 1; Price = 300.0; Tax = 0.15 }
]

// ── Helper Functions ──────────────────────────────────────────────────────────

[<Fact>]
let ``splitCsvLine divide linha por virgula`` () =
    let result = splitCsvLine "1,101,2024-01-15,Complete,O"
    Assert.Equal(5, result.Length)
    Assert.Equal("1", result.[0])
    Assert.Equal("O", result.[4])

[<Fact>]
let ``parseOrder carrega campos corretamente`` () =
    let fields = [| "1"; "101"; "2024-01-15"; "Complete"; "O" |]
    let order  = parseOrder fields
    Assert.Equal(1,          order.Id)
    Assert.Equal(101,        order.ClientId)
    Assert.Equal("Complete", order.Status)
    Assert.Equal("O",        order.Origin)

[<Fact>]
let ``parseOrderItem carrega campos corretamente`` () =
    let fields = [| "1"; "201"; "2"; "100.0"; "0.10" |]
    let item   = parseOrderItem fields
    Assert.Equal(1,     item.OrderId)
    Assert.Equal(2,     item.Quantity)
    Assert.Equal(100.0, item.Price)
    Assert.Equal(0.10,  item.Tax)

// ── filterOrders ──────────────────────────────────────────────────────────────

[<Fact>]
let ``filterOrders retorna apenas pedidos com status e origin corretos`` () =
    let result = filterOrders "Complete" "O" sampleOrders
    Assert.Equal(2, List.length result)
    Assert.True(result |> List.forall (fun o -> o.Status = "Complete" && o.Origin = "O"))

[<Fact>]
let ``filterOrders retorna lista vazia quando nenhum pedido bate`` () =
    let result = filterOrders "Complete" "X" sampleOrders
    Assert.Empty(result)

[<Fact>]
let ``filterOrders e case-sensitive`` () =
    let result = filterOrders "complete" "O" sampleOrders
    Assert.Empty(result)

// ── itemRevenue / itemTax ─────────────────────────────────────────────────────

[<Fact>]
let ``itemRevenue calcula price vezes quantity`` () =
    let item = { OrderId = 1; ProductId = 1; Quantity = 3; Price = 50.0; Tax = 0.10 }
    Assert.Equal(150.0, itemRevenue item)

[<Fact>]
let ``itemTax calcula tax vezes receita`` () =
    let item = { OrderId = 1; ProductId = 1; Quantity = 2; Price = 100.0; Tax = 0.10 }
    Assert.Equal(20.0, itemTax item)

[<Fact>]
let ``itemTax com tax zero retorna zero`` () =
    let item = { OrderId = 1; ProductId = 1; Quantity = 5; Price = 200.0; Tax = 0.0 }
    Assert.Equal(0.0, itemTax item)

// ── joinOrdersWithItems ───────────────────────────────────────────────────────

[<Fact>]
let ``joinOrdersWithItems retorna apenas itens dos pedidos filtrados`` () =
    let filtered = filterOrders "Complete" "O" sampleOrders  // ids 1 e 3
    let joined   = joinOrdersWithItems filtered sampleItems
    let orderIds = joined |> List.map (fun (_, i) -> i.OrderId) |> List.distinct |> List.sort
    Assert.Equal<int list>([1; 3], orderIds)

[<Fact>]
let ``joinOrdersWithItems retorna lista vazia se nenhum item bate`` () =
    let orders = [ { Id = 99; ClientId = 1; OrderDate = "2024-01-01"; Status = "Complete"; Origin = "O" } ]
    let joined = joinOrdersWithItems orders sampleItems
    Assert.Empty(joined)

// ── aggregateByOrder ──────────────────────────────────────────────────────────

[<Fact>]
let ``aggregateByOrder soma receita e impostos corretamente`` () =
    // order 1: item (qty=2, price=100, tax=0.10) + item (qty=1, price=200, tax=0.05)
    // total_amount = 200 + 200 = 400 | total_taxes = 20 + 10 = 30
    let filtered = filterOrders "Complete" "O" sampleOrders
    let joined   = joinOrdersWithItems filtered sampleItems
    let result   = aggregateByOrder joined
    let order1   = result |> List.find (fun s -> s.OrderId = 1)
    Assert.Equal(400.0, order1.TotalAmount)
    Assert.Equal(30.0,  order1.TotalTaxes)

[<Fact>]
let ``aggregateByOrder retorna um registro por pedido`` () =
    let filtered = filterOrders "Complete" "O" sampleOrders
    let joined   = joinOrdersWithItems filtered sampleItems
    let result   = aggregateByOrder joined
    let ids      = result |> List.map (fun s -> s.OrderId)
    Assert.Equal(ids, ids |> List.distinct)

// ── transform (pipeline) ──────────────────────────────────────────────────────

[<Fact>]
let ``transform retorna lista vazia para filtro sem resultados`` () =
    let result = transform "Complete" "X" sampleOrders sampleItems
    Assert.Empty(result)

[<Fact>]
let ``transform retorna summaries apenas dos pedidos filtrados`` () =
    let result  = transform "Complete" "O" sampleOrders sampleItems
    let ids     = result |> List.map (fun s -> s.OrderId) |> List.sort
    Assert.Equal<int list>([1; 3], ids)

// ── parseYearMonth ────────────────────────────────────────────────────────────

[<Fact>]
let ``parseYearMonth extrai ano e mes de data simples`` () =
    Assert.Equal(Some (2024, 3), parseYearMonth "2024-03-15")

[<Fact>]
let ``parseYearMonth extrai ano e mes de datetime com T`` () =
    Assert.Equal(Some (2024, 8), parseYearMonth "2024-08-17T03:05:39")

[<Fact>]
let ``parseYearMonth retorna None para string invalida`` () =
    Assert.Equal(None, parseYearMonth "invalido")

// ── Serialização CSV ──────────────────────────────────────────────────────────

[<Fact>]
let ``summaryToCsvLine formata corretamente`` () =
    let s = { OrderId = 5; TotalAmount = 1234.56; TotalTaxes = 98.76 }
    Assert.Equal("5,1234.56,98.76", summaryToCsvLine s)

[<Fact>]
let ``monthlySummaryToCsvLine formata mes com zero a esquerda`` () =
    let m = { Year = 2024; Month = 3; AvgAmount = 500.0; AvgTaxes = 50.0 }
    Assert.Equal("2024,03,500.00,50.00", monthlySummaryToCsvLine m)
