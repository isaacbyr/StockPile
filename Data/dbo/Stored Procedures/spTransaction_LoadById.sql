CREATE PROCEDURE [dbo].[spTransaction_LoadById]
	@UserId NVARCHAR(128)
AS
BEGIN
	SET NOCOUNT ON;
	SELECT *
	FROM [dbo].[Transactions]
	WHERE UserId = @UserId
END
