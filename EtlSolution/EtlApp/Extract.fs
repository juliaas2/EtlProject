module Extract

open Types

let private parseLines (lines: string[]) =
    lines
    |> Array.skip 1
    |> Array.filter (fun l -> l.Trim() <> "")

let private ordersFromLines (lines: string[]) : Order list =
    parseLines lines
    |> Array.map (splitCsvLine >> parseOrder)
    |> Array.toList

let private itemsFromLines (lines: string[]) : OrderItem list =
    parseLines lines
    |> Array.map (splitCsvLine >> parseOrderItem)
    |> Array.toList

let loadOrders (path: string) : Order list =
    System.IO.File.ReadAllLines(path) |> ordersFromLines

let loadOrderItems (path: string) : OrderItem list =
    System.IO.File.ReadAllLines(path) |> itemsFromLines

let loadOrdersFromUrl (url: string) : Order list =
    use client = new System.Net.Http.HttpClient()
    client.GetStringAsync(url).Result.Split([| '\n'; '\r' |], System.StringSplitOptions.RemoveEmptyEntries)
    |> ordersFromLines

let loadOrderItemsFromUrl (url: string) : OrderItem list =
    use client = new System.Net.Http.HttpClient()
    client.GetStringAsync(url).Result.Split([| '\n'; '\r' |], System.StringSplitOptions.RemoveEmptyEntries)
    |> itemsFromLines
