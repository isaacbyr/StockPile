CREATE PROCEDURE [dbo].[spStrategy_GetAll]
	@UserId NVARCHAR(128)
AS
BEGIN 
	SET NOCOUNT ON;
	SELECT * FROM [dbo].[Strategies]
	WHERE UserId = @UserId
END
