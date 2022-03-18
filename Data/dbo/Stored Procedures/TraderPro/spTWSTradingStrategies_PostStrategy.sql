CREATE PROCEDURE [dbo].[spTWSTradingStrategies_PostStrategy]
	@UserId NVARCHAR(128),
	@Ticker NVARCHAR(10),
	@BuyShares INT,
	@SellShares INT,
	@MA1 NVARCHAR(10),
	@MA2 NVARCHAR(10),
	@Indicator NVARCHAR(20),
	@Interval NVARCHAR(10),
	@Range NVARCHAR(10)
AS
SET NOCOUNT ON;
BEGIN
	INSERT INTO [dbo].[TWSTradingStrategies] (UserId, Ticker, BuyShares, SellShares, MA1, MA2, Indicator, Interval, [Range])
	VALUES (@UserId, @Ticker, @BuyShares, @SellShares, @MA1, @MA2, @Indicator, @Interval, @Range)
END
