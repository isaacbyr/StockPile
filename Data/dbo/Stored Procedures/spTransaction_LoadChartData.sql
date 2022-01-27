CREATE PROCEDURE [dbo].[spTransaction_LoadChartData]
	@UserId NVARCHAR(128)
AS
BEGIN
	SET NOCOUNT ON;
	SELECT [Date], COUNT(*) As 'Count'
	FROM [dbo].[Transactions]
	WHERE UserId = @UserId
	GROUP BY [Date]
END
