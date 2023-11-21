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
