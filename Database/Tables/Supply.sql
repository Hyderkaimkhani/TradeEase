CREATE TABLE Supply (
    Id             INT IDENTITY(1,1) PRIMARY KEY,
    SupplierId     INT NOT NULL FOREIGN KEY REFERENCES Customer(Id),
    FruitId        INT NOT NULL FOREIGN KEY REFERENCES Fruit(Id),
    TruckId        INT NULL FOREIGN KEY REFERENCES Truck(Id),
    Quantity       DECIMAL(10,2) NOT NULL, -- Kg or Units
    PurchasePrice  DECIMAL(10,2) NOT NULL, -- Price per unit at the time of purchase
    TotalPrice     DECIMAL(10,2) NOT NULL, -- PurchasePrice * Quantity
    AmountPaid     DECIMAL(10,2) NOT NULL,
    PaymentStatus  VARCHAR(10) NOT NULL CHECK (PaymentStatus IN ('Unpaid', 'Partial', 'Paid')) DEFAULT 'Unpaid', -- Unpaid, Partially Paid,
    SupplyDate     DATETIME DEFAULT GETDATE(),
    [IsActive]     BIT            NOT NULL DEFAULT 1,
    [CreatedBy]    NVARCHAR (100) NULL,
    [CreatedDate]  DATETIME       NOT NULL DEFAULT GetDate(),
    [UpdatedBy]    NVARCHAR (100) NULL,
    [UpdatedDate]  DATETIME       NOT NULL DEFAULT GetDate(),
);