CREATE TABLE dbo.LoginHistory (
    LoginHistoryId INT IDENTITY(1,1) PRIMARY KEY,
    UserName       NVARCHAR(100) NOT NULL,
    LoginTime      DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    Success        BIT NOT NULL,
    IPAddress      NVARCHAR(45) NULL
);
