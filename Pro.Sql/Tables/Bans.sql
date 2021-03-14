﻿SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [Pro].[Bans](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[UserID] [int] NOT NULL,
	[BanReason] [tinyint] NOT NULL,
	[Description] [nvarchar](255) NOT NULL,
 CONSTRAINT [PK_Bans] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [Pro].[Bans]  WITH CHECK ADD  CONSTRAINT [FK_Bans_Users] FOREIGN KEY([UserID])
REFERENCES [Pro].[Users] ([Id])
GO

ALTER TABLE [Pro].[Bans] CHECK CONSTRAINT [FK_Bans_Users]
GO