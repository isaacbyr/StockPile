CREATE PROCEDURE [dbo].[spFriendRequests_LoadRequestsById]
	@FolloweeId NVARCHAR(128)
AS
BEGIN
	SET NOCOUNT ON;
	SELECT  [f].FollowerId as 'Id', [u].FirstName, [u].LastName
	FROM [dbo].[FriendRequests] as [f]
	FULL JOIN [dbo].[User] as [u] ON [f].FollowerId = [u].Id
	WHERE [f].FolloweeId = @FolloweeId
END