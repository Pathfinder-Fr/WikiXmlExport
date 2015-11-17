@echo off
@set WikiPublic=..\sueetie\www\Wiki\public
@set 7zOut=..\Db\Raw\WikiXml.7z

@rem Copie BDD
sqlcmd -i "Config\CopyDb.sql" > Run.log

@rem Copie dossier public
robocopy "%WikiPublic%" ".\In" /MIR /XD Attachments Upload Messages >> Run.log

@rem Copie fichier Config.cs
copy "Config\Config.cs" "In\Config.cs" /y >> Run.log

@rem Application Snippets Customs
"Bin\WikiXmlExport.exe" ApplySnippets >> Run.log

@rem Export
"Bin\WikiXmlExport.exe" Export /ptrn:++Pathfinder-RPG /out:Out /tmpl:Config\template.xml >> Run.log

@rem Creation ZIP
"Tools\7z.exe" a "%7zOut%" "Out\Pathfinder-RPG\*.*" >> Run.log

@rem Nettoyage
sqlcmd -i "Config\DropDb.sql" >> Run.log

@rem Suppression .bak
del /q "Sql\Export.bak" >> Run.log