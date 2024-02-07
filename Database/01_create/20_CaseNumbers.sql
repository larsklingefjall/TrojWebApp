USE [d002883]
GO

/****** Object:  Table [dbo].[CaseNumbers]    Script Date: 2017-09-07 19:54:18 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING OFF
GO

CREATE TABLE [dbo].[CaseNumbers](
	[CaseNumberId] [int] IDENTITY(1,1) NOT NULL,
	[CaseId] [int] NOT NULL,
	[CourtId] [int] NOT NULL,
	[CaseNumber] [nvarchar](255) NULL,
	[CaseNumberCry] [varbinary](256) NULL,
	[Changed] [datetime] NULL,
	[ChangedBy] [nvarchar](255) NULL,
 CONSTRAINT [PK_CaseNumbers] PRIMARY KEY CLUSTERED 
(
	[CaseNumberId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

ALTER TABLE [dbo].[CaseNumbers]  WITH CHECK ADD  CONSTRAINT [FK_CaseNumbers_Cases] FOREIGN KEY([CaseId])
REFERENCES [dbo].[Cases] ([CaseId])
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[CaseNumbers] CHECK CONSTRAINT [FK_CaseNumbers_Cases]
GO

ALTER TABLE [dbo].[CaseNumbers]  WITH CHECK ADD  CONSTRAINT [FK_CaseNumbers_Courts] FOREIGN KEY([CourtId])
REFERENCES [dbo].[Courts] ([CourtId])
GO

ALTER TABLE [dbo].[CaseNumbers] CHECK CONSTRAINT [FK_CaseNumbers_Courts]
GO


