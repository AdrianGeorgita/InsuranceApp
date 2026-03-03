CREATE TABLE dbo.RiskIndicators(
	Id INT IDENTITY(1, 1) PRIMARY KEY,
	[Name] NVARCHAR(255) NOT NULL
);

ALTER TABLE dbo.RiskIndicators
ADD CONSTRAINT UQ_RiskIndicators_Name
UNIQUE ([Name]);

CREATE TABLE dbo.Buildings(
	Id UNIQUEIDENTIFIER PRIMARY KEY,
	[OwnerId] UNIQUEIDENTIFIER NOT NULL,
	[Address] NVARCHAR(255) NOT NULL,
	[CityId] UNIQUEIDENTIFIER NOT NULL,
	[ConstructionYear] INT NOT NULL,
	[BuildingType] NVARCHAR(50) NOT NULL,
	[NumberOfFloors] INT NOT NULL,
	[SurfaceArea] DECIMAL(6, 2) NOT NULL,
	[InsuredValue] DECIMAL NOT NULL
);

ALTER TABLE dbo.Buildings
ADD CONSTRAINT CK_Buildings_Type
CHECK ([BuildingType] IN ('Residential', 'Office', 'Industrial'));

ALTER TABLE dbo.Buildings
ADD CONSTRAINT CK_Buildings_PositiveNumberOfFloors
CHECK ([NumberOfFloors] > 0);

ALTER TABLE dbo.Buildings
ADD CONSTRAINT CK_Buildings_PositiveSurfaceArea
CHECK (SurfaceArea > 0);

ALTER TABLE dbo.Buildings
ADD CONSTRAINT CK_Buildings_PositiveInsuredValue
CHECK (InsuredValue > 0);

ALTER TABLE dbo.Buildings
ADD CONSTRAINT FK_Buildings_Owner
FOREIGN KEY ([OwnerId]) REFERENCES dbo.Clients(Id);

ALTER TABLE dbo.Buildings
ADD CONSTRAINT FK_Buildings_City
FOREIGN KEY ([CityId]) REFERENCES dbo.Cities(Id);

CREATE TABLE dbo.BuildingRiskIndicators(
	[BuildingId] UNIQUEIDENTIFIER NOT NULL,
	[RiskIndicatorId] INT NOT NULL
);

ALTER TABLE dbo.BuildingRiskIndicators
ADD CONSTRAINT PK_BuildingRiskIndicators
PRIMARY KEY ([BuildingId], [RiskIndicatorId]);

ALTER TABLE dbo.BuildingRiskIndicators
ADD CONSTRAINT FK_BuildingRiskIndicators_Buildings
FOREIGN KEY ([BuildingId]) REFERENCES dbo.Buildings(Id);

ALTER TABLE dbo.BuildingRiskIndicators
ADD CONSTRAINT FK_BuildingRiskIndicators_RiskIndicators
FOREIGN KEY ([RiskIndicatorId]) REFERENCES dbo.RiskIndicators(Id);