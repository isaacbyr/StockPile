CREATE PROCEDURE [dbo].[spTradesPortfolio_LoadPortfolioByUserId]
	@UserId NVARCHAR(128)
AS
BEGIN
	SET NOCOUNT ON;
	SELECT Ticker, Shares, AveragePrice
	FROM [dbo].[TradesPortfolio]
	WHERE UserId = @UserId
END
