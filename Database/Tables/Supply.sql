CREATE TABLE Supply (
    Id             INT IDENTITY(1,1) PRIMARY KEY,
    SupplyNumber   VARCHAR(20) NOT NULL,
    SupplierId     INT NOT NULL FOREIGN KEY REFERENCES Customer(Id),
    FruitId        INT NOT NULL FOREIGN KEY REFERENCES Fruit(Id),
    TruckId        INT NULL FOREIGN KEY REFERENCES Truck(Id),
    Quantity       DECIMAL(10,2) NOT NULL CHECK (Quantity > 0),     -- Kg or Units
    PurchasePrice  DECIMAL(10,2) NOT NULL CHECK (PurchasePrice > 0), -- Price per unit at the time of purchase
    TotalPrice     DECIMAL(10,2) NOT NULL CHECK (TotalPrice > 0),   -- PurchasePrice * Quantity
    AmountPaid     DECIMAL(10,2) NOT NULL default 0,
    PaymentStatus  VARCHAR(10) NOT NULL CHECK (PaymentStatus IN ('Unpaid', 'Partial', 'Paid')) DEFAULT 'Unpaid', -- Unpaid, Partially Paid,
    SupplyDate     DATETIME DEFAULT GETDATE(),
    TruckAssignmentId   INT NULL FOREIGN KEY REFERENCES TruckAssignment(Id),
    Notes               VARCHAR(255) NULL,
    [IsActive]     BIT            NOT NULL DEFAULT 1,
    [CreatedBy]    NVARCHAR (100) NULL,
    [CreatedDate]  DATETIME       NOT NULL DEFAULT GetDate(),
    [UpdatedBy]    NVARCHAR (100) NULL,
    [UpdatedDate]  DATETIME       NOT NULL DEFAULT GetDate(),
);

CREATE INDEX IX_Supply_SupplyDate ON Supply (SupplyDate DESC); -- For sorting
CREATE INDEX IX_Supply_CustomerId ON Supply (SupplierId); -- For Filtering