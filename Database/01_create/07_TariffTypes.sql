USE [d002883]
GO

/****** Object:  Table [dbo].[TariffTypes]    Script Date: 2017-09-07 19:28:34 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[TariffTypes](
	[TariffTypeId] [int] IDENTITY(1,1) NOT NULL,
	[TariffType] [nvarchar](255) NOT NULL,
	[NoLevel] [bit] NULL,
	[Invisible] [bit] NULL,
	[BackgroundColor] [nvarchar](50) NULL,	
	[Changed] [datetime] NULL,
	[ChangedBy] [nvarchar](255) NULL,
 CONSTRAINT [PK_TariffTypes] PRIMARY KEY CLUSTERED 
(
	[TariffTypeId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO


