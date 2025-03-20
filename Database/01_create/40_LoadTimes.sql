USE [d000540]
GO

/****** Object:  Table [dbo].[LoadTimes]    Script Date: 2025-03-20 11:52:44 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[LoadTimes](
	[LoadTimeId] [int] IDENTITY(1,1) NOT NULL,
	[Host] [nvarchar](50) NOT NULL,
	[Controller] [nvarchar](50) NOT NULL,
	[Action] [nvarchar](50) NOT NULL,
	[LoadTime] [int] NOT NULL,
	[Changed] [datetime] NULL,
	[ChangedBy] [nvarchar](50) NULL,
 CONSTRAINT [PK_LoadTimes] PRIMARY KEY CLUSTERED 
(
	[LoadTimeId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO


