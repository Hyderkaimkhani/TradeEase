SET IDENTITY_INSERT UserRole ON;

MERGE INTO UserRole AS Target 
USING (VALUES 
  (1, N'Admin',  1, getdate(), getdate()), 
  (2, N'User',  1, getdate(), getdate())
) 
AS Source (Id, [Name], [IsActive], [CreatedDate], [UpdatedDate]) 
ON Target.Id = Source.Id

--update matched rows
WHEN MATCHED 
THEN 
UPDATE SET 
	[Name] = Source.[Name], 
	[IsActive] = Source.[IsActive],
	[UpdatedDate] = Source.UpdatedDate

--insert new rows
WHEN NOT MATCHED BY TARGET THEN 
INSERT (Id, [Name], [IsActive], [CreatedDate], [UpdatedDate]) 
VALUES (Source.Id, Source.[Name], Source.[IsActive], Source.[CreatedDate], Source.[UpdatedDate]);

SET IDENTITY_INSERT UserRole OFF;


INSERT INTO [dbo].[Account] ([CompanyId], [Name], [Type], [AccountNumber], [BankName], [OpeningBalance], [CurrentBalance], [IsActive], [CreatedBy])
     VALUES (1,'Account Receivable','Receivable',null,null,0,0,1,'System');
GO
INSERT INTO [dbo].[Account] ([CompanyId], [Name], [Type], [AccountNumber], [BankName], [OpeningBalance], [CurrentBalance], [IsActive], [CreatedBy])
     VALUES (1,'Account Payable','Payable',null,null,0,0,1,'System');
