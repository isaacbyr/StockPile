CREATE PROCEDURE [dbo].[spRealizedPL_LoadHistoryByUser]
	@UserId NVARCHAR(128)
AS
BEGIN
	SET NOCOUNT ON;
	SELECT [Date], [TotalRealized], ProfitLoss
	FROM [dbo].[RealizedProfitLoss]
	WHERE UserId = @UserId
	ORDER BY [Date]
END;
