SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Configurations](
	[ConfigurationId] [int] IDENTITY(1,1) NOT NULL,
	[ConfigKey] [nvarchar](255) NULL,
	[ConfigValue] [nvarchar](MAX) NULL,
	[ChangedBy] [nvarchar](50) NULL,
	[Changed] [datetime] NULL,
 CONSTRAINT [PK_Configurations] PRIMARY KEY CLUSTERED 
(
	[ConfigurationId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
