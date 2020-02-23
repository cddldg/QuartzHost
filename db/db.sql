
CREATE TABLE [dbo].[JobTasks]
(
	[Id] [UNIQUEIDENTIFIER] NOT NULL PRIMARY KEY,
	[NodeName] [NVARCHAR](500) NOT NULL,
	[Title] [NVARCHAR](50) NOT NULL,
	[Remark] [NVARCHAR](500) NULL,
	[CronExpression] [NVARCHAR](50) NULL,
	[AssemblyName] [NVARCHAR](200) NOT NULL,
	[ClassName] [NVARCHAR](200) NOT NULL,
	[CustomParamsJson] [NVARCHAR](2000) NULL,
	[Status] [INT] NOT NULL,
	[CreateTime] [DATETIME2](7) NOT NULL,
	[CreateUserId] [INT] NOT NULL,
	[CreateUserName] [NVARCHAR](300) NULL,
	[LastRunTime] [DATETIME2](7) NULL,
	[NextRunTime] [DATETIME2](7) NULL,
	[TotalRunCount] [INT] NOT NULL
	)
GO
CREATE TABLE [dbo].[JobNodes](
	[NodeName] [NVARCHAR](400) NOT NULL PRIMARY KEY,
	[NodeType] [NVARCHAR](500) NOT NULL,
	[MachineName] [NVARCHAR](500) NULL,
	[AccessProtocol] [NVARCHAR](500) NOT NULL,
	[Host] [NVARCHAR](500) NOT NULL,
	[AccessSecret] [NVARCHAR](500) NULL,
	[LastUpdateTime] [DATETIME2](7) NULL,
	[Status] [INT] NOT NULL,
	[Priority] [INT] NOT NULL
	)
GO
CREATE TABLE [dbo].[JobTrace](
	[TraceId] [UNIQUEIDENTIFIER] NOT NULL PRIMARY KEY,
	[TaskId] [UNIQUEIDENTIFIER] NOT NULL,
	[Node] [NVARCHAR](400) NULL,
	[StartTime] [DATETIME2](7) NOT NULL,
	[EndTime] [DATETIME2](7)  NULL,
	[ElapsedTime] [FLOAT] NOT NULL,
	[Result] [INT] NOT NULL) 
GO

