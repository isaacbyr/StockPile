CREATE PROCEDURE [dbo].[spUserAccount_UpdateTradesAfterSale]
@UserId NVARCHAR(128),
	@RealizedProfitLoss MONEY,
	@SaleAmount MONEY
AS
BEGIN
	SET NOCOUNT ON;
	UPDATE [dbo].[UserAccount]
	SET TradesAccountBalance = (TradesAccountBalance + @RealizedProfitLoss + @SaleAmount)
	WHERE UserId = @UserId

	SELECT TradesAccountBalance
	FROM [dbo].[UserAccount]
	WHERE UserId = @UserId
END
