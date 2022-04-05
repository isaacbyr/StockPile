CREATE PROCEDURE [dbo].[spPortfolio_LoadPortfolioByUserId]
	@UserId NVARCHAR(128)
AS
BEGIN
	SET NOCOUNT ON;
	SELECT Ticker, Shares, AveragePrice
	FROM [dbo].[Portfolio]
	WHERE UserId = @UserId
END
