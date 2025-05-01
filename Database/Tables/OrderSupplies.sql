CREATE TABLE OrderSupplies (
    Id INT PRIMARY KEY IDENTITY(1,1),
    OrderId INT NOT NULL,
    SupplyId INT NOT NULL,
    QuantityUsed DECIMAL(18,2) NOT NULL, -- if partial quantity used from a supply
    FOREIGN KEY (OrderId) REFERENCES [Order] (Id),
    FOREIGN KEY (SupplyId) REFERENCES Supply(Id)
);
