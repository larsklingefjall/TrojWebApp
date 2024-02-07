USE [d002883]
GO

/****** Object:  Table [dbo].[TariffTypePrintingFields]    Script Date: 2017-10-29 10:52:23 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[TariffTypePrintingFields](
	[TariffTypePrintingFieldId] [int] IDENTITY(1,1) NOT NULL,
	[InvoiceUnderlayId] [int] NOT NULL,
	[TariffTypeId] [int] NOT NULL,
	[Invisible] [bit] NULL,
	[Changed] [datetime] NULL,
	[ChangedBy] [nvarchar](255) NULL,
 CONSTRAINT [PK_TariffTypePrintingFields] PRIMARY KEY CLUSTERED 
(
	[TariffTypePrintingFieldId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[TariffTypePrintingFields]  WITH CHECK ADD  CONSTRAINT [FK_TariffTypePrintingFields_InvoiceUnderlays] FOREIGN KEY([InvoiceUnderlayId])
REFERENCES [dbo].[InvoiceUnderlays] ([InvoiceUnderlayId])
GO

ALTER TABLE [dbo].[TariffTypePrintingFields] CHECK CONSTRAINT [FK_TariffTypePrintingFields_InvoiceUnderlays]
GO

ALTER TABLE [dbo].[TariffTypePrintingFields]  WITH CHECK ADD  CONSTRAINT [FK_TariffTypePrintingFields_TariffTypes] FOREIGN KEY([TariffTypeId])
REFERENCES [dbo].[TariffTypes] ([TariffTypeId])
GO

ALTER TABLE [dbo].[TariffTypePrintingFields] CHECK CONSTRAINT [FK_TariffTypePrintingFields_TariffTypes]
GO