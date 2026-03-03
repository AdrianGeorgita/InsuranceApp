CREATE TABLE dbo.PoliciesReplica(
	[PolicyNumber] NVARCHAR(256) PRIMARY KEY,
	[BuildingId] UNIQUEIDENTIFIER NOT NULL,
	[BrokerId] UNIQUEIDENTIFIER NOT NULL,
	[BrokerName] NVARCHAR(256) NOT NULL,
	[BuildingType] NVARCHAR(50) NOT NULL,
	[CountryName] NVARCHAR(255) NOT NULL,
	[CountyName] NVARCHAR(255) NOT NULL,
	[CityName] NVARCHAR(255) NOT NULL,
	[Status] NVARCHAR(64) NOT NULL,
	[StartDate] DATETIME2(0) NOT NULL,
	[EndDate] DATETIME2(0) NOT NULL,
	[CurrencyCode] NVARCHAR(3) NOT NULL,
	[FinalPremium] DECIMAL(20, 4) NOT NULL,
	[FinalPremiumInBaseCurrency] DECIMAL(20,4) NOT NULL
);

CREATE INDEX IX_PoliciesReplica_StartDate
ON dbo.PoliciesReplica (StartDate);

CREATE INDEX IX_PoliciesReplica_Status_StartDate
ON dbo.PoliciesReplica (Status, StartDate);

CREATE INDEX IX_PoliciesReplica_Currency_StartDate
ON dbo.PoliciesReplica (CurrencyCode, StartDate);

CREATE INDEX IX_PoliciesReplica_Country_Currency_StartDate
ON dbo.PoliciesReplica (CountryName, CurrencyCode, StartDate);

CREATE INDEX IX_PoliciesReplica_County_Currency_StartDate
ON dbo.PoliciesReplica (CountyName, CurrencyCode, StartDate);

CREATE INDEX IX_PoliciesReplica_City_Currency_StartDate
ON dbo.PoliciesReplica (CityName, CurrencyCode, StartDate);

CREATE INDEX IX_PoliciesReplica_Broker_Currency_StartDate
ON dbo.PoliciesReplica (BrokerId, CurrencyCode, StartDate);