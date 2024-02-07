USE [d002883]
GO

/****** Object:  Table [dbo].[PhoneNumbers]    Script Date: 2017-09-07 19:39:33 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING OFF
GO

CREATE TABLE [dbo].[PhoneNumbers](
	[PhoneNumberId] [int] IDENTITY(1,1) NOT NULL,
	[PersonId] [int] NOT NULL,
	[PhoneNumberTypeId] [int] NOT NULL,
	[PhoneNumber] [nvarchar](255) NULL,
	[PhoneNumberCry] [varbinary](256) NULL,
	[Changed] [datetime] NULL,
	[ChangedBy] [nvarchar](255) NULL,
 CONSTRAINT [PK_PhoneNumbers] PRIMARY KEY CLUSTERED 
(
	[PhoneNumberId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

ALTER TABLE [dbo].[PhoneNumbers]  WITH CHECK ADD  CONSTRAINT [FK_PhoneNumbers_Persons] FOREIGN KEY([PersonId])
REFERENCES [dbo].[Persons] ([PersonId])
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[PhoneNumbers] CHECK CONSTRAINT [FK_PhoneNumbers_Persons]
GO

ALTER TABLE [dbo].[PhoneNumbers]  WITH CHECK ADD  CONSTRAINT [FK_PhoneNumbers_PhoneNumberTypes] FOREIGN KEY([PhoneNumberTypeId])
REFERENCES [dbo].[PhoneNumberTypes] ([PhoneNumberTypeId])
GO

ALTER TABLE [dbo].[PhoneNumbers] CHECK CONSTRAINT [FK_PhoneNumbers_PhoneNumberTypes]
GO


