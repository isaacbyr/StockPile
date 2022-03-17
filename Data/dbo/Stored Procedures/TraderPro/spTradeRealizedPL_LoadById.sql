CREATE PROCEDURE [dbo].[spTradeRealizedPL_LoadById]
@Id NVARCHAR(128)
AS
BEGIN
	SET NOCOUNT ON;
	SELECT [u].Id, CONCAT(FirstName, ' ', LastName) as 'FullName', [r].ProfitLoss, [r].[Date], [r].TotalRealized
	FROM [dbo].[User] AS [u]
	FULL JOIN [dbo].[TradesRealizedProtitLoss] AS [r] ON [u].Id = [r].UserId
	WHERE Id = @Id
	ORDER BY [r].[Date] DESC
END
