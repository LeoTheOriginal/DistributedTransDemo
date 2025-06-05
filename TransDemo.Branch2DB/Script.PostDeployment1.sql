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

-- === Klienci lokalni (z centrali BranchId = 2) ===
INSERT INTO dbo.BranchClients (CentralClientId, LocalAccountNo, Balance, LastSync)
SELECT CustomerId, 'BR2_' + RIGHT(AccountNumber, 4), Balance, SYSUTCDATETIME()
FROM BankCentral.dbo.Accounts
WHERE BranchId = 2;
GO

-- === Transakcje lokalne ===
INSERT INTO dbo.BranchTransactions (BranchClientId, Amount, TxnType, Description, CreatedAt)
SELECT bc.BranchClientId, 75.00, 'DEBIT', 'Opłata lokalna', DATEADD(DAY, -1, SYSUTCDATETIME())
FROM dbo.BranchClients bc;
GO

-- === Historia ===
INSERT INTO dbo.History (Info)
VALUES
('Dane klientów z centrali zsynchronizowane'),
('Utworzono transakcje lokalne na podstawie danych centralnych');
GO
