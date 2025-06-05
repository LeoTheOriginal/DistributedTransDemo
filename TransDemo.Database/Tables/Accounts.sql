CREATE TABLE dbo.Accounts (
    AccountId     INT IDENTITY(1,1) PRIMARY KEY,
    CustomerId    INT NOT NULL,
    AccountNumber VARCHAR(34) NOT NULL UNIQUE,
    Balance       MONEY NOT NULL CONSTRAINT DF_Accounts_Balance DEFAULT(0),
    BranchId      INT NOT NULL,
    AccountTypeId INT NOT NULL,
    CreatedDate   DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    CONSTRAINT FK_Accounts_Customers    FOREIGN KEY(CustomerId)    REFERENCES dbo.Customers(CustomerId),
    CONSTRAINT FK_Accounts_Branches     FOREIGN KEY(BranchId)      REFERENCES dbo.Branches(BranchId),
    CONSTRAINT FK_Accounts_AccountTypes FOREIGN KEY(AccountTypeId) REFERENCES dbo.AccountTypes(AccountTypeId)
);

GO

CREATE INDEX IX_Accounts_CustomerId ON dbo.Accounts(CustomerId);
