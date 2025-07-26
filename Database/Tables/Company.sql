CREATE TABLE Company (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    [Name] NVARCHAR(100) NOT NULL,
    Code NVARCHAR(20) UNIQUE NOT NULL,
    [Address] NVARCHAR(255),
    Phone NVARCHAR(20),
    Email NVARCHAR(100),

    LogoUrl NVARCHAR(255) NULL, -- For branding
    [Logo] VARBINARY(MAX) NULL,
    MaxUsers INT NULL,     -- License cap
    ExpiryDate DATETIME NULL, -- Subscription expiry
    Timezone NVARCHAR(50), -- Optional localization
    CurrencySymbol NVARCHAR(10), -- For formatting (e.g., Rs, ₹, $, €)
    GSTNumber NVARCHAR(50),     -- Optional tax ID / regional field

    IsActive BIT NOT NULL DEFAULT 1,
    CreatedDate DATETIME NOT NULL DEFAULT GETDATE(),
    UpdatedDate DATETIME NOT NULL DEFAULT GETDATE(),
    CreatedBy NVARCHAR(100),
    UpdatedBy NVARCHAR(100)
);


--INSERT INTO Company ( [Name], Code, [Address], Phone, Email, MaxUsers, CurrencySymbol, Timezone, IsActive, CreatedDate, UpdatedDate, CreatedBy, UpdatedBy ) 
--VALUES ( 'Al Azeem Farms', LEFT(REPLACE(NEWID(), '-', ''), 10), 'Mirpurkhas Road', '03335432123', 'alazeem@gmail.com', 5, 'Rs', 'Asia/Karachi | +05:00', 1,  GETDATE(), GETDATE(),  'System', 'System' );