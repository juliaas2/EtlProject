module Types

type Order = {
    Id        : int
    ClientId  : int
    OrderDate : string
    Status    : string
    Origin    : string
}

type OrderItem = {
    OrderId   : int
    ProductId : int
    Quantity  : int
    Price     : float
    Tax       : float
}

type OrderSummary = {
    OrderId     : int
    TotalAmount : float
    TotalTaxes  : float
}

type MonthlySummary = {
    Year      : int
    Month     : int
    AvgAmount : float
    AvgTaxes  : float
}

let splitCsvLine (line: string) : string[] =
    line.Split(',')

let parseOrder (fields: string[]) : Order option =
    try
        if fields.Length <> 5 then None
        else
            let id = int fields.[0]
            let clientId = int fields.[1]
            let orderDate = fields.[2].Trim()
            let status = fields.[3].Trim()
            let origin = fields.[4].Trim()
            // Basic validation
            if id <= 0 || clientId <= 0 || System.String.IsNullOrWhiteSpace(orderDate) || System.String.IsNullOrWhiteSpace(status) || System.String.IsNullOrWhiteSpace(origin) then
                None
            else
                Some { Id = id
                       ClientId = clientId
                       OrderDate = orderDate
                       Status = status
                       Origin = origin }
    with
    | _ -> None

let parseOrderItem (fields: string[]) : OrderItem option =
    try
        if fields.Length <> 5 then None
        else
            let orderId = int fields.[0]
            let productId = int fields.[1]
            let quantity = int fields.[2]
            let price = float fields.[3]
            let tax = float fields.[4]
            // Basic validation
            if orderId <= 0 || productId <= 0 || quantity <= 0 || price < 0.0 || tax < 0.0 || tax > 1.0 then
                None
            else
                Some { OrderId = orderId
                       ProductId = productId
                       Quantity = quantity
                       Price = price
                       Tax = tax }
    with
    | _ -> None
