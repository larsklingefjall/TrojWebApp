USE [d002883]
GO

/****** Object:  Table [dbo].[PersonAddresses]    Script Date: 2017-09-07 18:57:36 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[PersonAddresses](
	[PersonAddressId] [int] IDENTITY(1,1) NOT NULL,
	[PersonId] [int] NOT NULL,
	[CareOf] [nvarchar](255) NULL,
	[CareOfCry] [varbinary](512) NULL,
	[StreetName] [nvarchar](255) NULL,
	[StreetNameCry] [varbinary](256) NULL,
	[StreetNumber] [nvarchar](100) NULL,
	[PostalCode] [nvarchar](20) NULL,
	[PostalAddress] [nvarchar](255) NULL,
	[Country] [nvarchar](255) NULL,
	[Valid] [bit] NOT NULL,
	[Changed] [datetime] NULL,
	[ChangedBy] [nvarchar](255) NULL,
 CONSTRAINT [PK_PersonAddresses] PRIMARY KEY CLUSTERED 
(
	[PersonAddressId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

ALTER TABLE [dbo].[PersonAddresses]  WITH CHECK ADD  CONSTRAINT [FK_PersonAddresses_Persons] FOREIGN KEY([PersonId])
REFERENCES [dbo].[Persons] ([PersonId])
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[PersonAddresses] CHECK CONSTRAINT [FK_PersonAddresses_Persons]
GO


