module Database

open System.Data.SQLite
open Types

let createTables (connection: SQLiteConnection) =
    let createOrdersTable = """
        CREATE TABLE IF NOT EXISTS Orders (
            Id INTEGER PRIMARY KEY,
            ClientId INTEGER,
            OrderDate TEXT,
            Status TEXT,
            Origin TEXT
        );
    """
    let createOrderItemsTable = """
        CREATE TABLE IF NOT EXISTS OrderItems (
            OrderId INTEGER,
            ProductId INTEGER,
            Quantity INTEGER,
            Price REAL,
            Tax REAL,
            PRIMARY KEY (OrderId, ProductId)
        );
    """
    let createOrderSummariesTable = """
        CREATE TABLE IF NOT EXISTS OrderSummaries (
            OrderId INTEGER PRIMARY KEY,
            TotalAmount REAL,
            TotalTaxes REAL
        );
    """
    let createMonthlySummariesTable = """
        CREATE TABLE IF NOT EXISTS MonthlySummaries (
            Year INTEGER,
            Month INTEGER,
            AvgAmount REAL,
            AvgTaxes REAL,
            PRIMARY KEY (Year, Month)
        );
    """
    use command = new SQLiteCommand(createOrdersTable, connection)
    command.ExecuteNonQuery() |> ignore
    use command = new SQLiteCommand(createOrderItemsTable, connection)
    command.ExecuteNonQuery() |> ignore
    use command = new SQLiteCommand(createOrderSummariesTable, connection)
    command.ExecuteNonQuery() |> ignore
    use command = new SQLiteCommand(createMonthlySummariesTable, connection)
    command.ExecuteNonQuery() |> ignore

let insertOrders (connection: SQLiteConnection) (orders: Order list) =
    let insertSql = "INSERT OR REPLACE INTO Orders (Id, ClientId, OrderDate, Status, Origin) VALUES (@Id, @ClientId, @OrderDate, @Status, @Origin)"
    orders |> List.iter (fun order ->
        use command = new SQLiteCommand(insertSql, connection)
        command.Parameters.AddWithValue("@Id", order.Id) |> ignore
        command.Parameters.AddWithValue("@ClientId", order.ClientId) |> ignore
        command.Parameters.AddWithValue("@OrderDate", order.OrderDate) |> ignore
        command.Parameters.AddWithValue("@Status", order.Status) |> ignore
        command.Parameters.AddWithValue("@Origin", order.Origin) |> ignore
        command.ExecuteNonQuery() |> ignore
    )

let insertOrderItems (connection: SQLiteConnection) (items: OrderItem list) =
    let insertSql = "INSERT OR REPLACE INTO OrderItems (OrderId, ProductId, Quantity, Price, Tax) VALUES (@OrderId, @ProductId, @Quantity, @Price, @Tax)"
    items |> List.iter (fun item ->
        use command = new SQLiteCommand(insertSql, connection)
        command.Parameters.AddWithValue("@OrderId", item.OrderId) |> ignore
        command.Parameters.AddWithValue("@ProductId", item.ProductId) |> ignore
        command.Parameters.AddWithValue("@Quantity", item.Quantity) |> ignore
        command.Parameters.AddWithValue("@Price", item.Price) |> ignore
        command.Parameters.AddWithValue("@Tax", item.Tax) |> ignore
        command.ExecuteNonQuery() |> ignore
    )

let insertOrderSummaries (connection: SQLiteConnection) (summaries: OrderSummary list) =
    let insertSql = "INSERT OR REPLACE INTO OrderSummaries (OrderId, TotalAmount, TotalTaxes) VALUES (@OrderId, @TotalAmount, @TotalTaxes)"
    summaries |> List.iter (fun summary ->
        use command = new SQLiteCommand(insertSql, connection)
        command.Parameters.AddWithValue("@OrderId", summary.OrderId) |> ignore
        command.Parameters.AddWithValue("@TotalAmount", summary.TotalAmount) |> ignore
        command.Parameters.AddWithValue("@TotalTaxes", summary.TotalTaxes) |> ignore
        command.ExecuteNonQuery() |> ignore
    )

let insertMonthlySummaries (connection: SQLiteConnection) (summaries: MonthlySummary list) =
    let insertSql = "INSERT OR REPLACE INTO MonthlySummaries (Year, Month, AvgAmount, AvgTaxes) VALUES (@Year, @Month, @AvgAmount, @AvgTaxes)"
    summaries |> List.iter (fun summary ->
        use command = new SQLiteCommand(insertSql, connection)
        command.Parameters.AddWithValue("@Year", summary.Year) |> ignore
        command.Parameters.AddWithValue("@Month", summary.Month) |> ignore
        command.Parameters.AddWithValue("@AvgAmount", summary.AvgAmount) |> ignore
        command.Parameters.AddWithValue("@AvgTaxes", summary.AvgTaxes) |> ignore
        command.ExecuteNonQuery() |> ignore
    )

let loadOrdersFromDb (connection: SQLiteConnection) : Order list =
    let selectSql = "SELECT Id, ClientId, OrderDate, Status, Origin FROM Orders"
    use command = new SQLiteCommand(selectSql, connection)
    use reader = command.ExecuteReader()
    let rec readOrders acc =
        if reader.Read() then
            let order = {
                Id = reader.GetInt32(0)
                ClientId = reader.GetInt32(1)
                OrderDate = reader.GetString(2)
                Status = reader.GetString(3)
                Origin = reader.GetString(4)
            }
            readOrders (order :: acc)
        else
            List.rev acc
    readOrders []

let loadOrderItemsFromDb (connection: SQLiteConnection) : OrderItem list =
    let selectSql = "SELECT OrderId, ProductId, Quantity, Price, Tax FROM OrderItems"
    use command = new SQLiteCommand(selectSql, connection)
    use reader = command.ExecuteReader()
    let rec readItems acc =
        if reader.Read() then
            let item = {
                OrderId = reader.GetInt32(0)
                ProductId = reader.GetInt32(1)
                Quantity = reader.GetInt32(2)
                Price = reader.GetDouble(3)
                Tax = reader.GetDouble(4)
            }
            readItems (item :: acc)
        else
            List.rev acc
    readItems []