module Extract

open EtlCore.Types

/// Lê linhas de um CSV a partir de um array de strings (sem cabeçalho).
let private parseLines (lines: string[]) =
    lines
    |> Array.skip 1
    |> Array.filter (fun l -> l.Trim() <> "")

/// Carrega Orders a partir de um array de linhas CSV.
let private ordersFromLines (lines: string[]) : Order list =
    parseLines lines
    |> Array.map (splitCsvLine >> parseOrder)
    |> Array.toList

/// Carrega OrderItems a partir de um array de linhas CSV.
let private itemsFromLines (lines: string[]) : OrderItem list =
    parseLines lines
    |> Array.map (splitCsvLine >> parseOrderItem)
    |> Array.toList

// ── Leitura local ─────────────────────────────────────────────────────────────

/// Lê o arquivo de pedidos de um caminho local.
let loadOrders (path: string) : Order list =
    System.IO.File.ReadAllLines(path) |> ordersFromLines

/// Lê o arquivo de itens de um caminho local.
let loadOrderItems (path: string) : OrderItem list =
    System.IO.File.ReadAllLines(path) |> itemsFromLines

// ── Leitura via HTTP (opcional 1) ─────────────────────────────────────────────

/// Baixa um CSV a partir de uma URL HTTP e retorna as linhas.
let private fetchLines (url: string) : string[] =
    use client = new System.Net.Http.HttpClient()
    let content = client.GetStringAsync(url).Result
    content.Split([| '\n'; '\r' |], System.StringSplitOptions.RemoveEmptyEntries)

/// Lê pedidos a partir de uma URL HTTP.
let loadOrdersFromUrl (url: string) : Order list =
    fetchLines url |> ordersFromLines

/// Lê itens a partir de uma URL HTTP.
let loadOrderItemsFromUrl (url: string) : OrderItem list =
    fetchLines url |> itemsFromLines
