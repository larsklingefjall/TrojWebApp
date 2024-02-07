USE [240470-vox]
GO

/****** Object:  Table [dbo].[UnderlayTitle]    Script Date: 2020-03-07 10:42:37 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[UnderlayTitle](
	[UnderlayTitleId] [int] IDENTITY(1,1) NOT NULL,
	[Title] [nvarchar](512) NULL,
 CONSTRAINT [PK_UnderlayTitleId] PRIMARY KEY CLUSTERED 
(
	[UnderlayTitleId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO


