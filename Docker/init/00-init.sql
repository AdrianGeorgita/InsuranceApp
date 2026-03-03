IF DB_ID(N'$(APP_DB_NAME)') IS NULL
BEGIN
    DECLARE @sql nvarchar(max) =
        N'CREATE DATABASE ' + QUOTENAME(N'$(APP_DB_NAME)') + N';';
    EXEC(@sql);
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.server_principals WHERE name = N'$(APP_DB_USER)')
BEGIN
    DECLARE @sql nvarchar(max) =
        N'CREATE LOGIN ' + QUOTENAME(N'$(APP_DB_USER)') +
        N' WITH PASSWORD = ' + QUOTENAME(N'$(APP_DB_PASSWORD)', '''') +
        N', CHECK_POLICY = OFF;';
    EXEC(@sql);
END
GO

DECLARE @sql nvarchar(max) = N'
USE ' + QUOTENAME(N'$(APP_DB_NAME)') + N';

IF NOT EXISTS (SELECT 1 FROM sys.database_principals WHERE name = N''$(APP_DB_USER)'')
BEGIN
    CREATE USER ' + QUOTENAME(N'$(APP_DB_USER)') + N' FOR LOGIN ' + QUOTENAME(N'$(APP_DB_USER)') + N';
END;

IF IS_ROLEMEMBER(N''db_owner'', N''$(APP_DB_USER)'') = 0
BEGIN
    ALTER ROLE db_owner ADD MEMBER ' + QUOTENAME(N'$(APP_DB_USER)') + N';
END;
';
EXEC(@sql);
GO

DECLARE @sql nvarchar(max) = N'
IF IS_SRVROLEMEMBER(N''dbcreator'', N''$(APP_DB_USER)'') = 0
BEGIN
    ALTER SERVER ROLE dbcreator ADD MEMBER ' + QUOTENAME(N'$(APP_DB_USER)') + N';
END;
';
EXEC(@sql);
GO