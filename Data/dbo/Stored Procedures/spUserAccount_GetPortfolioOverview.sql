CREATE PROCEDURE [dbo].[spUserAccount_GetPortfolioOverview]
	@UserId NVARCHAR(128)
AS
BEGIN
	SET NOCOUNT ON;
	SELECT [u].StartAmount, [u].PortfolioAccountBalance, [r].TotalRealized
	FROM [dbo].[UserAccount] AS u
	FULL JOIN [dbo].[RealizedProfitLoss] AS r ON [u].UserId = [r].UserId
	WHERE [u].UserId = @UserId
END
