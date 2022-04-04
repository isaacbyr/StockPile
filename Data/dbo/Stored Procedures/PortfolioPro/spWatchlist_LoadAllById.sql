CREATE PROCEDURE [dbo].[spWatchlist_LoadAllById]
	@UserId NVARCHAR(128)
AS
BEGIN
	SET NOCOUNT ON;
	SELECT Ticker, UserId
	FROM [dbo].[WatchList]
	WHERE UserId = @UserId
END
