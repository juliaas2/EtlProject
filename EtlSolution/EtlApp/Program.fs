module Program

open System
open Microsoft.Extensions.Configuration
open Microsoft.Extensions.Logging
open Types
open Transform
open Database

type EtlSettings = {
    OrdersPath: string
    ItemsPath: string
    OutputSummaryPath: string
    OutputMonthlyPath: string
    DatabasePath: string
    DefaultStatus: string
    DefaultOrigin: string
}

[<EntryPoint>]
let main argv =
    let config = 
        ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional = true, reloadOnChange = true)
            .AddEnvironmentVariables()
            .AddCommandLine(argv)
            .Build()

    let loggerFactory = LoggerFactory.Create(fun builder ->
        builder.AddConfiguration(config.GetSection("Logging")).AddConsole() |> ignore
    )
    let logger = loggerFactory.CreateLogger("ETL")

    logger.LogInformation("=== ETL — Programação Funcional ===")

    let settings = {
        OrdersPath = config.GetValue("EtlSettings:OrdersPath", "data/orders.csv")
        ItemsPath = config.GetValue("EtlSettings:ItemsPath", "data/order_items.csv")
        OutputSummaryPath = config.GetValue("EtlSettings:OutputSummaryPath", "data/output_summary.csv")
        OutputMonthlyPath = config.GetValue("EtlSettings:OutputMonthlyPath", "data/output_monthly.csv")
        DatabasePath = config.GetValue("EtlSettings:DatabasePath", "data/etl.db")
        DefaultStatus = config.GetValue("EtlSettings:DefaultStatus", "Complete")
        DefaultOrigin = config.GetValue("EtlSettings:DefaultOrigin", "O")
    }

    let mutable status = config.GetValue("status", settings.DefaultStatus)
    let mutable origin = config.GetValue("origin", settings.DefaultOrigin)

    if argv.Length = 0 then
        printfn "Escolha o status para filtrar (padrão: %s):" settings.DefaultStatus
        let statusInput = Console.ReadLine()
        status <- if String.IsNullOrWhiteSpace(statusInput) then settings.DefaultStatus else statusInput
        printfn "Escolha a origem para filtrar (padrão: %s):" settings.DefaultOrigin
        let originInput = Console.ReadLine()
        origin <- if String.IsNullOrWhiteSpace(originInput) then settings.DefaultOrigin else originInput

    logger.LogInformation("Filtros: status={Status} | origin={Origin}", status, origin)

    let orders = Extract.loadOrders settings.OrdersPath
    let items  = Extract.loadOrderItems settings.ItemsPath
    logger.LogInformation("[Extract] {OrderCount} pedidos | {ItemCount} itens", List.length orders, List.length items)

    let summaries = transform status origin orders items
    let monthly   = monthlyAverages status origin orders items
    logger.LogInformation("[Transform] {SummaryCount} pedidos | {MonthlyCount} entradas mensais", List.length summaries, List.length monthly)

    logger.LogInformation("order_id | total_amount | total_taxes")
    logger.LogInformation("{0}", String.replicate 40 "-")
    summaries |> List.iter (fun s ->
        logger.LogInformation("{OrderId,8} | {TotalAmount,12:F2} | {TotalTaxes,11:F2}", s.OrderId, s.TotalAmount, s.TotalTaxes))

    logger.LogInformation("")
    logger.LogInformation("year/month | avg_amount | avg_taxes")
    logger.LogInformation("{0}", String.replicate 40 "-")
    monthly |> List.iter (fun m ->
        logger.LogInformation(" {Year,4d}/{Month,02d}   | {AvgAmount,10:F2} | {AvgTaxes,9:F2}", m.Year, m.Month, m.AvgAmount, m.AvgTaxes))

    logger.LogInformation("")
    Load.writeSummaries settings.OutputSummaryPath summaries
    Load.writeMonthlySummaries settings.OutputMonthlyPath monthly

    // Save to database
    use connection = new System.Data.SQLite.SQLiteConnection($"Data Source={settings.DatabasePath}")
    connection.Open()
    createTables connection
    insertOrders connection orders
    insertOrderItems connection items
    insertOrderSummaries connection summaries
    insertMonthlySummaries connection monthly
    logger.LogInformation("[Database] Dados salvos em {DbPath}", settings.DatabasePath)

    logger.LogInformation("=== Concluído! ===")
    0