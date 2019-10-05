@ECHO OFF

echo Building Win x64
dotnet publish "StarCitizen JSON to SQL.csproj" -c Release -r win-x64 --self-contained=true

echo Building Linux x64
wsl dotnet publish "StarCitizen JSON to SQL.csproj" -c Release -r linux-x64 --self-contained=true 
