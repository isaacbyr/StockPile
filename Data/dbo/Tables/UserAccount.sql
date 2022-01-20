CREATE TABLE [dbo].[UserAccount]
(
	[UserId] NVARCHAR(128) NOT NULL PRIMARY KEY,
	[StartAmount] INT NOT NULL,
	[AccountBalance] INT NOT NULL, 
    CONSTRAINT [FK_UserAccount_User] FOREIGN KEY ([UserId]) REFERENCES [User]([Id])
)
