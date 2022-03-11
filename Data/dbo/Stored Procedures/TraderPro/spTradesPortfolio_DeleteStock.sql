CREATE PROCEDURE [dbo].[spTradesPortfolio_DeleteStock]
@UserId NVARCHAR(128),
	@Ticker NVARCHAR(10)
AS
BEGIN
	SET NOCOUNT ON;
	DELETE FROM [dbo].[TradesPortfolio]
	WHERE UserId = @UserId AND Ticker = @Ticker
END
