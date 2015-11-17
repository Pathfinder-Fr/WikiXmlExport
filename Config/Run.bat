@echo off

rem Copie BDD
sqlcmd -i CopyDb.sql > Run.log

rem Copie dossier public
robocopy ..\sueetie\www\Wiki\public In /MIR /XD Attachments Upload Messages >> Run.log

rem Copie fichier Config.cs
copy Config.cs In\Config.cs /y >> Run.log

rem Application Snippets Customs
Bin\WikiXmlExport.exe ApplySnippets >> Run.log

rem Export
Bin\WikiXmlExport.exe Export /ptrn:++Pathfinder-RPG /out:Out /tmpl:template.txt /ext:xml >> Run.log

rem Creation ZIP
7z a ..\Db\Raw\WikiXml.7z Out\Pathfinder-RPG\*.* >> Run.log

rem Nettoyage
sqlcmd -i DropDb.sql >> Run.log

rem Suppression .bak
del /q Sql\Export.bak >> Run.log