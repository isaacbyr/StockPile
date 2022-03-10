CREATE PROCEDURE [dbo].[spTradeTransaction_PostTransaction]
	@UserId NVARCHAR(128),
	@Shares INT,
	@Buy BIT,
	@Sell BIT,
	@Price MONEY,
	@Ticker NVARCHAR(10),
	@Date DATETIME2
AS
BEGIN
	SET NOCOUNT ON;
	INSERT INTO [dbo].[TradeTransaction] (UserId, Shares, Buy, Sell, Price, Ticker, [Date])
	VALUES (@UserId, @Shares, @Buy, @Sell, @Price, @Ticker, @Date)
END
