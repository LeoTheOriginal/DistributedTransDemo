CREATE VIEW dbo.vDailyBalances
AS
WITH DailyNet AS (
    SELECT
        CAST(CreatedAt AS DATE) AS BalanceDate,
        SUM(CASE WHEN TransactionType = 'CREDIT' THEN Amount ELSE -Amount END) AS NetAmount
    FROM dbo.Transactions
    GROUP BY CAST(CreatedAt AS DATE)
)
SELECT
    BalanceDate,
    SUM(NetAmount) OVER (ORDER BY BalanceDate ROWS BETWEEN UNBOUNDED PRECEDING AND CURRENT ROW) AS CumulativeBalance
FROM DailyNet;
