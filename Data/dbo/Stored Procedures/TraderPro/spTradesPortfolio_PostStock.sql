CREATE PROCEDURE [dbo].[spTradesPortfolio_PostStock]
	@UserId NVARCHAR(128),
	@Ticker NVARCHAR(10),
	@AveragePrice MONEY,
	@Shares INT
AS
BEGIN
	SET NOCOUNT ON;
	INSERT INTO [dbo].[TradesPortfolio] (UserId, Ticker, AveragePrice, Shares)
	VALUES (@UserId, @Ticker, @AveragePrice, @Shares)
END