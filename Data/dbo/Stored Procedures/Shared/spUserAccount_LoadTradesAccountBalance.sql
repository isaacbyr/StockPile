CREATE PROCEDURE [dbo].[spUserAccount_LoadTradesAccountBalance]
	@UserId NVARCHAR(128)
AS
BEGIN
	SET NOCOUNT ON;
	SELECT TradesAccountBalance FROM [dbo].[UserAccount]
	WHERE UserId = @UserId
END
