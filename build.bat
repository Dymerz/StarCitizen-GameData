@ECHO OFF

ECHO Building StarCitizen XML to JSON
CD "StarCitizen XML to JSON"
CALL ./build.bat
CD ..

ECHO Building StarCitizen JSON to SQL
CD "StarCitizen JSON to SQL"
CALL ./build.bat
CD .. 
