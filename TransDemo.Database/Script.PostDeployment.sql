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
-- === Typy kont ===
INSERT INTO dbo.AccountTypes (TypeName, Description)
VALUES
('Osobiste', 'Standardowe konto osobiste'),
('Firmowe', 'Konto dla działalności gospodarczej'),
('Oszczędnościowe', 'Konto do oszczędzania z oprocentowaniem');
GO

-- === Oddziały ===
INSERT INTO dbo.Branches (BranchName, Address)
VALUES
('Oddział Centrum', 'ul. Marszałkowska 1, Warszawa'),
('Oddział Wschód', 'ul. Lubelska 42, Lublin');
GO

-- === Klienci ===
INSERT INTO dbo.Customers (FirstName, LastName, Email, DateOfBirth)
VALUES
('Jan', 'Kowalski', 'jan.k@example.com', '1980-01-15'),
('Anna', 'Nowak', 'anna.n@example.com', '1991-04-12'),
('Piotr', 'Wiśniewski', 'piotr.w@example.com', '1975-09-30'),
('Magda', 'Mazur', 'magda.mazur@example.com', '1988-07-19'),
('Kamil', 'Wójcik', 'kamil.w@example.com', '1993-02-27');
GO

-- === Konta klientów ===
INSERT INTO dbo.Accounts (CustomerId, AccountNumber, Balance, BranchId, AccountTypeId)
VALUES
(1, 'PL001122334455667788990001', 10250.50, 1, 1),
(2, 'PL001122334455667788990002', 15300.00, 1, 3),
(3, 'PL001122334455667788990003', 74200.00, 2, 2),
(4, 'PL001122334455667788990004', 1100.00, 2, 1),
(5, 'PL001122334455667788990005', 580.00, 2, 1);
GO

-- === Transakcje kontowe ===
INSERT INTO dbo.Transactions (AccountId, BranchId, Amount, TransactionType, Description, CreatedAt)
VALUES
(1, 1, 250.50, 'CREDIT', 'Wpłata gotówki', DATEADD(DAY, -10, SYSUTCDATETIME())),
(1, 1, 500.00, 'DEBIT', 'Płatność za zakupy', DATEADD(DAY, -5, SYSUTCDATETIME())),
(2, 1, 15000.00, 'CREDIT', 'Przelew oszczędności', DATEADD(DAY, -20, SYSUTCDATETIME())),
(3, 2, 4200.00, 'CREDIT', 'Wpłata z działalności', DATEADD(DAY, -15, SYSUTCDATETIME())),
(3, 2, 200.00, 'DEBIT', 'Zakup wyposażenia', DATEADD(DAY, -1, SYSUTCDATETIME())),
(5, 2, 100.00, 'DEBIT', 'Opłata za subskrypcję', DATEADD(DAY, -2, SYSUTCDATETIME()));
GO

-- === Historia logowań ===
INSERT INTO dbo.LoginHistory (UserName, LoginTime, Success, IPAddress)
VALUES
('BranchAppUser',   DATEADD(HOUR, -8, SYSUTCDATETIME()), 1, '192.168.1.101'),
('BranchAppUser',   DATEADD(HOUR, -3, SYSUTCDATETIME()), 0, '192.168.1.101'),
('Auditor',         DATEADD(HOUR, -1, SYSUTCDATETIME()), 1, '10.10.10.5');
GO


EXEC dbo.sp_TransferDistributed
     @FromAccId = 1,
     @ToAccId   = 3,
     @Amount    = 100;
GO


EXEC dbo.sp_TransferDistributed
     @FromAccId = 2,
     @ToAccId   = 5,
     @Amount    = 50;
GO