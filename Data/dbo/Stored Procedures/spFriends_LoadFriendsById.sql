CREATE PROCEDURE [dbo].[spFriends_LoadFriendsById]
	@FolloweeId NVARCHAR(128)
AS
BEGIN
	SET NOCOUNT ON;
	SELECT  [f].FollowerId as 'Id', [u].FirstName, [u].LastName
	FROM [dbo].[Friends] as [f]
	FULL JOIN [dbo].[User] as [u] ON [f].FollowerId = [u].Id
	WHERE [f].FolloweeId = @FolloweeId
END