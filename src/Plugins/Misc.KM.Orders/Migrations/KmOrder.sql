CREATE TABLE KmOrder(
	Id int NOT NULL AUTO_INCREMENT,
	CreatedOnUtc TIMESTAMP NOT NULL DEFAULT (UTC_TIMESTAMP),
    Data tinytext NULL,
    Status varchar(128) NOT NULL,
    KmOrderId varchar(256) NOT NULL,
    KmUserId varchar(256) NOT NULL,
    NopOrderId int NOT NULL,
    Errors mediumtext NULL,
    PRIMARY KEY (Id),
    FOREIGN KEY (NopOrderId) REFERENCES Order(Id)
);