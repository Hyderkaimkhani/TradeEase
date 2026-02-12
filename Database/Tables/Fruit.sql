CREATE TABLE Fruit (
    Id             INT IDENTITY(1,1) PRIMARY KEY,
    Name           VARCHAR(100) NOT NULL,
    UnitType       VARCHAR(20) NOT NULL CHECK (UnitType IN ('Kg', 'Dozen', 'Box','Man')),
    [IsActive]            BIT            NOT NULL DEFAULT 1,
    [CreatedBy]           NVARCHAR (100) NULL,
    [CreatedDate]         DATETIME       NOT NULL DEFAULT GetDate(),
    [UpdatedBy]           NVARCHAR (100) NULL,
    [UpdatedDate]         DATETIME       NOT NULL DEFAULT GetDate(),
);