CREATE PROCEDURE [dbo].[spRealizedPL_PostRealizedPL]
	@UserId NVARCHAR(128),
	@RealizedProfitLoss MONEY
AS
DECLARE @Prevtotal MONEY;
BEGIN
	SET NOCOUNT ON;
	
SELECT TOP(1)  @Prevtotal = TotalRealized 
	FROM [dbo].[RealizedProfitLoss]
	WHERE UserId = @UserId
	ORDER BY [Date] DESC

INSERT INTO [dbo].[RealizedProfitLoss] (UserId, ProfitLoss, TotalRealized)
	VALUES (@UserId, @RealizedProfitLoss, @PrevTotal + @RealizedProfitLoss)

END
