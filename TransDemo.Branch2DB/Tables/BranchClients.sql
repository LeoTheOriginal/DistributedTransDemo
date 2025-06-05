CREATE TABLE dbo.BranchClients (
    BranchClientId   INT IDENTITY(1,1) PRIMARY KEY,
    CentralClientId  INT NOT NULL,
    LocalAccountNo   CHAR(20) NOT NULL UNIQUE,
    Balance          MONEY NOT NULL,
    LastSync         DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME()
);
