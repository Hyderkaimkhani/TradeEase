CREATE TABLE [dbo].[UserAuth]
(
	[Id]                     INT           IDENTITY (1, 1) NOT NULL,
    [UserId]                 INT           NOT NULL,
    [RefreshToken]           VARCHAR (MAX) NOT NULL,
    [JWTToken]               VARCHAR (MAX) NOT NULL,
    [CreatedDate]            DATETIME      NOT NULL,
    [UpdatedDate]            DATETIME      NULL,
    [RefreshTokenValidTill]  DATETIME      NOT NULL,
    CONSTRAINT [PK_UserAuth] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_UserAuth_Users] FOREIGN KEY ([UserId]) REFERENCES [dbo].[User] ([Id])
)
