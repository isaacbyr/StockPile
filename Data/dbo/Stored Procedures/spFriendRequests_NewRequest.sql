CREATE PROCEDURE [dbo].[spFriendRequests_NewRequest]
	@FolloweeId NVARCHAR(128),
	@FollowerId NVARCHAR(128)
AS
BEGIN
	SET NOCOUNT ON;
	INSERT INTO [dbo].[FriendRequests] (FolloweeId, FollowerId)
	VALUES (@FolloweeId, @FollowerId)
END
