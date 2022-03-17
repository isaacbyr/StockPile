CREATE PROCEDURE [dbo].[spTradeTransaction_LoadChartData]
	@UserId NVARCHAR(128)
AS
BEGIN
	SET NOCOUNT ON;
	SELECT CONVERT(NVARCHAR(10), [Date], 120) as 'Date', COUNT(*) As 'Count'
	FROM [dbo].[TradeTransaction]
	WHERE UserId = '3c0056da-6bfa-40f5-81cf-b0e34b8a198f'
GROUP BY CONVERT(NVARCHAR(10), [Date], 120)
	
END
