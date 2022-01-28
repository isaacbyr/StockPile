CREATE PROCEDURE [dbo].[spUser_FriendSearch]
	@Keyword NVARCHAR(20)
AS
BEGIN
	SET NOCOUNT ON;
	SELECT Id, FirstName, LastName
	FROM [dbo].[User]
	Where FirstName LIKE @Keyword OR LastName LIKE @Keyword
END;
