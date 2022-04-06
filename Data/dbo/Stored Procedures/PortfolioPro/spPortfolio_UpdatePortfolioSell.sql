CREATE PROCEDURE [dbo].[spPortfolio_UpdatePortfolioSell]
	@UserId NVARCHAR(128),
	@Ticker NVARCHAR(10),
	@Shares INT,
	@Price MONEY
AS
BEGIN 
SET NOCOUNT ON;

SELECT (@Price - AveragePrice) * @Shares as 'ProfitLoss'	
	FROM [dbo].[Portfolio]
	WHERE UserId = @UserId  AND Ticker = @Ticker

UPDATE [dbo].[Portfolio]
	SET Shares = Shares - @Shares
	WHERE UserId = @UserId AND Ticker = @Ticker

	
END
