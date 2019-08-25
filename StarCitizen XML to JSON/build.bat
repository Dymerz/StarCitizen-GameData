@ECHO OFF

echo Building Win x64
dotnet publish -c Release --self-contained -r win-x64

echo Building WinUbuntu 16.10 x64
dotnet publish -c Release --self-contained -r ubuntu.16.10-x64 
