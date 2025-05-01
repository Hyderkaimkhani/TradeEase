
CREATE TABLE Customers (
    Id     INT IDENTITY(1,1) PRIMARY KEY,
    [Name]           VARCHAR(100) NOT NULL,
    Phone        	VARCHAR(20)  NOT NULL,
	City       		VARCHAR (100) NULL,
    [Address]        VARCHAR(255) NULL,
    PaymentTerms   VARCHAR(50), -- Cash, Credit, Partial
    TotalCredit    DECIMAL(10,2) DEFAULT 0,
	CreditBalance  DECIMAL(10,2) DEFAULT 0, -- updated automatically
    EntityType   VARCHAR(50) NOT NULL CHECK (EntityType IN ('Customer', 'Supplier')) DEFAULT 'Customer',
    [IsActive]            BIT            NOT NULL DEFAULT 1,
    [CreatedBy]           NVARCHAR (100) NULL,
    [CreatedDate]         DATETIME       NOT NULL DEFAULT GetDate(),
    [UpdatedBy]           NVARCHAR (100) NULL,
    [UpdatedDate]         DATETIME       NOT NULL DEFAULT GetDate(),
);
