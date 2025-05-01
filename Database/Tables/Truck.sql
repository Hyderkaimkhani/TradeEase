CREATE TABLE Truck (
    Id        INT IDENTITY(1,1) PRIMARY KEY,
    TruckNumber    VARCHAR(50) UNIQUE NOT NULL,
    Capacity       DECIMAL(10,2) NULL, -- Capacity in kg
    DriverName     VARCHAR(100),
    DriverContact  VARCHAR(20),
    Status         VARCHAR(20) DEFAULT 'Available', -- Available, Assigned, In-Transit, Delivered
    [IsActive]     BIT            NOT NULL DEFAULT 1,
    [CreatedBy]    NVARCHAR (100) NULL,
    [CreatedDate]  DATETIME       NOT NULL DEFAULT GetDate(),
    [UpdatedBy]    NVARCHAR (100) NULL,
    [UpdatedDate]  DATETIME       NOT NULL DEFAULT GetDate(),
);
