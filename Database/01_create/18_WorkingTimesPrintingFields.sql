USE [d002883]
GO

/****** Object:  Table [dbo].[WorkingTimesPrintingFields]    Script Date: 2017-09-07 19:50:49 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[WorkingTimesPrintingFields](
	[WorkingTimesPrintingFieldId] [int] IDENTITY(1,1) NOT NULL,
	[InvoiceUnderlayId] [int] NOT NULL,
	[FieldName] [nvarchar](255) NOT NULL,
	[Changed] [datetime] NULL,
	[ChangedBy] [nvarchar](255) NULL,
 CONSTRAINT [PK_WorkingTimesPrintingFields] PRIMARY KEY CLUSTERED 
(
	[WorkingTimesPrintingFieldId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[WorkingTimesPrintingFields]  WITH CHECK ADD  CONSTRAINT [FK_WorkingTimesPrintingFields_InvoiceUnderlays] FOREIGN KEY([InvoiceUnderlayId])
REFERENCES [dbo].[InvoiceUnderlays] ([InvoiceUnderlayId])
GO

ALTER TABLE [dbo].[WorkingTimesPrintingFields] CHECK CONSTRAINT [FK_WorkingTimesPrintingFields_InvoiceUnderlays]
GO


