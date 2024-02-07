USE [d002883]
GO

/****** Object:  Table [dbo].[CaseTypes]    Script Date: 2017-09-07 19:04:54 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[CaseTypes](
	[CaseTypeId] [int] IDENTITY(1,1) NOT NULL,
	[CaseType] [nvarchar](255) NULL,
	[Changed] [datetime] NULL,
	[ChangedBy] [nvarchar](255) NULL,
 CONSTRAINT [PK_CaseTypes] PRIMARY KEY CLUSTERED 
(
	[CaseTypeId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO