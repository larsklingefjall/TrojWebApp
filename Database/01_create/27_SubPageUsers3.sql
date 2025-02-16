USE [d000540]
GO

/****** Object:  Table [dbo].[SubPageUsers3]    Script Date: 2025-02-16 18:18:55 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[SubPageUsers3](
	[SubPageUserId] [int] IDENTITY(1,1) NOT NULL,
	[SubPageId] [int] NOT NULL,
	[EmployeeId] [int] NOT NULL,
	[Changed] [datetime] NULL,
	[ChangedBy] [nvarchar](50) NULL,
 CONSTRAINT [PK_SubPageUsers3] PRIMARY KEY CLUSTERED 
(
	[SubPageUserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[SubPageUsers3]  WITH CHECK ADD  CONSTRAINT [FK_SubPageUsers3_Employees] FOREIGN KEY([EmployeeId])
REFERENCES [dbo].[Employees] ([EmployeeId])
GO

ALTER TABLE [dbo].[SubPageUsers3] CHECK CONSTRAINT [FK_SubPageUsers3_Employees]
GO

ALTER TABLE [dbo].[SubPageUsers3]  WITH CHECK ADD  CONSTRAINT [FK_SubPageUsers3_SubPages3] FOREIGN KEY([SubPageId])
REFERENCES [dbo].[SubPages3] ([SubPageId])
GO

ALTER TABLE [dbo].[SubPageUsers3] CHECK CONSTRAINT [FK_SubPageUsers3_SubPages3]
GO


