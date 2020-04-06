@ECHO OFF

ECHO Building StarCitizen XML to JSON
CD "StarCitizen_XML_To_JSON"
CALL ./build.bat
CD ..

ECHO Building StarCitizen JSON to SQL
CD "StarCitizen_JSON_To_SQL"
CALL ./build.bat
CD .. 
