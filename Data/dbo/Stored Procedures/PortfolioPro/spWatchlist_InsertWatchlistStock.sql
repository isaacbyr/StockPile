CREATE PROCEDURE [dbo].[spWatchlist_InsertWatchlistStock]
	@UserId NVARCHAR(128),
	@Ticker NVARCHAR(10)
AS
BEGIN
	SET NOCOUNT ON;
	INSERT INTO [dbo].[WatchList] (UserId, Ticker)
	VALUES (@UserId, @Ticker)
END
