ALTER TABLE dbo.BuildingRiskIndicators
DROP CONSTRAINT DF_BuildingRiskIndicators_CreatedAt;

ALTER TABLE dbo.BuildingRiskIndicators
DROP CONSTRAINT DF_BuildingRiskIndicators_UpdatedAt;

ALTER TABLE dbo.BuildingRiskIndicators
DROP COLUMN CreatedAt;

ALTER TABLE dbo.BuildingRiskIndicators
DROP COLUMN UpdatedAt;