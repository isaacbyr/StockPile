CREATE PROCEDURE [dbo].[spComments_LoadTrending]
AS
BEGIN
	SET NOCOUNT ON;
	SELECT Ticker, Count(*) as 'Count' FROM [dbo].[Comments]
	WHERE [PostedAt] >= DATEADD(day,-1, GETDATE())
	GROUP BY Ticker
	ORDER BY count DESC

END
