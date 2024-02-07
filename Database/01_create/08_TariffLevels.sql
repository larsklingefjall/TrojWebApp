USE [d002883]
GO

/****** Object:  Table [dbo].[TariffLevels]    Script Date: 2017-09-07 19:32:38 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[TariffLevels](
	[TariffLevelId] [int] IDENTITY(1,1) NOT NULL,
	[TariffTypeId] [int] NOT NULL,
	[TariffLevel] [float] NOT NULL,
	[ValidFrom] [datetime] NOT NULL,
	[ValidTo] [datetime] NOT NULL,
	[Valid] [bit] NULL,
	[Changed] [datetime] NULL,
	[ChangedBy] [nvarchar](255) NULL,
 CONSTRAINT [PK_TariffLevels] PRIMARY KEY CLUSTERED 
(
	[TariffLevelId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[TariffLevels]  WITH CHECK ADD  CONSTRAINT [FK_TariffLevels_TariffTypes] FOREIGN KEY([TariffTypeId])
REFERENCES [dbo].[TariffTypes] ([TariffTypeId])
GO

ALTER TABLE [dbo].[TariffLevels] CHECK CONSTRAINT [FK_TariffLevels_TariffTypes]
GO


