CREATE PROCEDURE [dbo].[spTradesPortfolio_LoadTopHoldings]
	@userId NVARCHAR(256)
AS
SET NOCOUNT ON;
BEGIN
	SELECT TOP(3) Ticker, Shares 
	FROM [dbo].[TradesPortfolio]
	WHERE UserId = @userId
	ORDER BY Shares DESC
END