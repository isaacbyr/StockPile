CREATE PROCEDURE [dbo].[spTradeTransaction_LoadCurrent]
	@UserId NVARCHAR(128)
AS
BEGIN
	SET NOCOUNT ON;
	SELECT *
	FROM [dbo].[TradeTransaction]
	WHERE UserId = @UserId AND [Date] >= DATEADD(day,-1, GETDATE())
END
