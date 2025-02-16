USE [d000540]
GO

/****** Object:  Table [dbo].[Pages]    Script Date: 2025-02-16 15:09:00 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Pages3](
	[PageId] [int] IDENTITY(1,1) NOT NULL,
	[Title] [nvarchar](255) NOT NULL,
	[Controller] [nvarchar](255) NOT NULL,
	[Action] [nvarchar](255) NOT NULL,
	[Tip] [nvarchar](255) NULL,
	[Position] [int] NOT NULL,
	[HasChild] [bit] NULL,
	[Changed] [datetime] NULL,
	[ChangedBy] [nvarchar](50) NULL,
 CONSTRAINT [PK_Pages3] PRIMARY KEY CLUSTERED 
(
	[PageId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO


