CREATE PROCEDURE [dbo].[spTradeRealizedPL_LoadHistoryByUser]
@UserId NVARCHAR(128)
AS
BEGIN
	SET NOCOUNT ON;
	SELECT [Date], [TotalRealized], ProfitLoss
	FROM [dbo].[TradesRealizedProtitLoss]
	WHERE UserId = @UserId
	ORDER BY [Date]
END;
