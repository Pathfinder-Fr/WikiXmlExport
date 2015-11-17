WikiXmlExport
=============

Cet outil permet, à partir des données du wiki actuel, d'exporter les pages dans des fichiers XML.

Il nécessite d'avoir accès aux données WIKI du site, c'est à dire à la base de données SQL et aux fichiers de configuration du wiki.

Etapes d'initialisation de l'environnement

1) Recopie des fichiers public du wiki dans un dossier In
robocopy "%WikiPublic%" ".\In" /MIR /XD Attachments Upload Messages

2) Recopie du fichier de config
copy "Config\Config.cs" "In\Config.cs" /y

3) Application Snippets Customs
"Bin\WikiXmlExport.exe" ApplySnippets

4) Export
"Bin\WikiXmlExport.exe" Export /ptrn:++Pathfinder-RPG /out:.\Out /tmpl:Config\template.xml