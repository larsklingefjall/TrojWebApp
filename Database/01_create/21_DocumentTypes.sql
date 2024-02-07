USE [d002883]
GO

/****** Object:  Table [dbo].[DocumentTypes]    Script Date: 2017-09-07 19:57:00 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[DocumentTypes](
	[DocumentTypeId] [int] IDENTITY(1,1) NOT NULL,
	[DocumentType] [nvarchar](255) NULL,
	[Extension] [nvarchar](10) NULL,
	[ContentType] [nvarchar](25) NULL,
	[Changed] [datetime] NULL,
	[ChangedBy] [nvarchar](255) NULL,
 CONSTRAINT [PK_DocumentTypes] PRIMARY KEY CLUSTERED 
(
	[DocumentTypeId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO


