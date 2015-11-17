BACKUP DATABASE [PathfinderFr] TO
  DISK = N'C:\Websites\Pathfinder-FR\WikiXmlExport\Sql\Export.bak' WITH
  COPY_ONLY,
  NOFORMAT,
  INIT,
  NAME = N'PathfinderFR',
  SKIP,
  NOREWIND,
  NOUNLOAD,
  STATS = 10
GO

RESTORE DATABASE [WikiExport] FROM
  DISK = N'C:\Websites\Pathfinder-FR\WikiXmlExport\Sql\Export.bak' WITH
  FILE = 1,
  MOVE N'PathfinderFR' TO N'C:\Websites\Pathfinder-FR\WikiXmlExport\Sql\WikiExport.mdf',
  MOVE N'PathfinderFR_log' TO N'C:\Websites\Pathfinder-FR\WikiXmlExport\Sql\WikiExport.ldf',
  NOUNLOAD,
  REPLACE,
  STATS = 10
GO
