CREATE PROCEDURE [dbo].[spUserAccount_GetTradesPortfolioOverview]
@UserId NVARCHAR(128)
AS
BEGIN
	SET NOCOUNT ON;
	SELECT [u].StartAmount, [u].TradesAccountBalance as 'AccountBalance', [r].TotalRealized
	FROM [dbo].[UserAccount] AS u
	FULL JOIN [dbo].[TradesRealizedProtitLoss] AS r ON [u].UserId = [r].UserId
	WHERE [u].UserId = @UserId
END
