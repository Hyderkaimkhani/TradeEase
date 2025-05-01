CREATE TABLE [Order] (
    Id                  INT IDENTITY(1,1) PRIMARY KEY,
    CustomerId          INT NOT NULL,
    TruckId             INT NOT NULL,
    FruitId             INT NOT NULL,
    Quantity            DECIMAL(10,2) NOT NULL, -- Total quantity in Kg
    PurchasePrice       DECIMAL(10,2) NOT NULL, -- Avg purchase price per 40Kg
    SellingPrice        DECIMAL(10,2) NOT NULL, -- Selling price per 40Kg
    TotalPurchaseAmount DECIMAL(10,2),	        -- GENERATED ALWAYS AS (Quantity / 40 * PurchasePrice) STORED,
    TotalSellingAmount  DECIMAL(10,2),	        -- GENERATED ALWAYS AS (Quantity / 40 * SellingPrice) STORED,
    ProfitLoss          DECIMAL(10,2),	        -- GENERATED ALWAYS AS (TotalSellingAmount - TotalPurchaseAmount) STORED,
    OrderDate           DATETIME    DEFAULT CURRENT_TIMESTAMP,
    [Status]            VARCHAR(20) DEFAULT 'Pending', -- Pending, Dispatched, Delivered, Canceled
    PaymentStatus       VARCHAR(10) NOT NULL CHECK (PaymentStatus IN ('Unpaid', 'Partial', 'Paid')) DEFAULT 'Unpaid',
	[IsActive]          BIT            NOT NULL DEFAULT 1,
	[CreatedBy]         NVARCHAR (100) NULL,
	[CreatedDate]       DATETIME       NOT NULL DEFAULT GetDate(),
	[UpdatedBy]         NVARCHAR (100) NULL,
	[UpdatedDate]       DATETIME       NOT NULL DEFAULT GetDate(),
    FOREIGN KEY (CustomerId) REFERENCES Customer(Id),
    FOREIGN KEY (TruckId) REFERENCES Truck(Id),
    FOREIGN KEY (FruitId) REFERENCES Fruit(Id)
);