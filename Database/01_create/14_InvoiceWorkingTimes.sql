SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[InvoiceWorkingTimes](
	[InvoiceWorkingTimeId] [int] IDENTITY(1,1) NOT NULL,
	[InvoiceUnderlayId] [int] NOT NULL,
	[WorkingTimeId] [int] NOT NULL,
	[UnitCost] [float] NULL,
	[NumberOfHours] [float] NULL,
	[Sum] [float] NULL,
	[Changed] [datetime] NULL,
	[ChangedBy] [nvarchar](255) NULL,
 CONSTRAINT [PK_InvoiceWorkingTimes] PRIMARY KEY CLUSTERED 
(
	[InvoiceWorkingTimeId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[InvoiceWorkingTimes]  WITH CHECK ADD  CONSTRAINT [FK_InvoiceWorkingTimes_InvoiceUnderlays] FOREIGN KEY([InvoiceUnderlayId])
REFERENCES [dbo].[InvoiceUnderlays] ([InvoiceUnderlayId])
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[InvoiceWorkingTimes] CHECK CONSTRAINT [FK_InvoiceWorkingTimes_InvoiceUnderlays]
GO

ALTER TABLE [dbo].[InvoiceWorkingTimes]  WITH CHECK ADD  CONSTRAINT [FK_InvoiceWorkingTimes_WorkingTimes] FOREIGN KEY([WorkingTimeId])
REFERENCES [dbo].[WorkingTimes] ([WorkingTimeId])
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[InvoiceWorkingTimes] CHECK CONSTRAINT [FK_InvoiceWorkingTimes_WorkingTimes]
GO


