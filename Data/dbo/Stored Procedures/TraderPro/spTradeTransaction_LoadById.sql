CREATE PROCEDURE [dbo].[spTradeTransaction_LoadById]
@UserId NVARCHAR(128)
AS
BEGIN
	SET NOCOUNT ON;
	SELECT *
	FROM [dbo].[TradeTransaction]
	WHERE UserId = @UserId
END
