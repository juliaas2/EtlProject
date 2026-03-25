module Transform

open Types

let filterOrders (status: string) (origin: string) (orders: Order list) : Order list =
    orders
    |> List.filter (fun o -> o.Status = status && o.Origin = origin)

let itemRevenue (item: OrderItem) : float =
    item.Price * float item.Quantity

let itemTax (item: OrderItem) : float =
    item.Tax * itemRevenue item

let joinOrdersWithItems (orders: Order list) (items: OrderItem list) : (Order * OrderItem) list =
    let orderIds = orders |> List.map (fun o -> o.Id) |> Set.ofList
    items
    |> List.filter (fun i -> Set.contains i.OrderId orderIds)
    |> List.map (fun i ->
        let order = orders |> List.find (fun o -> o.Id = i.OrderId)
        (order, i))

let aggregateByOrder (pairs: (Order * OrderItem) list) : OrderSummary list =
    let grouped =
        pairs
        |> List.fold (fun (acc: Map<int, OrderItem list>) (_, item) ->
            let current = acc |> Map.tryFind item.OrderId |> Option.defaultValue []
            acc |> Map.add item.OrderId (item :: current)
        ) Map.empty
    grouped
    |> Map.toList
    |> List.map (fun (orderId, items) ->
        let totalAmount = items |> List.fold (fun acc i -> acc + itemRevenue i) 0.0
        let totalTaxes  = items |> List.fold (fun acc i -> acc + itemTax i) 0.0
        { OrderId     = orderId
          TotalAmount = System.Math.Round(totalAmount, 2)
          TotalTaxes  = System.Math.Round(totalTaxes, 2) })
    |> List.sortBy (fun s -> s.OrderId)

let transform (status: string) (origin: string) (orders: Order list) (items: OrderItem list) : OrderSummary list =
    orders
    |> filterOrders status origin
    |> fun filtered -> joinOrdersWithItems filtered items
    |> aggregateByOrder

let parseYearMonth (dateStr: string) : (int * int) option =
    let datePart = dateStr.Split('T').[0]
    let parts    = datePart.Split('-')
    if parts.Length >= 2 then Some (int parts.[0], int parts.[1])
    else None

let monthlyAverages (status: string) (origin: string) (orders: Order list) (items: OrderItem list) : MonthlySummary list =
    let filteredOrders = filterOrders status origin orders
    let orderDateMap =
        filteredOrders
        |> List.choose (fun o ->
            parseYearMonth o.OrderDate |> Option.map (fun ym -> (o.Id, ym)))
        |> Map.ofList
    let summaries = transform status origin orders items
    let grouped =
        summaries
        |> List.choose (fun s ->
            orderDateMap |> Map.tryFind s.OrderId |> Option.map (fun ym -> (ym, s)))
        |> List.fold (fun (acc: Map<(int * int), OrderSummary list>) (ym, s) ->
            let current = acc |> Map.tryFind ym |> Option.defaultValue []
            acc |> Map.add ym (s :: current)
        ) Map.empty
    grouped
    |> Map.toList
    |> List.map (fun ((yr, mo), ss) ->
        let count     = List.length ss |> float
        let avgAmount = ss |> List.fold (fun a s -> a + s.TotalAmount) 0.0 |> fun t -> System.Math.Round(t / count, 2)
        let avgTaxes  = ss |> List.fold (fun a s -> a + s.TotalTaxes)  0.0 |> fun t -> System.Math.Round(t / count, 2)
        { Year = yr; Month = mo; AvgAmount = avgAmount; AvgTaxes = avgTaxes })
    |> List.sortBy (fun m -> (m.Year, m.Month))

let summaryToCsvLine (s: OrderSummary) : string =
    sprintf "%d,%.2f,%.2f" s.OrderId s.TotalAmount s.TotalTaxes

let monthlySummaryToCsvLine (m: MonthlySummary) : string =
    sprintf "%d,%02d,%.2f,%.2f" m.Year m.Month m.AvgAmount m.AvgTaxes
