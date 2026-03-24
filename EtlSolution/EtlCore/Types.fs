namespace EtlCore

module Types =

    // ── Records ───────────────────────────────────────────────────────────────────

    /// Representa um pedido da tabela Order.
    type Order = {
        Id        : int
        ClientId  : int
        OrderDate : string
        Status    : string
        Origin    : string
    }

    /// Representa um item da tabela OrderItem.
    type OrderItem = {
        OrderId   : int
        ProductId : int
        Quantity  : int
        Price     : float
        Tax       : float
    }

    /// Resultado do ETL: totais agregados por pedido.
    type OrderSummary = {
        OrderId     : int
        TotalAmount : float
        TotalTaxes  : float
    }

    /// Resultado opcional: média de receita e impostos por mês/ano.
    type MonthlySummary = {
        Year      : int
        Month     : int
        AvgAmount : float
        AvgTaxes  : float
    }

    // ── Helper Functions ──────────────────────────────────────────────────────────

    /// Divide uma linha CSV pelo delim itador vírgula.
    let splitCsvLine (line: string) : string[] =
        line.Split(',')

    /// Converte um array de campos CSV em um record Order.
    /// Esperado: id, client_id, order_date, status, origin
    let parseOrder (fields: string[]) : Order =
        { Id        = int   fields.[0]
          ClientId  = int   fields.[1]
          OrderDate = fields.[2].Trim()
          Status    = fields.[3].Trim()
          Origin    = fields.[4].Trim() }

    /// Converte um array de campos CSV em um record OrderItem.
    /// Esperado: order_id, product_id, quantity, price, tax
    let parseOrderItem (fields: string[]) : OrderItem =
        { OrderId   = int   fields.[0]
          ProductId = int   fields.[1]
          Quantity  = int   fields.[2]
          Price     = float fields.[3]
          Tax       = float fields.[4] }
