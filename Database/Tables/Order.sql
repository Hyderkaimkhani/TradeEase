CREATE TABLE [Order] (
    Id                  INT IDENTITY(1,1) PRIMARY KEY,
    CustomerId          INT NOT NULL,
    TruckId             INT NOT NULL,
    FruitId             INT NOT NULL,
    Quantity            DECIMAL(10,2) NOT NULL CHECK (Quantity > 0), -- Total quantity
    PurchasePrice       DECIMAL(10,2) NOT NULL CHECK (PurchasePrice > 0), -- Avg purchase price in case of multiple supplier against an order
    SellingPrice        DECIMAL(10,2) NOT NULL CHECK (SellingPrice > 0), -- Selling price
    TotalPurchaseAmount DECIMAL(10,2), 	        -- GENERATED ALWAYS AS (Quantity * PurchasePrice) STORED,
    TotalSellingAmount  DECIMAL(10,2),	        -- GENERATED ALWAYS AS (Quantity * SellingPrice) STORED,
    ProfitLoss          DECIMAL(10,2),	        -- GENERATED ALWAYS AS (TotalSellingAmount - TotalPurchaseAmount) STORED,
    AmountReceived      DECIMAL(10,2) NOT NULL default 0,
    PaymentStatus       VARCHAR(10) NOT NULL CHECK (PaymentStatus IN ('Unpaid', 'Partial', 'Paid')) DEFAULT 'Unpaid',
    OrderDate           DATETIME    DEFAULT GetDate(),
    DeliveryDate        DATETIME    NULL,
    [Status]            VARCHAR(20) DEFAULT 'Pending', -- Pending, Dispatched, Delivered, Canceled
    TruckAssignmentId   INT NULL,
    Notes               VARCHAR(255) NULL,
	[IsActive]          BIT            NOT NULL DEFAULT 1,
	[CreatedBy]         NVARCHAR (100) NULL,
	[CreatedDate]       DATETIME       NOT NULL DEFAULT GetDate(),
	[UpdatedBy]         NVARCHAR (100) NULL,
	[UpdatedDate]       DATETIME       NOT NULL DEFAULT GetDate(),
    FOREIGN KEY (CustomerId) REFERENCES Customer(Id),
    FOREIGN KEY (TruckId) REFERENCES Truck(Id),
    FOREIGN KEY (FruitId) REFERENCES Fruit(Id),
    FOREIGN KEY (TruckAssignmentId) REFERENCES TruckAssignment(Id)
);

CREATE INDEX IX_Order_OrderDate ON [Order] (OrderDate DESC); -- For sorting
CREATE INDEX IX_Order_CustomerId ON [Order] (CustomerId); -- For Filtering