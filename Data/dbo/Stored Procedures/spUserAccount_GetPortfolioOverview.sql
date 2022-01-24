CREATE PROCEDURE [dbo].[spUserAccount_GetPortfolioOverview]
	@UserId NVARCHAR(128)
AS
BEGIN
	SET NOCOUNT ON;
	SELECT StartAmount, AccountBalance
	FROM [dbo].[UserAccount]
	WHERE UserId = @UserId
END
