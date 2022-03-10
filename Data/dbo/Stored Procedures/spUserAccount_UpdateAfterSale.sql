CREATE PROCEDURE [dbo].[spUserAccount_UpdateAfterSale]
	@UserId NVARCHAR(128),
	@RealizedProfitLoss MONEY,
	@SaleAmount MONEY
AS
BEGIN
	SET NOCOUNT ON;
	UPDATE [dbo].[UserAccount]
	SET PortfolioAccountBalance = (PortfolioAccountBalance + @RealizedProfitLoss + @SaleAmount)
	WHERE UserId = @UserId

	SELECT PortfolioAccountBalance
	FROM [dbo].[UserAccount]
	WHERE UserId = @UserId
END
