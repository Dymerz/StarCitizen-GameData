@ECHO OFF

echo Building Win x64
dotnet publish "StarCitizen JSON to SQL.csproj" -c Release -r win-x64 --self-contained=true

WHERE wsl
IF %ERRORLEVEL% EQU 0 (
	echo Building Linux x64
	wsl dotnet publish "StarCitizen JSON to SQL.csproj" -c Release -r linux-x64 --self-contained=true
)else(
	ECHO Skipping Linux build, missing WSL
)
