CREATE OR ALTER PROCEDURE [dbo].[sp_GetAccountStatement]
    @CompanyId INT,
    @FromDate DATE,
    @ToDate DATE,
    @AccountId INT = NULL,
    @EntityId INT = NULL,
    @TransactionType NVARCHAR(50) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    -- Validate parameters - either AccountId or EntityId must be provided
    IF (@AccountId IS NULL AND @EntityId IS NULL)
    BEGIN
        RAISERROR('Either @AccountId or @EntityId must be specified', 16, 1);
        RETURN;
    END

    -- Get AP/AR account IDs if this is an entity statement
    DECLARE @AccountsPayableId INT = NULL;
    DECLARE @AccountsReceivableId INT = NULL;
    
    IF (@EntityId IS NOT NULL)
    BEGIN
        -- Adjust this query based on how you identify AP/AR accounts in your system
        SELECT @AccountsPayableId = Id FROM Account 
        WHERE CompanyId = @CompanyId AND Type = 'Payable' AND IsActive = 1;
        
        SELECT @AccountsReceivableId = Id FROM Account 
        WHERE CompanyId = @CompanyId AND Type = 'Receivable' AND IsActive = 1;
        
        IF (@AccountsPayableId IS NULL OR @AccountsReceivableId IS NULL)
        BEGIN
            RAISERROR('Accounts Payable/Receivable not configured for this company', 16, 1);
            RETURN;
        END
    END

    -- Calculate opening balance (transactions before the from date)
    DECLARE @OpeningBalance DECIMAL(18, 2);
    
    IF (@AccountId IS NOT NULL)
    BEGIN
        -- Account-specific opening balance
        SELECT @OpeningBalance = ISNULL(SUM(
            CASE WHEN TransactionDirection = 'Debit' THEN Amount ELSE -Amount END
        ), 0)
        FROM AccountTransaction 
        WHERE AccountId = @AccountId 
          AND CompanyId = @CompanyId 
          AND IsActive = 1 
          AND TransactionDate < @FromDate;
    END
    ELSE
    BEGIN
        -- Entity-specific opening balance (using both AP and AR accounts)
        SELECT @OpeningBalance = ISNULL(SUM(
            CASE WHEN TransactionDirection = 'Debit' THEN Amount ELSE -Amount END
        ), 0)
        FROM AccountTransaction 
        WHERE EntityId = @EntityId
          AND AccountId IN (@AccountsPayableId, @AccountsReceivableId)
          AND CompanyId = @CompanyId 
          AND IsActive = 1 
          AND TransactionDate < @FromDate;
    END

    -- Get transactions with running balance
    -- First CTE: Filter transactions based on parameters
	SET @OpeningBalance = ISNULL(@OpeningBalance, 0);
    WITH FilteredTransactions AS (
        SELECT
            t.Id,
            t.CompanyId,
            t.AccountId,
            a.Name AS AccountName,
            t.TransactionType,
            t.TransactionDirection,
            t.Amount,
            CASE 
                WHEN t.TransactionDirection = 'Debit' THEN t.Amount
                WHEN t.TransactionDirection = 'Credit' THEN -t.Amount
            END AS SignedAmount,
            t.TransactionDate,
            t.PaymentMethod,
            t.Notes,
            t.EntityId,
            c.Name AS EntityName,
            t.ReferenceType,
            t.ReferenceId,
            t.Category,
            t.Party,
            t.ToAccountId,
            ta.Name AS ToAccountName,
            t.IsActive,
            t.CreatedDate,
            ROW_NUMBER() OVER (ORDER BY t.TransactionDate, t.Id) AS RowNum
        FROM AccountTransaction t
        INNER JOIN Account a ON a.Id = t.AccountId
        LEFT JOIN Account ta ON t.ToAccountId = ta.Id
        LEFT JOIN Customer c ON t.EntityId = c.Id
        WHERE 
            t.CompanyId = @CompanyId
            AND t.IsActive = 1
            AND t.TransactionDate BETWEEN @FromDate AND @ToDate
            AND (
                (@AccountId IS NOT NULL AND t.AccountId = @AccountId)
                OR
                (@EntityId IS NOT NULL AND t.EntityId = @EntityId 
                 AND t.AccountId IN (@AccountsPayableId, @AccountsReceivableId))
            )
            AND (@TransactionType IS NULL OR t.TransactionType = @TransactionType)
    ),
    -- Second CTE: Calculate running balances
    RunningBalances AS (
        SELECT
            *,
            @OpeningBalance + SUM(SignedAmount) OVER (
                ORDER BY RowNum
                ROWS BETWEEN UNBOUNDED PRECEDING AND CURRENT ROW
            ) AS RunningBalance
        FROM FilteredTransactions
    )
    -- Main query to return results
    SELECT
        Id,
        CompanyId,
        AccountId,
        AccountName,
        TransactionType,
        TransactionDirection,
        Amount,
        SignedAmount,
        TransactionDate,
        PaymentMethod,
        Notes,
        EntityId,
        EntityName,
        ReferenceType,
        ReferenceId,
        Category,
        Party,
        ToAccountId,
        ToAccountName,
        IsActive,
        CreatedDate,
        RunningBalance
    FROM RunningBalances
    ORDER BY TransactionDate, Id;

    -- Return metadata in a separate result set
    SELECT 
        @OpeningBalance AS OpeningBalance,
        @OpeningBalance + ISNULL((
            SELECT SUM(SignedAmount)
            FROM (
                SELECT
                    CASE 
                        WHEN TransactionDirection = 'Debit' THEN Amount
                        WHEN TransactionDirection = 'Credit' THEN -Amount
                    END AS SignedAmount
                FROM AccountTransaction t
                WHERE 
                    t.CompanyId = @CompanyId
                    AND t.IsActive = 1
                    AND t.TransactionDate BETWEEN @FromDate AND @ToDate
                    AND (
                        (@AccountId IS NOT NULL AND t.AccountId = @AccountId)
                        OR
                        (@EntityId IS NOT NULL AND t.EntityId = @EntityId 
                         AND t.AccountId IN (@AccountsPayableId, @AccountsReceivableId))
                    )
                    AND (@TransactionType IS NULL OR t.TransactionType = @TransactionType)
            ) AS tx
        ), 0) AS ClosingBalance,
        CASE 
            WHEN @AccountId IS NOT NULL THEN 
                (SELECT Name FROM Account WHERE Id = @AccountId)
            ELSE 
                (SELECT Name FROM Customer WHERE Id = @EntityId) + ' (AP/AR Statement)'
        END AS StatementTitle;
END
