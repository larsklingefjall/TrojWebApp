USE [d002883]
GO

/****** Object:  Table [dbo].[WorkingTimes]    Script Date: 2017-09-07 19:36:27 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[WorkingTimes](
	[WorkingTimeId] [int] IDENTITY(1,1) NOT NULL,
	[PersonId] [int] NOT NULL,
	[CaseId] [int] NOT NULL,
	[TariffTypeId] [int] NOT NULL,
	[EmployeeId] [int] NOT NULL,
	[WhenDate] [datetime] NULL,
	[TariffLevel] [float] NULL,
	[NumberOfHours] [float] NULL,
	[Cost] [float] NULL,
	[Sum] [float] NULL,
	[Comment] [nvarchar](2048) NULL,
	[CommentCry] [varbinary](2048) NULL,
	[Billed] [bit] NULL,
	[Changed] [datetime] NULL,
	[ChangedBy] [nvarchar](255) NULL,
 CONSTRAINT [PK_WorkingTimes] PRIMARY KEY CLUSTERED 
(
	[WorkingTimeId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

ALTER TABLE [dbo].[WorkingTimes]  WITH CHECK ADD  CONSTRAINT [FK_WorkingTimes_Cases] FOREIGN KEY([CaseId])
REFERENCES [dbo].[Cases] ([CaseId])
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[WorkingTimes] CHECK CONSTRAINT [FK_WorkingTimes_Cases]
GO

ALTER TABLE [dbo].[WorkingTimes]  WITH CHECK ADD  CONSTRAINT [FK_WorkingTimes_Employees] FOREIGN KEY([EmployeeId])
REFERENCES [dbo].[Employees] ([EmployeeId])
GO

ALTER TABLE [dbo].[WorkingTimes] CHECK CONSTRAINT [FK_WorkingTimes_Employees]
GO

ALTER TABLE [dbo].[WorkingTimes]  WITH CHECK ADD  CONSTRAINT [FK_WorkingTimes_Persons] FOREIGN KEY([PersonId])
REFERENCES [dbo].[Persons] ([PersonId])
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[WorkingTimes] CHECK CONSTRAINT [FK_WorkingTimes_Persons]
GO

ALTER TABLE [dbo].[WorkingTimes]  WITH CHECK ADD  CONSTRAINT [FK_WorkingTimes_TariffTypes] FOREIGN KEY([TariffTypeId])
REFERENCES [dbo].[TariffTypes] ([TariffTypeId])
GO

ALTER TABLE [dbo].[WorkingTimes] CHECK CONSTRAINT [FK_WorkingTimes_TariffTypes]
GO


