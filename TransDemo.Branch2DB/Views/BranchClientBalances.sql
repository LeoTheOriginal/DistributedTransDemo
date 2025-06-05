CREATE VIEW dbo.vBranchClientBalances AS
SELECT
    bc.BranchClientId,
    bc.LocalAccountNo,
    bc.Balance,
    MAX(bt.CreatedAt) AS LastTxn
FROM dbo.BranchClients bc
LEFT JOIN dbo.BranchTransactions bt ON bc.BranchClientId = bt.BranchClientId
GROUP BY bc.BranchClientId, bc.LocalAccountNo, bc.Balance;
GO