CREATE PROCEDURE [dbo].[spUser_InsertNewUser]
	@Id NVARCHAR(128),
	@FirstName NVARCHAR(50),
	@LastName NVARCHAR(50),
	@Email NVARCHAR(50)
AS
BEGIN
	SET NOCOUNT ON;
	INSERT INTO [dbo].[User] (Id, FirstName, LastName, Email)
	VALUES (@Id, @FirstName, @LastName, @Email)
END
