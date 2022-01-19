CREATE PROCEDURE [dbo].[spUser_GetUserById]
	@Id NVARCHAR(128)
AS
BEGIN
	SET NOCOUNT ON;
	SELECT Id, FirstName, LastName, Email
	FROM [dbo].[User]
	WHERE Id = @Id
END
