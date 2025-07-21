CREATE TABLE [dbo].[BillDetail] (
    [Id]              INT             IDENTITY (1, 1) NOT NULL,
    [BillId]          INT             NOT NULL,
    [ReferenceType]     VARCHAR(20) NOT NULL CHECK (ReferenceType IN ('Order', 'Supply')),
    [OrderId]         INT             NULL,
    [SupplyId]        INT             NULL,
    [ReferenceNumber] VARCHAR (20)    NULL,
    [Description]     NVARCHAR (255)  NULL,
    [Quantity]        DECIMAL (10, 2) NULL,
    [UnitPrice]       DECIMAL (10, 2) NULL,
    [LineTotal]       DECIMAL (18, 2) NOT NULL,
    [Unit]            VARCHAR (20)    NULL,
    CONSTRAINT [PK_BillDetail] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_BillDetail_Bill] FOREIGN KEY ([BillId]) REFERENCES [dbo].[Bill] ([Id])
);
GO
CREATE NONCLUSTERED INDEX IX_BillDetail_OrderId ON dbo.BillDetail(OrderId)
WHERE OrderId IS NOT NULL;
GO

CREATE NONCLUSTERED INDEX IX_BillDetail_SupplyId ON dbo.BillDetail(SupplyId)
WHERE SupplyId IS NOT NULL;
GO
