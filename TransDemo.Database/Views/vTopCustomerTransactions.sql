CREATE VIEW dbo.vTopCustomerTransactions
AS
SELECT
    c.FirstName + ' ' + c.LastName AS FullName,
    SUM(t.Amount) AS TotalAmount
FROM dbo.Customers c
JOIN dbo.Accounts a ON c.CustomerId = a.CustomerId
JOIN dbo.Transactions t ON a.AccountId = t.AccountId
GROUP BY c.FirstName, c.LastName;
