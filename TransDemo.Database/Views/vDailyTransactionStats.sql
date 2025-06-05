CREATE VIEW dbo.vDailyTransactionStats
AS
SELECT
    CAST(CreatedAt AS DATE) AS TxnDate,
    COUNT(*) AS TxnCount,
    SUM(Amount) AS TotalAmount
FROM dbo.Transactions
GROUP BY CAST(CreatedAt AS DATE);

