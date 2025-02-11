USE [d000540]
GO

/****** Object:  Table [dbo].[MenuPages]    Script Date: 2025-02-11 11:17:17 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[MenuPages](
	[MenuPageId] [int] IDENTITY(1,1) NOT NULL,
	[Title] [nvarchar](255) NOT NULL,
	[Controller] [nvarchar](255) NOT NULL,
	[Action] [nvarchar](255) NOT NULL,
	[Id] [int] NOT NULL,
	[Changed] [datetime] NULL,
	[ChangedBy] [nvarchar](50) NULL,
 CONSTRAINT [PK_MenuPages] PRIMARY KEY CLUSTERED 
(
	[MenuPageId] DESC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO


