CREATE PROCEDURE [dbo].[spPortfolio_GetStockByTicker]
	@UserId NVARCHAR(128),
	@Ticker NVARCHAR(10)
AS
BEGIN
	SET NOCOUNT ON;
	SELECT Ticker, Shares, AveragePrice
	FROM [dbo].[Portfolio]
	WHERE UserId = @UserId AND Ticker = @Ticker
END
