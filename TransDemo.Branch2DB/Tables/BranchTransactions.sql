CREATE TABLE dbo.BranchTransactions (
    BranchTxnId    INT IDENTITY(1,1) PRIMARY KEY,
    BranchClientId INT NOT NULL,
    Amount         MONEY NOT NULL,
    TxnType        CHAR(10) NOT NULL CHECK (TxnType IN ('DEBIT','CREDIT')),
    CreatedAt      DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    Description    NVARCHAR(200) NULL,
    CONSTRAINT FK_BT_BranchClients FOREIGN KEY(BranchClientId) REFERENCES dbo.BranchClients(BranchClientId)
);

GO

CREATE INDEX IX_BranchTxn_ClientId ON dbo.BranchTransactions(BranchClientId);
