EXEC msdb.dbo.sp_delete_database_backuphistory @database_name = N'WikiExport'
GO

USE [master]
GO

DROP DATABASE [WikiExport]
GO
