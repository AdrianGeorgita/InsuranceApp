CREATE TABLE dbo.Roles(
    [Id] UNIQUEIDENTIFIER PRIMARY KEY,
    [Name] NVARCHAR(256) NOT NULL,
    [NormalizedName] NVARCHAR(256) NOT NULL,
    [ConcurrencyStamp] NVARCHAR(36) NULL
);

CREATE UNIQUE INDEX IX_Roles_NormalizedName
ON dbo.Roles(NormalizedName);

CREATE TABLE dbo.Users(
    [Id] UNIQUEIDENTIFIER PRIMARY KEY,
    [UserName] NVARCHAR(256) NOT NULL,
    [NormalizedUserName] NVARCHAR(256) NOT NULL,
    [Email] NVARCHAR(256) NOT NULL,
    [NormalizedEmail] NVARCHAR(256) NOT NULL,
    [PasswordHash] NVARCHAR(MAX) NOT NULL,
    [SecurityStamp] NVARCHAR(36) NULL,
    [ConcurrencyStamp] NVARCHAR(36) NULL,
	[AccessFailedCount] INT NOT NULL CONSTRAINT DF_Users_AccessFailedCount DEFAULT 0,
    [EmailConfirmed] BIT NOT NULL CONSTRAINT DF_Users_EmailConfirmed DEFAULT 0,
    [LockoutEnabled] BIT NOT NULL CONSTRAINT DF_Users_LockoutEnabled DEFAULT 0,
    [LockoutEnd] DATETIMEOFFSET(7) NULL,
    [PhoneNumber] NVARCHAR(MAX) NULL,
    [PhoneNumberConfirmed] BIT NOT NULL CONSTRAINT DF_Users_PhoneNumberConfirmed DEFAULT 0,
    [TwoFactorEnabled] BIT NOT NULL CONSTRAINT DF_Users_TwoFactorEnabled DEFAULT 0,
	[CreatedAt] DATETIME2(0) NOT NULL
		CONSTRAINT DF_Users_CreatedAt DEFAULT (SYSUTCDATETIME()),
	[UpdatedAt] DATETIME2(0) NOT NULL
		CONSTRAINT DF_Users_UpdatedAt DEFAULT (SYSUTCDATETIME()),
	IsDeleted BIT NOT NULL 
        CONSTRAINT DF_Users_IsDeleted DEFAULT 0,
    DeletedAt DATETIME2(0) NULL
);

CREATE UNIQUE INDEX IX_Users_NormalizedEmail
ON dbo.Users(NormalizedEmail);

CREATE TABLE dbo.UserRoles(
    [UserId] UNIQUEIDENTIFIER NOT NULL,
	[RoleId] UNIQUEIDENTIFIER NOT NULL,
	CONSTRAINT PK_UserRoles PRIMARY KEY ([UserId], [RoleId])
);

ALTER TABLE dbo.UserRoles
ADD CONSTRAINT FK_UserRoles_User
FOREIGN KEY ([UserId]) REFERENCES dbo.Users(Id);

ALTER TABLE dbo.UserRoles
ADD CONSTRAINT FK_UserRoles_Role
FOREIGN KEY ([RoleId]) REFERENCES dbo.Roles(Id);

CREATE INDEX IX_UserRoles_RoleId ON dbo.UserRoles(RoleId);