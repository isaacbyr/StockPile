CREATE PROCEDURE [dbo].[spStrategy_GetStrategyId]
	@UserId NVARCHAR(128)
AS
BEGIN
	SET NOCOUNT ON;

	SELECT TOP 1 Id
	FROM [dbo].Strategies
	WHERE UserId = @UserId
	Order By Id DESC
END