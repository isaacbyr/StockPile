CREATE PROCEDURE [dbo].[spTradesPortfolio_UpdatePortfolioBuy]
	@UserId NVARCHAR(128),
	@Shares INT,
	@Ticker NVARCHAR(10),
	@Price MONEY
AS
BEGIN
	SET NOCOUNT ON;
	UPDATE [dbo].[TradesPortfolio]
	SET Shares =  Shares + @Shares, AveragePrice = ((AveragePrice * Shares) + (@Price * @Shares))/ (Shares + @Shares)
	WHERE [UserId] = @UserId AND Ticker = @Ticker
END
