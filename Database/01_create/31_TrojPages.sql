USE [d002883]
GO

/****** Object:  Table [dbo].[TrojPages]    Script Date: 2018-11-30 19:40:36 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[TrojPages](
	[PageId] [int] IDENTITY(1,1) NOT NULL,
	[PageLink] [nvarchar](512) NULL,
	[AccessTime] [datetime] NULL,
	[UserName] [nvarchar](512) NULL,
 CONSTRAINT [PK_TrojPages] PRIMARY KEY CLUSTERED 
(
	[PageId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
