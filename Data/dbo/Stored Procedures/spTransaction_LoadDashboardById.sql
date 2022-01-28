CREATE PROCEDURE [dbo].[spTransaction_LoadDashboardById]
	@Id NVARCHAR(128)
AS
BEGIN
	SET NOCOUNT ON;
	SELECT [u].Id, CONCAT(FirstName, ' ', LastName) as 'FullName', [t].Ticker, [t].Shares,
	[t].[Date], [t].Price, [t].Buy, [t].Sell
	FROM [dbo].[User] AS [u]
	FULL JOIN [dbo].[Transactions] AS [t] ON [u].Id = [t].UserId
	WHERE [u].Id = @Id
	ORDER BY [t].[Date] DESC
END