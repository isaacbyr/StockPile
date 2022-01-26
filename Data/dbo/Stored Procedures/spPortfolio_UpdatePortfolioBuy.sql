CREATE PROCEDURE [dbo].[spPortfolio_UpdatePortfolioBuy]
	@UserId NVARCHAR(128),
	@Shares INT,
	@Ticker NVARCHAR(10),
	@Price MONEY
AS
BEGIN
	SET NOCOUNT ON;
	UPDATE [dbo].[Portfolio]
	SET Shares =  Shares + @Shares, AveragePrice = ((AveragePrice * Shares) + (@Price * @Shares))/ (Shares + @Shares)
	WHERE [UserId] = @UserId AND Ticker = @Ticker
END
