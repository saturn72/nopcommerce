-- MySql
CREATE TABLE KmUserCustomerMap(
	Id int NOT NULL AUTO_INCREMENT,
	CreatedOnUtc TIMESTAMP NOT NULL DEFAULT (UTC_TIMESTAMP),
    KmUserId VARCHAR(256) NOT NULL,
    ProviderId varchar(128) NOT NULL,
    TenantId varchar(128) NULL,
    CustomerId int NOT NULL,
    ShouldProvisionBasicClaims boolean NOT NULL DEFAULT(false),
    PRIMARY KEY (Id)
);
