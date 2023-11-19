CREATE TABLE KmUserCustomerMap(
	`Id` int NOT NULL AUTO_INCREMENT,
    `CreatedOnUtc` DATETIME(6) NOT NULL DEFAULT (UTC_TIMESTAMP),
    `KmUserId` VARCHAR(256) NOT NULL,
    `ProviderId` varchar(128) NOT NULL,
    `TenantId` varchar(128) NOT NULL,
    `CustomerId` int NOT NULL,
    `ShouldProvisionBasicClaims` boolean NOT NULL DEFAULT(false),
    INDEX `CustomerId_idx` (`CustomerId` ASC) VISIBLE,
    PRIMARY KEY (`Id`),
    CONSTRAINT `CustomerId_fk`
        FOREIGN KEY (`CustomerId`)
        REFERENCES `Customer` (`Id`)
        ON DELETE CASCADE
);
