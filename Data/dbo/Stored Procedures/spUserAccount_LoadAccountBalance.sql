CREATE PROCEDURE [dbo].[spUserAccount_LoadAccountBalance]
	@UserId NVARCHAR(128)
AS
BEGIN
	SET NOCOUNT ON;
	SELECT TradesAccountBalance FROM [dbo].[UserAccount]
	WHERE UserId = @UserId
END
