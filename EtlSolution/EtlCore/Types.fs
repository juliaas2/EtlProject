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

let parseOrder (fields: string[]) : Order =
    { Id        = int   fields.[0]
      ClientId  = int   fields.[1]
      OrderDate = fields.[2].Trim()
      Status    = fields.[3].Trim()
      Origin    = fields.[4].Trim() }

let parseOrderItem (fields: string[]) : OrderItem =
    { OrderId   = int   fields.[0]
      ProductId = int   fields.[1]
      Quantity  = int   fields.[2]
      Price     = float fields.[3]
      Tax       = float fields.[4] }
