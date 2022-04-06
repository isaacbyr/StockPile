CREATE PROCEDURE [dbo].[spComments_LoadCommentsByTicker]
	@Ticker NVARCHAR(10)
AS
SET NOCOUNT ON;
BEGIN
	SELECT [c].Id, [c].Ticker, [c].Comment, [c].PostedAt, CONCAT([u].FirstName, ' ', [u].LastName) as 'FullName' FROM
	[dbo].[Comments] as [c]
	LEFT JOIN [dbo].[User] as [u] ON [c].UserId = [u].Id
	WHERE [c].Ticker = @Ticker
	ORDER BY [c].PostedAt DESC
END