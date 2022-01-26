CREATE PROCEDURE [dbo].[spPortfolio_PostStock]
	@UserId NVARCHAR(128),
	@Ticker NVARCHAR(10),
	@AveragePrice MONEY,
	@Shares INT
AS
BEGIN
	SET NOCOUNT ON;
	INSERT INTO [dbo].[Portfolio] (UserId, Ticker, AveragePrice, Shares)
	VALUES (@UserId, @Ticker, @AveragePrice, @Shares)
END