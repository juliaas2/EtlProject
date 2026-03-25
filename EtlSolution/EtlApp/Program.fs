module Program

open Types
open Transform

[<EntryPoint>]
let main argv =
    let ordersPath    = "data/orders.csv"
    let itemsPath     = "data/order_items.csv"
    let outputPath    = "data/output_summary.csv"
    let outputMonthly = "data/output_monthly.csv"

    let status = if argv.Length > 0 then argv.[0] else "Complete"
    let origin = if argv.Length > 1 then argv.[1] else "O"

    printfn "=== ETL — Programação Funcional ==="
    printfn "Filtros: status=%s | origin=%s\n" status origin

    let orders = Extract.loadOrders ordersPath
    let items  = Extract.loadOrderItems itemsPath
    printfn "[Extract] %d pedidos | %d itens\n" (List.length orders) (List.length items)

    let summaries = transform status origin orders items
    let monthly   = monthlyAverages status origin orders items
    printfn "[Transform] %d pedidos | %d entradas mensais\n" (List.length summaries) (List.length monthly)

    printfn "order_id | total_amount | total_taxes"
    printfn "%s" (String.replicate 40 "-")
    summaries |> List.iter (fun s ->
        printfn "%8d | %12.2f | %11.2f" s.OrderId s.TotalAmount s.TotalTaxes)

    printfn "\nyear/month | avg_amount | avg_taxes"
    printfn "%s" (String.replicate 40 "-")
    monthly |> List.iter (fun m ->
        printfn " %4d/%02d   | %10.2f | %9.2f" m.Year m.Month m.AvgAmount m.AvgTaxes)

    printfn ""
    Load.writeSummaries outputPath summaries
    Load.writeMonthlySummaries outputMonthly monthly

    printfn "\n=== Concluído! ==="
    0