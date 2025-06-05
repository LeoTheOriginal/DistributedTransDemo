CREATE PROCEDURE dbo.sp_TransferDistributed
    @FromAccId INT,
    @ToAccId   INT,
    @Amount    MONEY
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN DISTRIBUTED TRANSACTION;
    BEGIN TRY
        -- 1) Centralny etap
        EXEC dbo.sp_TransferCentral @FromAccId, @ToAccId, @Amount;

        --------------------------------------------------
        -- 2) Oddzia? 1 (Branch1DB przez link BR1_LINKED)
        --------------------------------------------------
        DECLARE
            @cidFrom INT,
            @sql1    NVARCHAR(MAX),
            @params1 NVARCHAR(100);

        SELECT @cidFrom = CustomerId
          FROM dbo.Accounts
         WHERE AccountId = @FromAccId;

        SET @sql1 = N'
            UPDATE dbo.BranchClients
                SET Balance = Balance - @amt,
                    LastSync = SYSUTCDATETIME()
                WHERE CentralClientId = @cid;

            INSERT INTO dbo.BranchTransactions (BranchClientId, Amount, TxnType, Description)
            SELECT BranchClientId, @amt, N''DEBIT'', N''Transfer out''
              FROM dbo.BranchClients
             WHERE CentralClientId = @cid;

            INSERT INTO dbo.History(Info)
                VALUES(N''Obci??enie konta w wyniku transferu rozproszonego – '' + CAST(@amt AS NVARCHAR));
        ';
        SET @params1 = N'@amt MONEY, @cid INT';

        EXEC [BR1_LINKED].Branch1DB.dbo.sp_executesql
            @stmt   = @sql1,
            @params = @params1,
            @amt    = @Amount,
            @cid    = @cidFrom;

        --------------------------------------------------
        -- 3) Oddzia? 2 (Branch2DB przez link BR2_LINKED)
        --------------------------------------------------
        DECLARE
            @cidTo  INT,
            @sql2   NVARCHAR(MAX),
            @params2 NVARCHAR(100) = N'@amt MONEY, @cid INT';

        SELECT @cidTo = CustomerId
          FROM dbo.Accounts
         WHERE AccountId = @ToAccId;

        SET @sql2 = N'
            UPDATE dbo.BranchClients
               SET Balance = Balance + @amt,
                    LastSync = SYSUTCDATETIME()
                WHERE CentralClientId = @cid;

            INSERT INTO dbo.BranchTransactions (BranchClientId, Amount, TxnType, Description)
            SELECT BranchClientId, @amt, N''CREDIT'', N''Transfer in''
              FROM dbo.BranchClients
             WHERE CentralClientId = @cid;
            INSERT INTO dbo.History(Info)
                    VALUES(N''Uznanie konta w wyniku transferu rozproszonego – '' + CAST(@amt AS NVARCHAR));
        ';

        EXEC [BR2_LINKED].Branch2DB.dbo.sp_executesql
            @stmt   = @sql2,
            @params = @params2,
            @amt    = @Amount,
            @cid    = @cidTo;

        -- 4) Commit 2PC
        COMMIT TRANSACTION;
        DECLARE
            @fromName NVARCHAR(200),
            @toName NVARCHAR(200),
            @fromAccNo VARCHAR(34),
            @toAccNo   VARCHAR(34);

        -- Dane nadawcy
        SELECT
            @fromName = c.FirstName + ' ' + c.LastName,
            @fromAccNo = a.AccountNumber
        FROM dbo.Accounts a
        JOIN dbo.Customers c ON a.CustomerId = c.CustomerId
        WHERE a.AccountId = @FromAccId;

        -- Dane odbiorcy
        SELECT
            @toName = c.FirstName + ' ' + c.LastName,
            @toAccNo = a.AccountNumber
        FROM dbo.Accounts a
        JOIN dbo.Customers c ON a.CustomerId = c.CustomerId
        WHERE a.AccountId = @ToAccId;

        -- Wpis do historii
        INSERT INTO dbo.History(Info)
        VALUES (
            N'Transfer z ' + @fromName + ' (' + @fromAccNo + ')'
            + N' do ' + @toName + ' (' + @toAccNo + ')'
            + N' - kwota: ' + CAST(@Amount AS NVARCHAR)
        );
    END TRY
    BEGIN CATCH
        IF XACT_STATE() <> 0
            ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END;