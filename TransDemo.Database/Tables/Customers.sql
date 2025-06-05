CREATE TABLE dbo.Customers (
    CustomerId INT IDENTITY(1,1) PRIMARY KEY,
    FirstName  NVARCHAR(100) NOT NULL,
    LastName   NVARCHAR(100) NOT NULL,
    Email      NVARCHAR(256) NOT NULL UNIQUE,
    DateOfBirth DATE NULL,
    CreatedDate DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME()
);
GO

CREATE INDEX IX_Customers_LastName ON dbo.Customers(LastName, FirstName);
