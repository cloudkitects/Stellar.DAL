USE [{0}];

IF  NOT EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'Customer') AND [type] IN (N'U'))
BEGIN
    CREATE TABLE Customer
    (
        CustomerId bigint NOT NULL IDENTITY(1,1),
        FirstName nvarchar(120) NOT NULL,
        LastName nvarchar(120) NOT NULL,
        DateOfBirth date NOT NULL,
        Id uniqueidentifier NOT NULL,
        CONSTRAINT PKCustomer PRIMARY KEY CLUSTERED(CustomerId ASC)
    );
END

IF  NOT EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'Address') AND [type] IN (N'U'))
BEGIN
	CREATE TABLE [Address]
	(
		AddressId bigint NOT NULL IDENTITY(1,1) CONSTRAINT PKAddressId PRIMARY KEY CLUSTERED(AddressId ASC),
		IsPoBox bit NOT NULL CONSTRAINT DFAddressIsPoBox DEFAULT(0),
		Line1 nvarchar(100) NOT NULL CONSTRAINT DFAddressLine1 DEFAULT(''),
		Line2 nvarchar(100) NULL,
		Line3 nvarchar(100) NULL,
		City nvarchar(100) NOT NULL,
		Country int NOT NULL,
		StateOrProvince char(2) NOT NULL,
		Zip nvarchar(10) NULL,
		Latitude decimal(9,6) NULL,
		Longitude decimal(9,6) NULL,
		Id char(36) NOT NULL
	)
END

/*
 * NOTES
 * The International Telecommunication Union (ITU) E.164 numbering plan imposes a maximum length of 15 digits to telephone numbers.
 * The length limit on email addresses is 254 characters (octets) accepted by IETF RFC3696 (following submitted erratum). A lower
 * limit is a conscious design choice.
 * The HMACSHA512 key can be any length but it is hashed using SHA-512 to a 128-byte (256 hex character) key.
 * SHA512 algorithms output a 64-byte (128 hex character) value.
 * Country codes are Internet Assigned Numbers Authority (IANA) two-character codes.
 * DIal codes are ITU codes.
*/

IF  NOT EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'Person') AND [type] IN (N'U'))
BEGIN
	CREATE TABLE Person
	(
		PersonId bigint NOT NULL IDENTITY(1,1) CONSTRAINT PKPerson PRIMARY KEY CLUSTERED(PersonId ASC),
		FirstName nvarchar(45) NOT NULL,
		LastName nvarchar(45) NOT NULL,
		Email nvarchar(100) NOT NULL CONSTRAINT UXPersonEmail UNIQUE NONCLUSTERED(Email ASC),
		Phone nvarchar(15) NULL CONSTRAINT UXPersonPhone UNIQUE NONCLUSTERED(Phone ASC),
		PasswordSalt varbinary(128) NULL,
		PasswordHash varbinary(64) NULL,
		AddressId bigint NULL CONSTRAINT FKPersonAddress FOREIGN KEY (AddressId) REFERENCES [Address](AddressId) ON DELETE CASCADE,
		Id char(36) NOT NULL
	)
END
