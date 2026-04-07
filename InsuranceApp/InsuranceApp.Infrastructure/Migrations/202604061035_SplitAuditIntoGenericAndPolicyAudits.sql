CREATE TABLE dbo.PolicyAuditLogs(
    [Id] UNIQUEIDENTIFIER PRIMARY KEY,
    [BrokerId] UNIQUEIDENTIFIER NOT NULL,
    [ColumnName] NVARCHAR(128) NOT NULL,
    [PolicyNumber] NVARCHAR(256) NOT NULL,
    [OldValue] NVARCHAR(MAX) NULL,
    [NewValue] NVARCHAR(MAX) NULL,
    [OccurredAt] DATETIME2(0) NOT NULL
);

ALTER TABLE dbo.PolicyAuditLogs
ADD CONSTRAINT DF_PolicyAuditLogs_OccurredAt
DEFAULT SYSUTCDATETIME() FOR [OccurredAt];

CREATE INDEX IX_PolicyAuditLogs_Broker
ON dbo.PolicyAuditLogs (BrokerId, OccurredAt DESC);

CREATE INDEX IX_PolicyAuditLogs_PolicyNumber
ON dbo.PolicyAuditLogs (PolicyNumber, OccurredAt DESC);


INSERT INTO dbo.PolicyAuditLogs (
    Id,
    BrokerId,
    ColumnName,
    PolicyNumber,
    OldValue,
    NewValue,
    OccurredAt
)
SELECT
    Id,
    UserId,
    ColumnName,
    RowId,
    OldValue,
    NewValue,
    UpdatedAt
FROM dbo.AuditTableValueChanges;


CREATE TABLE dbo.AuditLogs(
    [Id] UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWSEQUENTIALID(),
    [EventId] UNIQUEIDENTIFIER NOT NULL,
    [UserId] UNIQUEIDENTIFIER NOT NULL,
    [Action] NVARCHAR(128) NOT NULL,
    [TableName] NVARCHAR(128) NOT NULL,
    [ColumnName] NVARCHAR(128) NOT NULL,
    [RowId] NVARCHAR(256) NOT NULL,
    [OldValue] NVARCHAR(MAX) NULL,
    [NewValue] NVARCHAR(MAX) NULL,
    [OccurredAt] DATETIME2(0) NOT NULL
);

ALTER TABLE dbo.AuditLogs
ADD CONSTRAINT DF_AuditLogs_OccurredAt
DEFAULT SYSUTCDATETIME() FOR [OccurredAt];

CREATE INDEX IX_AuditLogs_TableName
ON dbo.AuditLogs (TableName, OccurredAt DESC);

CREATE INDEX IX_AuditLogs_User
ON dbo.AuditLogs (UserId, OccurredAt DESC);

CREATE INDEX IX_AuditLogs_EventId
ON dbo.AuditLogs (EventId, OccurredAt DESC);


DROP TABLE dbo.AuditTableValueChanges;