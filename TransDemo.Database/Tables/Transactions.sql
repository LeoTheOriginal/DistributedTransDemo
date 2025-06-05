CREATE TABLE dbo.Transactions (
    TransactionId    UNIQUEIDENTIFIER DEFAULT NEWID() PRIMARY KEY,
    AccountId        INT NOT NULL,
    BranchId         INT NOT NULL,
    Amount           MONEY NOT NULL,
    TransactionType  CHAR(10) NOT NULL CHECK (TransactionType IN ('DEBIT','CREDIT')),
    CreatedAt        DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    Description      NVARCHAR(200) NULL,
    CONSTRAINT FK_Transactions_Accounts FOREIGN KEY(AccountId)  REFERENCES dbo.Accounts(AccountId),
    CONSTRAINT FK_Transactions_Branches FOREIGN KEY(BranchId)   REFERENCES dbo.Branches(BranchId)
);

GO 

CREATE INDEX IX_Transactions_AccountId ON dbo.Transactions(AccountId);
