CREATE PROCEDURE [dbo].[spStrategy_PostStrategy]
	@UserId NVARCHAR(128),
	@Name NVARCHAR(100),
	@MA1 NVARCHAR(5),
	@MA2 NVARCHAR(5),
	@Indicator NVARCHAR(10),
	@Interval NVARCHAR(5),
	@Range NVARCHAR(5)
AS
BEGIN
	SET NOCOUNT ON;
	INSERT INTO [dbo].Strategies (UserId, [Name], MA1, MA2, Indicator , Interval, [Range])
	VALUES (@UserId, @Name,  @MA1, @MA2, @Indicator , @Interval, @Range)

		SELECT TOP 1 Id
	FROM [dbo].Strategies
	Order By Id DESC
END