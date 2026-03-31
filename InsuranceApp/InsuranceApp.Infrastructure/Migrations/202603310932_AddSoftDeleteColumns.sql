ALTER TABLE dbo.Countries
ADD 
	IsActive BIT NOT NULL 
	CONSTRAINT DF_Countries_IsActive DEFAULT 1;
	
ALTER TABLE dbo.Counties
ADD 
	IsActive BIT NOT NULL 
	CONSTRAINT DF_Counties_IsActive DEFAULT 1;
	
ALTER TABLE dbo.Cities
ADD 
	IsActive BIT NOT NULL 
	CONSTRAINT DF_Cities_IsActive DEFAULT 1;
	
ALTER TABLE dbo.RiskIndicators
ADD 
	IsActive BIT NOT NULL 
	CONSTRAINT DF_RiskIndicators_IsActive DEFAULT 1;
	
ALTER TABLE dbo.Buildings
ADD 
    IsDeleted BIT NOT NULL 
        CONSTRAINT DF_Buildings_IsDeleted DEFAULT 0,
    DeletedAt DATETIME2 NULL;
GO
	
CREATE INDEX IX_Buildings_Active 
ON dbo.Buildings(Id)
WHERE IsDeleted = 0;

ALTER TABLE dbo.Clients
ADD 
    IsDeleted BIT NOT NULL 
        CONSTRAINT DF_Clients_IsDeleted DEFAULT 0,
    DeletedAt DATETIME2 NULL;
GO
	
CREATE INDEX IX_Clients_Active 
ON dbo.Clients(Id)
WHERE IsDeleted = 0;

ALTER TABLE dbo.Currencies
ADD CONSTRAINT DF_Currencies_IsActive
DEFAULT 1 FOR IsActive;

ALTER TABLE dbo.FeeConfigurations
ADD CONSTRAINT DF_FeeConfigurations_IsActive
DEFAULT 1 FOR IsActive;

ALTER TABLE dbo.RiskFactorConfigurations
ADD CONSTRAINT DF_RiskFactorConfigurations_IsActive
DEFAULT 1 FOR IsActive;

ALTER TABLE dbo.Administrators
ADD 
    IsDeleted BIT NOT NULL 
        CONSTRAINT DF_Administrators_IsDeleted DEFAULT 0,
    DeletedAt DATETIME2 NULL;
GO
	
CREATE INDEX IX_Administrators_Active 
ON dbo.Administrators(Id)
WHERE IsDeleted = 0;

ALTER TABLE dbo.Brokers
ADD 
    IsDeleted BIT NOT NULL 
        CONSTRAINT DF_Brokers_IsDeleted DEFAULT 0,
    DeletedAt DATETIME2 NULL;
GO
	
CREATE INDEX IX_Brokers_Active 
ON dbo.Brokers(Id)
WHERE IsDeleted = 0;

ALTER TABLE dbo.Policies
ADD 
    IsDeleted BIT NOT NULL 
        CONSTRAINT DF_Policies_IsDeleted DEFAULT 0,
    DeletedAt DATETIME2 NULL;
GO
	
CREATE INDEX IX_Policies_Active 
ON dbo.Policies(PolicyNumber)
WHERE IsDeleted = 0;

ALTER TABLE dbo.PoliciesReplica
ADD 
    IsDeleted BIT NOT NULL 
        CONSTRAINT DF_PoliciesReplica_IsDeleted DEFAULT 0
GO
	
CREATE INDEX IX_PoliciesReplica_Active 
ON dbo.Policies(PolicyNumber)
WHERE IsDeleted = 0;