module Extract

open Types

let private parseLines (lines: string[]) =
    lines
    |> Array.skip 1
    |> Array.filter (fun l -> l.Trim() <> "")

let private ordersFromLines (lines: string[]) : Order list =
    parseLines lines
    |> Array.map (splitCsvLine >> parseOrder)
    |> Array.choose id
    |> Array.toList

let private itemsFromLines (lines: string[]) : OrderItem list =
    parseLines lines
    |> Array.map (splitCsvLine >> parseOrderItem)
    |> Array.choose id
    |> Array.toList

let loadOrders (path: string) : Order list =
    try
        System.IO.File.ReadAllLines(path) |> ordersFromLines
    with
    | :? System.IO.FileNotFoundException ->
        printfn "Erro: Arquivo de pedidos não encontrado: %s" path
        []
    | ex ->
        printfn "Erro ao carregar pedidos: %s" ex.Message
        []

let loadOrderItems (path: string) : OrderItem list =
    try
        System.IO.File.ReadAllLines(path) |> itemsFromLines
    with
    | :? System.IO.FileNotFoundException ->
        printfn "Erro: Arquivo de itens não encontrado: %s" path
        []
    | ex ->
        printfn "Erro ao carregar itens: %s" ex.Message
        []

let loadOrdersFromUrl (url: string) : Order list =
    try
        use client = new System.Net.Http.HttpClient()
        client.GetStringAsync(url).Result.Split([| '\n'; '\r' |], System.StringSplitOptions.RemoveEmptyEntries)
        |> ordersFromLines
    with
    | ex ->
        printfn "Erro ao carregar pedidos da URL %s: %s" url ex.Message
        []

let loadOrderItemsFromUrl (url: string) : OrderItem list =
    try
        use client = new System.Net.Http.HttpClient()
        client.GetStringAsync(url).Result.Split([| '\n'; '\r' |], System.StringSplitOptions.RemoveEmptyEntries)
        |> itemsFromLines
    with
    | ex ->
        printfn "Erro ao carregar itens da URL %s: %s" url ex.Message
        []
