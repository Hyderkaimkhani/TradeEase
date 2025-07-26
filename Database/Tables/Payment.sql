CREATE TABLE Payment (
    Id              INT IDENTITY(1,1) PRIMARY KEY,
    [CompanyId]     INT			 NOT NULL,
    EntityId        INT NOT NULL, -- Customer.CustomerId OR Supplier.Id
    Amount     	    DECIMAL(10,2) NOT NULL,
    PaymentDate     DATETIME DEFAULT GETDATE(),
    PaymentMethod   VARCHAR(50), -- Cash, Bank Transfer, Mobile Payment
    Notes           TEXT NULL,
	[IsActive]            BIT            NOT NULL DEFAULT 1,
    [CreatedBy]           NVARCHAR (100) NULL,
    [CreatedDate]         DATETIME       NOT NULL DEFAULT GetDate(),
    [UpdatedBy]           NVARCHAR (100) NULL,
    [UpdatedDate]         DATETIME       NOT NULL DEFAULT GetDate(),
    CONSTRAINT FK_Payments_Entities FOREIGN KEY (EntityId) REFERENCES Customer(Id),
    CONSTRAINT [FK_Payment_Company] FOREIGN KEY ([CompanyId]) REFERENCES [dbo].[Company] ([Id])
);