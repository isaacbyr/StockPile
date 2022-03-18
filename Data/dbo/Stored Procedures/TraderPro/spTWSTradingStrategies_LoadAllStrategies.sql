CREATE PROCEDURE [dbo].[spTWSTradingStrategies_LoadAllStrategies]
	@UserId NVARCHAR(128)
AS
SET NOCOUNT ON;
BEGIN
	SELECT UserId, Ticker, BuyShares, SellShares, MA1, MA2, Indicator, Interval, [Range]
	FROM [dbo].[TWSTradingStrategies]
	WHERE UserId = @UserId
END
