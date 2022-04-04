CREATE PROCEDURE [dbo].[spComments_PostNewComment]
	@Ticker NVARCHAR(10),
	@UserId NVARCHAR(128),
	@Comment NVARCHAR(256)
AS
BEGIN
	SET NOCOUNT ON;
	INSERT INTO [dbo].Comments (Ticker, UserId, Comment)
	VALUES (@Ticker, @UserId, @Comment)
END
