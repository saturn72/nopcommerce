-- MySql
CREATE TABLE KmCatalogMetadata(
	Id int NOT NULL AUTO_INCREMENT,
	CreatedOnUtc TIMESTAMP NOT NULL DEFAULT (UTC_TIMESTAMP),
	StoresVersion tinytext NOT NULL,
    PRIMARY KEY (Id)
);

-- Sql Server

USE [nopCommerce]
GO

CREATE TABLE [dbo].[KmCatalogMetadata](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[CreatedOnUtc] [datetime2](7) NULL,
	[StoresVersion] [nvarchar](64) NOT NULL,
CONSTRAINT [PK_KmCatalogMetadata] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[KmCatalogMetadata] ADD  CONSTRAINT [DF_KmCatalogMetadata_CreatedOnUtc]  DEFAULT (getutcdate()) FOR [CreatedOnUtc]
GO