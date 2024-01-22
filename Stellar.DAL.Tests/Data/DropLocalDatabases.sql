USE [master];

DECLARE @names nvarchar(max) = '';
DECLARE @statement nvarchar(max) = '';

IF @@SERVICENAME LIKE 'LOCALDB%' BEGIN
	SELECT @names = @names + ',[' + [name] + ']' FROM [master].[dbo].[sysdatabases] WHERE [name] LIKE '%{0}%';

	IF LEN(@names) = 0 BEGIN
		PRINT 'no databases to drop.';
	END
	ELSE BEGIN
		SET @statement = 'DROP DATABASE ' + SUBSTRING(@names, 2, LEN(@names)) + ';';
		PRINT @statement;
		EXEC sp_executesql @statement;
	END
END