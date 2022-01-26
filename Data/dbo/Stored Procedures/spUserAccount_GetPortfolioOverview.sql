CREATE PROCEDURE [dbo].[spUserAccount_GetPortfolioOverview]
	@UserId NVARCHAR(128)
AS
BEGIN
	SET NOCOUNT ON;
	SELECT StartAmount, AccountBalance, RealizedProfitLoss
	FROM [dbo].[UserAccount]
	WHERE UserId = @UserId
END
