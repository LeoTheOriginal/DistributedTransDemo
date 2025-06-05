CREATE VIEW dbo.vBranchTransactionShares
AS
WITH Total AS (
    SELECT SUM(Amount) AS TotalAmount FROM dbo.Transactions
)
SELECT
    b.BranchName,
    COUNT(t.TransactionId) AS TxnCount,
    SUM(t.Amount) AS BranchAmount,
    CAST(100.0 * SUM(t.Amount) / tAll.TotalAmount AS DECIMAL(5,2)) AS SharePercent
FROM dbo.Transactions t
JOIN dbo.Branches b ON t.BranchId = b.BranchId
CROSS JOIN Total tAll
GROUP BY b.BranchName, tAll.TotalAmount;
