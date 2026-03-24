module Transform

open Types

let rec filterOrders (status: string) (origin: string) (orders: Order list) : Order list =
    match orders with
    | [] -> []
    | h :: t ->
        let tf = filterOrders status origin t
        if h.Status = status && h.Origin = origin then h :: tf else tf

let itemRevenue (item: OrderItem) : float = item.Price * float item.Quantity

let itemTax (item: OrderItem) : float = item.Tax * itemRevenue item

let joinOrdersWithItems (orders: Order list) (items: OrderItem list) : (Order * OrderItem) list =
    let rec loop rest =
        match rest with
        | [] -> []
        | i :: xs ->
            let o = orders |> List.tryFind (fun x -> x.Id = i.OrderId)
            match o with
            | Some order -> (order, i) :: loop xs
            | None -> loop xs
    loop items

let aggregateByOrder (pairs: (Order * OrderItem) list) : OrderSummary list =
    let grouped = pairs |> List.fold (fun acc (_, item) ->
        let curr = acc |> Map.tryFind item.OrderId |> Option.defaultValue []
        acc |> Map.add item.OrderId (item :: curr)
    ) Map.empty
    grouped |> Map.toList |> List.map (fun (orderId, items) ->
        let totalAmount = items |> List.fold (fun acc i -> acc + itemRevenue i) 0.0
        let totalTaxes = items |> List.fold (fun acc i -> acc + itemTax i) 0.0
        { OrderId = orderId
          TotalAmount = System.Math.Round(totalAmount, 2)
          TotalTaxes = System.Math.Round(totalTaxes, 2) }
    ) |> List.sortBy (fun s -> s.OrderId)

let transform (status: string) (origin: string) (orders: Order list) (items: OrderItem list) : OrderSummary list =
    orders |> filterOrders status origin |> fun f -> joinOrdersWithItems f items |> aggregateByOrder

let parseYearMonth (dateStr: string) : (int * int) option =
    let p = dateStr.Split('T').[0].Split('-')
    if p.Length >= 2 then Some (int p.[0], int p.[1]) else None

let monthlyAverages (status: string) (origin: string) (orders: Order list) (items: OrderItem list) : MonthlySummary list =
    let fo = filterOrders status origin orders
    let ym = fo |> List.choose (fun o -> parseYearMonth o.OrderDate |> Option.map (fun y -> (o.Id, y))) |> Map.ofList
    let sums = transform status origin orders items
    let grp = sums |> List.choose (fun s -> ym |> Map.tryFind s.OrderId |> Option.map (fun y -> (y, s))) |> List.fold (fun a (y, s) ->
        let c = a |> Map.tryFind y |> Option.defaultValue []
        a |> Map.add y (s :: c)
    ) Map.empty
    grp |> Map.toList |> List.map (fun ((yr, mo), ss) ->
        let cnt = List.length ss |> float
        let aa = ss |> List.fold (fun a s -> a + s.TotalAmount) 0.0 |> fun t -> System.Math.Round(t / cnt, 2)
        let at = ss |> List.fold (fun a s -> a + s.TotalTaxes) 0.0 |> fun t -> System.Math.Round(t / cnt, 2)
        { Year = yr; Month = mo; AvgAmount = aa; AvgTaxes = at }
    ) |> List.sortBy (fun m -> (m.Year, m.Month))

let summaryToCsvLine (s: OrderSummary) : string = sprintf "%d,%.2f,%.2f" s.OrderId s.TotalAmount s.TotalTaxes

let monthlySummaryToCsvLine (m: MonthlySummary) : string = sprintf "%d,%02d,%.2f,%.2f" m.Year m.Month m.AvgAmount m.AvgTaxes
