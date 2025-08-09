CREATE TABLE [dbo].[AccountTransaction]
(
	Id                      INT IDENTITY(1,1) PRIMARY KEY,
    CompanyId               INT NOT NULL,
    AccountId               INT NOT NULL,
    TransactionType         VARCHAR(20) NOT NULL CHECK (TransactionType IN ('Payment', 'Expense', 'Income', 'Transfer', 'Adjustment', 'Order', 'Supply')),
    TransactionDirection    VARCHAR(10)  NOT NULL DEFAULT 'Credit' CHECK (TransactionDirection IN ('Debit', 'Credit')),
    Amount                  DECIMAL(10,2) NOT NULL,
    TransactionDate         DATETIME DEFAULT GETDATE(),
    PaymentMethod           VARCHAR(50) NULL,    -- Cash, Bank Transfer, Mobile Payment
    Notes                   TEXT NULL,
    EntityId                INT NULL,           -- Customer/Supplier
    ReferenceType           VARCHAR(20) NOT NULL CHECK (ReferenceType IN ('Order', 'Supply', 'Expense', 'Payment', 'Transfer','OpeningBalance')),
    ReferenceId             INT NULL,
    Category                VARCHAR(50) NULL,   -- Rent, Salary, Utilities. Bonus, Commission, etc
    Party                   VARCHAR(100) NULL,  -- Who was paid for expenses
    ToAccountId             INT NULL,           -- Destination account for transfers
    IsActive                BIT NOT NULL DEFAULT 1,
    CreatedBy               NVARCHAR(100) NULL,
    CreatedDate             DATETIME NOT NULL DEFAULT GETDATE(),
    UpdatedBy               NVARCHAR(100) NULL,
    UpdatedDate             DATETIME NOT NULL DEFAULT GETDATE(),
    CONSTRAINT FK_AccountTransaction_Company FOREIGN KEY (CompanyId) REFERENCES Company(Id),
    CONSTRAINT FK_AccountTransaction_Account FOREIGN KEY (AccountId) REFERENCES Account(Id),
    CONSTRAINT FK_AccountTransaction_Entity FOREIGN KEY (EntityId) REFERENCES Customer(Id),
    CONSTRAINT FK_AccountTransaction_ToAccount FOREIGN KEY (ToAccountId) REFERENCES Account(Id)
);

GO

CREATE INDEX IX_AccountTransaction_Account_Date ON [dbo].[AccountTransaction](AccountId, TransactionDate);

GO
CREATE INDEX IX_AccountTransaction_Entity_Date ON [dbo].[AccountTransaction](EntityId, TransactionDate);

GO
CREATE INDEX IX_AccountTransaction_Company_Date ON [dbo].[AccountTransaction](CompanyId, TransactionDate);