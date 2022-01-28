CREATE PROCEDURE [dbo].[spFriendRequests_DeleteRequest]
	@FolloweeId NVARCHAR(128),
	@FollowerId NVARCHAR(128)
AS
BEGIN
	SET NOCOUNT ON;
	DELETE FROM [dbo].[FriendRequests]
	WHERE FolloweeId = @FolloweeId AND FollowerId = @FollowerId
END
