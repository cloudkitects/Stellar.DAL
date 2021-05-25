USE [master];

IF (SELECT DB_ID('{0}')) IS NOT NULL
BEGIN
    ALTER DATABASE [{0}] SET OFFLINE WITH ROLLBACK IMMEDIATE;
    ALTER DATABASE [{0}] SET ONLINE;
    DROP DATABASE [{0}];
END

DECLARE @names nvarchar(max) = '';
DECLARE @statement nvarchar(max) = '';

IF @@SERVICENAME LIKE 'LOCALDB%' BEGIN
	SELECT @names = @names + ',[' + [name] + ']' from [master].[dbo].[sysdatabases] WHERE [name] LIKE 'unit-tests%';

	IF LEN(@names) = 0 BEGIN
		PRINT 'no databases to drop';
	END
	ELSE BEGIN
		SET @statement = 'DROP DATABASE ' + SUBSTRING(@names, 2, LEN(@names)) + ';';
		PRINT @statement;
		EXEC sp_executesql @statement;
	END
END