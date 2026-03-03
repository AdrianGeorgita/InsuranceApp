CREATE TABLE dbo.AuditTableValueChanges(
	[Id] UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWSEQUENTIALID(),
	[EventId] UNIQUEIDENTIFIER NOT NULL,
	[UserId] UNIQUEIDENTIFIER NOT NULL,
	[TableName] NVARCHAR(128) NOT NULL,
	[ColumnName] NVARCHAR(128) NOT NULL,
	[RowId] NVARCHAR(256) NOT NULL,
	[OldValue] NVARCHAR(MAX) NULL,
	[NewValue] NVARCHAR(MAX) NULL,
	[UpdatedAt] DATETIME2(0) NOT NULL
);

ALTER TABLE dbo.AuditTableValueChanges
ADD CONSTRAINT DF_AuditTableValueChanges_UpdatedAt DEFAULT SYSUTCDATETIME() FOR [UpdatedAt];

CREATE INDEX IX_AuditTableValueChanges_TableName
ON dbo.AuditTableValueChanges (TableName, UpdatedAt DESC);

CREATE INDEX IX_AuditTableValueChanges_User
ON dbo.AuditTableValueChanges (UserId, UpdatedAt DESC);

CREATE INDEX IX_AuditTableValueChanges_EventId
ON dbo.AuditTableValueChanges (EventId, UpdatedAt DESC);