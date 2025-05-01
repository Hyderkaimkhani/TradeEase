
CREATE TABLE PaymentAllocation(
    Id              INT PRIMARY KEY IDENTITY(1,1),
    PaymentId       INT NOT NULL, -- FK to Payments
    ReferenceType   VARCHAR(20) NOT NULL CHECK (ReferenceType IN ('Order', 'Supply')),
    ReferenceId     INT NOT NULL, -- FK to either Orders or Supply depending on ReferenceType
    AllocatedAmount DECIMAL(18,2) NOT NULL,
    CONSTRAINT FK_Allocation_Payment FOREIGN KEY (PaymentId) REFERENCES Payment(Id)
);