CREATE TABLE Account (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    CompanyId INT NOT NULL,
    [Name] NVARCHAR(100) NOT NULL,
    [AccountType] VARCHAR(20) NOT NULL CHECK ([AccountType] IN ('Cash', 'Bank', 'Wallet', 'Other')),
    AccountNumber NVARCHAR(50) NULL,
    BankName NVARCHAR(100) NULL,
    OpeningBalance DECIMAL(18,2) DEFAULT 0,
    CurrentBalance DECIMAL(18,2) DEFAULT 0,
    IsActive BIT NOT NULL DEFAULT 1,

    CreatedBy NVARCHAR(100),
    CreatedDate DATETIME NOT NULL DEFAULT GETDATE(),
    UpdatedBy NVARCHAR(100),
    UpdatedDate DATETIME NOT NULL DEFAULT GETDATE(),

    CONSTRAINT FK_Account_Company FOREIGN KEY (CompanyId) REFERENCES Company(Id)
);
