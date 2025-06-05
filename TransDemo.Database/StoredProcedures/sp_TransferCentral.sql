CREATE PROCEDURE dbo.sp_TransferCentral
    @FromAccId INT,
    @ToAccId   INT,
    @Amount    MONEY
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        BEGIN TRANSACTION;

        DECLARE @FromBalance MONEY;
        SELECT @FromBalance = Balance FROM dbo.Accounts WHERE AccountId = @FromAccId;

        IF @FromBalance < @Amount
        BEGIN
 
            ROLLBACK TRANSACTION;
            RAISERROR(N'Niewystarczaj?ce ?rodki na ko?cie ?ród?owym.', 16, 1);
            RETURN;
        END

        UPDATE dbo.Accounts
           SET Balance = Balance - @Amount
         WHERE AccountId = @FromAccId;
        UPDATE dbo.Accounts
           SET Balance = Balance + @Amount
         WHERE AccountId = @ToAccId;

        INSERT INTO dbo.Transactions(AccountId, BranchId, Amount, TransactionType, Description)
        VALUES
            (@FromAccId, (SELECT BranchId FROM dbo.Accounts WHERE AccountId = @FromAccId), @Amount, 'DEBIT', 'Central transfer'),
            (@ToAccId,   (SELECT BranchId FROM dbo.Accounts WHERE AccountId = @ToAccId),   @Amount, 'CREDIT','Central transfer');

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;

        DECLARE @ErrMsg NVARCHAR(4000) = ERROR_MESSAGE();
        RAISERROR(@ErrMsg, 16, 1);
    END CATCH
END
