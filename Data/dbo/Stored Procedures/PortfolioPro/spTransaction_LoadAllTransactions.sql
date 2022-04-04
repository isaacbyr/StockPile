CREATE PROCEDURE [dbo].[spTransaction_LoadAllTransactions]
AS
BEGIN
	SET NOCOUNT ON;
	SELECT [T].Id, [T].Ticker, [T].Shares, [T].Buy, [T].Sell, [T].Price, [T].[Date], CONCAT([u].FirstName, ' ', [u].LastName) as 'FullName' FROM
	[dbo].[Transactions] as [T]
	LEFT JOIN [dbo].[User] as [u] ON [T].UserId = [u].Id
	ORDER BY [T].[Date] DESC
END;
