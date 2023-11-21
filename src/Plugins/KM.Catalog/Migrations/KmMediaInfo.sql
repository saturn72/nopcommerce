-- MySql
CREATE TABLE KmMediaItemInfo(
	Id int NOT NULL AUTO_INCREMENT,
	CreatedOnUtc TIMESTAMP NOT NULL DEFAULT (UTC_TIMESTAMP),
	EntityId int NOT NULL,
	EntityType tinytext NOT NULL,
	Storage tinytext NULL,
	StorageIdentifier tinytext NULL,
	Uri mediumtext NULL,
	BinaryData longblob NULL,
	Type tinytext NULL,
    PRIMARY KEY (Id)
);

-- Sql Server

USE [nopCommerce]
GO


CREATE TABLE [dbo].[KmMediaItemInfo](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[CreatedOnUtc] [datetime2](7) NULL,
	[EntityId] [int] NOT NULL,
	[EntityType] [nvarchar](256) NOT NULL,
	[Storage] [nvarchar](256) NULL,
	[StorageIdentifier] [nvarchar](256) NULL,
	[Uri] [nvarchar](MAX) NULL,
	[BinaryData] [varbinary](max) NULL,
	[Type] [nvarchar](256) NULL,
CONSTRAINT [PK_KmMediaItemInfo] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

ALTER TABLE [dbo].[KmMediaItemInfo] ADD  CONSTRAINT [DF_KmMediaItemInfo_CreatedOnUtc]  DEFAULT (getutcdate()) FOR [CreatedOnUtc]
GO


