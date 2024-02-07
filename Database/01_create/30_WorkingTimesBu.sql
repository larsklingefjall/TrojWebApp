USE [d002883]
GO

/****** Object:  Table [dbo].[WorkingTimesBu]    Script Date: 2017-11-07 16:34:26 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[WorkingTimesBu](
	[WorkingTimeBuId] [int] IDENTITY(1,1) NOT NULL,
	[WorkingTimeId] [int] NOT NULL,	
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
	[Reason] [nvarchar](50) NULL,
	[ChangedDone] [datetime] NULL,
	[ChangedByDone] [nvarchar](255) NULL,
 CONSTRAINT [PK_WorkingTimesBu] PRIMARY KEY CLUSTERED 
(
	[WorkingTimeBuId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO