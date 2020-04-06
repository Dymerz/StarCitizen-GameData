# StarCitizen-GameData

This tool allow users to convert raw XML from the [unp4k](https://github.com/dolkensp/unp4k) tool in JSON format or SQL format.

You can also directly download binaries [here](https://github.com/Dymerz/StarCitizen-GameData/releases).

This tool is user by the StarCitizen-API to allow user to easily retrieve Star Citizen data, if you want to contribute, follow the [contributing](contributing.md) guideline.

![Discord](https://img.shields.io/badge/discord-join-%237289DA?link=https://discord.gg/EcWagya)

## Supported versions

- 3.6.1 → 3.8.1
- 3.8.2 [not fully tested]

## Usage

**StarCitizen_XML_to_JSON.exe source [destination] [config...] [filter...]**
  - source: The path of the **Data** provided by **unp4k.**
  - destination: Where to store files (default: Working directory).
  - configs:
      - --debug: Enable Debug logs.
      - --cache: Use cache (speed up the process but may not be compatible between SC versions).
      - --rebuild: Remove the cache and rebuild a new one.
      - --english: Use the English localization (default).
      - --french: Use the French localization (not implemented).
      - --german: Use the German localization (not implemented).
      - --version: Print the current version of the application.
      - --help: Print help.
  - filters:
      - --all, -a
      - --ships, -s
      - --weapons, -w
      - --weapons-magazine, -wm
      - --commodities, -c
      - --tags, -t
      - --shops, -S
      - --manufacturers, -m
      - --starmap, -sm (not implemented).

**StarCitizen_JSON_to_SQL.exe source destination database_name sc_version [config...] [filter...]**
  - source
  - destination
  - database_name
  - sc-version
  - configs:
      - --debug: Enable Debug logs.
      - --minify: Generate minified JSON
      - --help: Print help.
  - filters:
      - --all, -a
      - --ships, -s
      - --weapons, -w
      - --weapons-magazine, -wm
      - --commodities, -c
      - --tags, -t
      - --shops, -S
      - --manufacturers, -m
      - --starmap, -sm (not implemented).

## Manually build

### Requirements

- nuget (Powershell →`Install-Package NuGet.CommandLine`).
- .Net Core 3.1+.

### Setting up

- Clone this repo.
- Install all dependencies using `nuget install`.

### Building

- Execute the `build.bat` to generate JSON and SQL Converter.
- Get the output in `{sub project}\bin\Release\{netcoreapp2.2}\{platform}`.

## Contributes

To contribute to this project, see [contributing](contributing.md) file.
