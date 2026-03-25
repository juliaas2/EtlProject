module Load

open Types
open Transform

let writeSummaries (path: string) (summaries: OrderSummary list) : unit =
    let header = "order_id,total_amount,total_taxes"
    let lines  = summaries |> List.map summaryToCsvLine
    System.IO.File.WriteAllLines(path, header :: lines)
    printfn "[Load] %s — %d registros gravados." path (List.length summaries)

let writeMonthlySummaries (path: string) (summaries: MonthlySummary list) : unit =
    let header = "year,month,avg_amount,avg_taxes"
    let lines  = summaries |> List.map monthlySummaryToCsvLine
    System.IO.File.WriteAllLines(path, header :: lines)
    printfn "[Load] %s — %d registros gravados." path (List.length summaries)
