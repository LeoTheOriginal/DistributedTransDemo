CREATE TABLE dbo.AccountTypes (
    AccountTypeId INT IDENTITY(1,1) PRIMARY KEY,
    TypeName   NVARCHAR(50) NOT NULL UNIQUE,
    Description NVARCHAR(200) NULL
);
