CREATE PROCEDURE [dbo].[spFriends_PostFriendship]
	@FolloweeId NVARCHAR(128),
	@FollowerId NVARCHAR(128)
AS
BEGIN
	SET NOCOUNT ON;
	INSERT INTO [dbo].[Friends] (FolloweeId, FollowerId)
	VALUES (@FolloweeId, @FollowerId)
END
