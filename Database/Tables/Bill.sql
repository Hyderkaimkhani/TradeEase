CREATE TABLE [dbo].[Bill] (
    [Id]            INT             IDENTITY (1, 1) NOT NULL,
    [BillNumber]    VARCHAR (50)    NOT NULL,
    [EntityType]    VARCHAR (10)    NOT NULL,
    [EntityId]      INT             NOT NULL,
    [EntityName]    VARCHAR (10)    NOT NULL,
    [FromDate]      DATETIME        NULL,
    [ToDate]        DATETIME        NULL,
    [DueDate]       DATETIME        NULL,
    [TotalAmount]   DECIMAL (18, 2) NOT NULL,
    [TotalPaid]     DECIMAL (18, 2) NOT NULL,
    [Balance]       DECIMAL (18, 2) NOT NULL,
    [PaymentStatus] VARCHAR (20)    CONSTRAINT [DF_Bill_PaymentStatus] DEFAULT 'Unpaid' NOT NULL,
    [PaidDate]      DATETIME        NULL,
    [PdfPath]       VARCHAR (255)   NULL,
    [Notes]         VARCHAR (255)   NULL,
    [IsActive]      BIT             CONSTRAINT [DF_Bill_IsActive] DEFAULT 1 NOT NULL,
    [CreatedBy]     NVARCHAR (100)  NULL,
    [CreatedDate]   DATETIME        CONSTRAINT [DF_Bill_CreatedDate] DEFAULT (GETDATE()) NULL,
    [UpdatedBy]     NVARCHAR (100)  NULL,
    [UpdatedDate]   DATETIME        CONSTRAINT [DF_Bill_UpdatedDate] DEFAULT (GETDATE()) NULL,
    CONSTRAINT [PK_Bill] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Bill_Customer] FOREIGN KEY ([EntityId]) REFERENCES [dbo].[Customer] ([Id]),
    CONSTRAINT [CK_Bill_EntityType] CHECK ([EntityType] IN ('Customer','Supplier')),
    CONSTRAINT [CK_Bill_PaymentStatus] CHECK ([PaymentStatus] IN ('Unpaid','Partial','Paid')),
    CONSTRAINT [UQ_BillNumber] UNIQUE NONCLUSTERED ([BillNumber] ASC)
);

GO

CREATE INDEX IDX_Bill_Entity ON [dbo].[Bill] (EntityType, EntityId);