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
CREATE UNIQUE INDEX [IX_BillDetail_Reference] ON [dbo].[BillDetail] 
(
    [BillId], 
    [ReferenceType]
)
GO