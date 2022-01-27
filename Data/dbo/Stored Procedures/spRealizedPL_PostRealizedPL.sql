CREATE PROCEDURE [dbo].[spRealizedPL_PostRealizedPL]
	@UserId NVARCHAR(128),
	@ProfitLoss MONEY
AS
DECLARE @Prevtotal MONEY;
BEGIN
	SET NOCOUNT ON;
	
SELECT TOP(1)  @Prevtotal = TotalRealized 
	FROM [dbo].[RealizedProfitLoss]
	WHERE UserId = @UserId
	ORDER BY [Date] DESC

INSERT INTO [dbo].[RealizedProfitLoss] (UserId, ProfitLoss, TotalRealized)
	VALUES (@UserId, @ProfitLoss, @PrevTotal + @ProfitLoss)

END
