USE [d000540]
GO

/****** Object:  Table [dbo].[MailAddresses]    Script Date: 2025-03-17 11:04:58 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[MailAddresses](
	[MailAddressId] [int] IDENTITY(1,1) NOT NULL,
	[PersonId] [int] NOT NULL,
	[MailAddress] [nvarchar](1024) NULL,
	[MailAddressCry] [varbinary](1024) NULL,
	[Comment] [nvarchar](1024) NULL,
	[CommentCry] [varbinary](1024) NULL,
	[Changed] [datetime] NULL,
	[ChangedBy] [nvarchar](50) NULL,
 CONSTRAINT [PK_MailAddresses] PRIMARY KEY CLUSTERED 
(
	[MailAddressId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[MailAddresses]  WITH CHECK ADD  CONSTRAINT [FK_MailAddresses_Persons] FOREIGN KEY([PersonId])
REFERENCES [dbo].[Persons] ([PersonId])
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[MailAddresses] CHECK CONSTRAINT [FK_MailAddresses_Persons]
GO


