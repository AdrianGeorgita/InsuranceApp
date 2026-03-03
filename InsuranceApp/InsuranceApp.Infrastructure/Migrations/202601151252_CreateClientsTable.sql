CREATE TABLE dbo.Clients(
	Id UNIQUEIDENTIFIER PRIMARY KEY,
	[Type] NVARCHAR(20) NOT NULL,
	[Name] NVARCHAR(255) NOT NULL,
	[IdentificationNumber] NVARCHAR(20) NOT NULL,
	[Email] NVARCHAR(255) NOT NULL,
	[Phone] NVARCHAR(15) NOT NULL,
	[Address] NVARCHAR(255) NOT NULL
);

ALTER TABLE dbo.Clients
ADD CONSTRAINT UQ_Clients_IdentificationNumber 
UNIQUE ([IdentificationNumber]);

ALTER TABLE dbo.Clients
ADD CONSTRAINT CK_Clients_Type
CHECK ([Type] IN ('Individual', 'Company'));