CREATE PROCEDURE [dbo].[spComments_LoadAllComments]
AS
BEGIN
	SET NOCOUNT ON;
	SELECT [c].Id, [c].Ticker, [c].Comment, [c].PostedAt, CONCAT([u].FirstName, ' ', [u].LastName) as 'FullName' FROM
	[dbo].[Comments] as [c]
	LEFT JOIN [dbo].[User] as [u] ON [c].UserId = [u].Id
	ORDER BY [c].PostedAt DESC
END;
