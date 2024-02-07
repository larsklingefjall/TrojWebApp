USE [d002883]
GO

/****** Object:  Table [dbo].[PersonCases]    Script Date: 2017-09-07 19:26:38 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[PersonCases](
	[PersonCaseId] [int] IDENTITY(1,1) NOT NULL,
	[PersonId] [int] NOT NULL,
	[CaseId] [int] NOT NULL,
	[PersonTypeId] [int] NOT NULL,
	[Responsible] [bit] NULL,
	[Changed] [datetime] NULL,
	[ChangedBy] [nvarchar](255) NULL,
 CONSTRAINT [PK_PersonCases] PRIMARY KEY CLUSTERED 
(
	[PersonCaseId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[PersonCases]  WITH CHECK ADD  CONSTRAINT [FK_PersonCases_Cases] FOREIGN KEY([CaseId])
REFERENCES [dbo].[Cases] ([CaseId])
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[PersonCases] CHECK CONSTRAINT [FK_PersonCases_Cases]
GO

ALTER TABLE [dbo].[PersonCases]  WITH CHECK ADD  CONSTRAINT [FK_PersonCases_Persons] FOREIGN KEY([PersonId])
REFERENCES [dbo].[Persons] ([PersonId])
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[PersonCases] CHECK CONSTRAINT [FK_PersonCases_Persons]
GO

ALTER TABLE [dbo].[PersonCases]  WITH CHECK ADD  CONSTRAINT [FK_PersonCases_PersonTypes] FOREIGN KEY([PersonTypeId])
REFERENCES [dbo].[PersonTypes] ([PersonTypeId])
GO

ALTER TABLE [dbo].[PersonCases] CHECK CONSTRAINT [FK_PersonCases_PersonTypes]
GO


