CREATE TABLE AuditLog(
    Id INT PRIMARY KEY IDENTITY,
    TableName NVARCHAR(100),
    RecordId NVARCHAR(100),
    [Action] NVARCHAR(10),         -- INSERT, UPDATE, DELETE
    ChangedColumns NVARCHAR(MAX), -- Comma-separated column names
    OldValues NVARCHAR(MAX),     -- JSON or serialized old values
    NewValues NVARCHAR(MAX),     -- JSON or serialized new values
    CreatedBy NVARCHAR(100),
    CreatedDate DATETIME DEFAULT GETDATE()
);