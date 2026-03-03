CREATE TABLE dbo.Currencies(
	[Code] NVARCHAR(3) PRIMARY KEY,
	[Name] NVARCHAR(256) NOT NULL,
	[ExchangeRateToBase] DECIMAL(10, 6) NOT NULL,
	[IsActive] BIT NOT NULL,
	[Deprecated] BIT NOT NULL
		CONSTRAINT DF_Currencies_Deprecated DEFAULT (0),
	[CreatedAt] DATETIME2(0) NOT NULL
		CONSTRAINT DF_Currencies_CreatedAt DEFAULT (SYSUTCDATETIME()),
	[UpdatedAt] DATETIME2(0) NOT NULL
		CONSTRAINT DF_Currencies_UpdatedAt DEFAULT (SYSUTCDATETIME())
);

CREATE TABLE dbo.FeeConfigurations(
	[Id] UNIQUEIDENTIFIER PRIMARY KEY,
	[Name] NVARCHAR(256) NOT NULL,
	[Type] NVARCHAR(128) NOT NULL,
	[Percentage] DECIMAL(10, 6) NOT NULL,
	[EffectiveFrom] DATETIME2(0) NOT NULL,
	[EffectiveTo] DATETIME2(0) NULL,
	[IsActive] BIT NOT NULL,
	[CreatedAt] DATETIME2(0) NOT NULL
		CONSTRAINT DF_FeeConfigurations_CreatedAt DEFAULT (SYSUTCDATETIME()),
	[UpdatedAt] DATETIME2(0) NOT NULL
		CONSTRAINT DF_FeeConfigurations_UpdatedAt DEFAULT (SYSUTCDATETIME())
);

ALTER TABLE dbo.FeeConfigurations
ADD CONSTRAINT CK_FeeConfigurations_ValidDates
CHECK ([EffectiveTo] IS NULL OR [EffectiveTo] > [EffectiveFrom]);

CREATE TABLE dbo.RiskFactorConfigurations(
	[Id] UNIQUEIDENTIFIER PRIMARY KEY,
	[Level] NVARCHAR(64) NOT NULL,
	[ReferenceId] UNIQUEIDENTIFIER NULL,
	[BuildingType] NVARCHAR(50) NULL,
	[AdjustmentPercentage] DECIMAL(10,6) NOT NULL,
	[IsActive] BIT NOT NULL,
	[CreatedAt] DATETIME2(0) NOT NULL
		CONSTRAINT DF_RiskFactorConfigurations_CreatedAt DEFAULT (SYSUTCDATETIME()),
	[UpdatedAt] DATETIME2(0) NOT NULL
		CONSTRAINT DF_RiskFactorConfigurations_UpdatedAt DEFAULT (SYSUTCDATETIME())
);

ALTER TABLE dbo.RiskFactorConfigurations
ADD CONSTRAINT UQ_RiskFactorConfigurations_Level_ReferenceId_BuildingType
UNIQUE ([Level], [ReferenceId], [BuildingType]);

CREATE TABLE dbo.Administrators(
	[Id] UNIQUEIDENTIFIER PRIMARY KEY,
	[Name] NVARCHAR(256) NOT NULL,
	[Email] NVARCHAR(256) NOT NULL,
	[Role] NVARCHAR(64) NOT NULL,
	[CreatedAt] DATETIME2(0) NOT NULL
		CONSTRAINT DF_Administrators_CreatedAt DEFAULT (SYSUTCDATETIME()),
	[UpdatedAt] DATETIME2(0) NOT NULL
		CONSTRAINT DF_Administrators_UpdatedAt DEFAULT (SYSUTCDATETIME())
);

ALTER TABLE dbo.Administrators
ADD CONSTRAINT UQ_Administrators_Email
UNIQUE ([Email]);

CREATE TABLE dbo.Brokers(
	[Id] UNIQUEIDENTIFIER PRIMARY KEY,
	[Code] NVARCHAR(128) NOT NULL,
	[Name] NVARCHAR(256) NOT NULL,
	[Email] NVARCHAR(256) NOT NULL,
	[Phone] NVARCHAR(15) NOT NULL,
	[Status] BIT NOT NULL,
	[CommissionPercentage] DECIMAL(10,6) NULL
		CONSTRAINT DF_Brokers_CommissionPercentage DEFAULT (0.0),
	[CreatedAt] DATETIME2(0) NOT NULL
		CONSTRAINT DF_Brokers_CreatedAt DEFAULT (SYSUTCDATETIME()),
	[UpdatedAt] DATETIME2(0) NOT NULL
		CONSTRAINT DF_Brokers_UpdatedAt DEFAULT (SYSUTCDATETIME())
);

ALTER TABLE dbo.Brokers
ADD CONSTRAINT UQ_Brokers_Email
UNIQUE ([Email]);

ALTER TABLE dbo.Brokers
ADD CONSTRAINT UQ_Brokers_Code
UNIQUE ([Code]);

ALTER TABLE dbo.Brokers
ADD CONSTRAINT UQ_Brokers_Phone
UNIQUE ([Phone]);

CREATE TABLE dbo.Policies(
	[PolicyNumber] NVARCHAR(256) PRIMARY KEY,
	[ClientId] UNIQUEIDENTIFIER NOT NULL,
	[BuildingId] UNIQUEIDENTIFIER NOT NULL,
	[BrokerId] UNIQUEIDENTIFIER NOT NULL,
	[Status] NVARCHAR(64) NOT NULL,
	[StartDate] DATETIME2(0) NOT NULL,
	[EndDate] DATETIME2(0) NOT NULL,
	[BasePremium] DECIMAL(20, 4) NOT NULL,
	[CurrencyCode] NVARCHAR(3) NOT NULL,
	[FinalPremium] DECIMAL(20, 4) NOT NULL,
	[CreatedAt] DATETIME2(0) NOT NULL
		CONSTRAINT DF_Policies_CreatedAt DEFAULT (SYSUTCDATETIME()),
	[UpdatedAt] DATETIME2(0) NOT NULL
		CONSTRAINT DF_Policies_UpdatedAt DEFAULT (SYSUTCDATETIME())
);

ALTER TABLE dbo.Policies
ADD CONSTRAINT CK_Policies_ValidEndDate
CHECK ([EndDate] > [StartDate]);

ALTER TABLE dbo.Policies
ADD CONSTRAINT UQ_Policies_Client_Building_Broker
UNIQUE ([ClientId], [BuildingId], [BrokerId]);

ALTER TABLE dbo.Policies
ADD CONSTRAINT FK_Policies_Client
FOREIGN KEY ([ClientId]) REFERENCES dbo.Clients(Id);

ALTER TABLE dbo.Policies
ADD CONSTRAINT FK_Policies_Building
FOREIGN KEY ([BuildingId]) REFERENCES dbo.Buildings(Id);

ALTER TABLE dbo.Policies
ADD CONSTRAINT FK_Policies_Brokers
FOREIGN KEY ([BrokerId]) REFERENCES dbo.Brokers(Id);

ALTER TABLE dbo.Policies
ADD CONSTRAINT FK_Policies_Currencies
FOREIGN KEY ([CurrencyCode]) REFERENCES dbo.Currencies(Code);