CREATE VIEW dbo.vAccountBalances
AS
SELECT
    a.AccountId,
    a.AccountNumber,
    c.FirstName,
    c.LastName,
    b.BranchName,
    at.TypeName AS AccountType,
    a.Balance,
    MAX(t.CreatedAt) AS LastTxnDate
FROM dbo.Accounts a
JOIN dbo.Customers c ON a.CustomerId = c.CustomerId
JOIN dbo.Branches b ON a.BranchId = b.BranchId
JOIN dbo.AccountTypes at ON a.AccountTypeId = at.AccountTypeId
LEFT JOIN dbo.Transactions t ON a.AccountId = t.AccountId
GROUP BY
    a.AccountId,
    a.AccountNumber,
    c.FirstName,
    c.LastName,
    b.BranchName,
    at.TypeName,
    a.Balance;
