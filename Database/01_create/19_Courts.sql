USE [d002883]
GO

/****** Object:  Table [dbo].[Courts]    Script Date: 2017-09-07 19:52:02 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Courts](
	[CourtId] [int] IDENTITY(1,1) NOT NULL,
	[CourtName] [nvarchar](255) NOT NULL,
	[StreetName] [nvarchar](255) NULL,
	[StreetNumber] [nvarchar](10) NULL,
	[PostalCode] [nvarchar](20) NULL,
	[PostalAddress] [nvarchar](255) NULL,
	[Country] [nvarchar](255) NULL,
	[Changed] [datetime] NULL,
	[ChangedBy] [nvarchar](255) NULL,
 CONSTRAINT [PK_Courts] PRIMARY KEY CLUSTERED 
(
	[CourtId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO


