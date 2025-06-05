/*
Post-Deployment Script Template							
--------------------------------------------------------------------------------------
 This file contains SQL statements that will be appended to the build script.		
 Use SQLCMD syntax to include a file in the post-deployment script.			
 Example:      :r .\myfile.sql								
 Use SQLCMD syntax to reference a variable in the post-deployment script.		
 Example:      :setvar TableName MyTable							
               SELECT * FROM [$(TableName)]					
--------------------------------------------------------------------------------------
*/

-- === Klienci lokalni (z centrali BranchId = 1) ===
INSERT INTO dbo.BranchClients (CentralClientId, LocalAccountNo, Balance, LastSync)
SELECT CustomerId, 'BR1_' + RIGHT(AccountNumber, 4), Balance, SYSUTCDATETIME()
FROM BankCentral.dbo.Accounts
WHERE BranchId = 1;
GO

-- === Transakcje lokalne ===
INSERT INTO dbo.BranchTransactions (BranchClientId, Amount, TxnType, Description, CreatedAt)
SELECT bc.BranchClientId, 150.00, 'CREDIT', 'Wpłata bankomatowa', DATEADD(DAY, -2, SYSUTCDATETIME())
FROM dbo.BranchClients bc;
GO

-- === Historia ===
INSERT INTO dbo.History (Info)
VALUES
('Synchronizacja kont lokalnych zakończona sukcesem'),
('Dodano nowe transakcje klientów z centrali');
GO
