
CREATE TABLE PaymentAllocation(
    Id              INT PRIMARY KEY IDENTITY(1,1),
    PaymentId       INT NULL, -- FK to Payments
    [CompanyId]     INT			 NOT NULL,
    ReferenceType   VARCHAR(20) NOT NULL CHECK (ReferenceType IN ('Order', 'Supply')),
    ReferenceId     INT NOT NULL, -- FK to either Orders or Supply depending on ReferenceType
    AllocatedAmount DECIMAL(18,2) NOT NULL,
    [IsActive]            BIT            NOT NULL DEFAULT 1,
    [CreatedBy]         NVARCHAR (100) NULL,
	[CreatedDate]       DATETIME       NOT NULL DEFAULT GetDate(),
	[UpdatedBy]         NVARCHAR (100) NULL,
	[UpdatedDate]       DATETIME       NOT NULL DEFAULT GetDate(),
    CONSTRAINT [FK_Allocation_Company] FOREIGN KEY ([CompanyId]) REFERENCES [dbo].[Company] ([Id]),
    CONSTRAINT FK_Allocation_Payment FOREIGN KEY (PaymentId) REFERENCES Payment(Id)
);