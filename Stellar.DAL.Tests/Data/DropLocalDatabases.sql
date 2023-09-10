USE [master];

DECLARE @names nvarchar(max) = '';
DECLARE @statement nvarchar(max) = '';

IF @@SERVICENAME LIKE 'LOCALDB%' BEGIN
	SELECT @names = @names + ',[' + [name] + ']' from [master].[dbo].[sysdatabases] WHERE [name] LIKE '%{0}%';

	IF LEN(@names) = 0 BEGIN
		PRINT 'no more %-unit-tests-% databases to drop.';
	END
	ELSE BEGIN
		SET @statement = 'DROP DATABASE ' + SUBSTRING(@names, 2, LEN(@names)) + ';';
		PRINT @statement;
		EXEC sp_executesql @statement;
	END
END