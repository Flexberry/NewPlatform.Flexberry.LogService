# NewPlatform.Flexberry.LogService Changelog

All notable changes to this project will be documented in this file.
This project adheres to [Semantic Versioning](http://semver.org/).

## [Unreleased]

### Added

### Fixed

### Changed

## [2.2.1] - 2023-12-22

### Changed
* improved reading connection string from `App.config` [#19](https://github.com/Flexberry/NewPlatform.Flexberry.LogService/pull/19):
  - `appSettings.DefaultConnectionStringName` is used when `ConnectionString`/`ConnectionStringName` are not specified in `CustomAdoNetAppender`
  - `CustomAdoNetAppender` falls back to `appSettings.CustomizationStrings` property when no connection string is specified elsewhere

### Fixed
* reading connection string from `appsettings.json` is fixed [#19](https://github.com/Flexberry/NewPlatform.Flexberry.LogService/pull/19)

## [2.2.0] - 2023-12-21

### Changed
* connection string may be read from `appsettings.json` in `.NET Standard 2.0` projects [#17](https://github.com/Flexberry/NewPlatform.Flexberry.LogService/pull/17)

## [2.1.0] - 2022-08-20

### Changed
* upgrade `log4net` to version 2.0.15.

## [2.0.0] - 2021-01-25

### Added
- `.NET Standard 2.0` implementation.

### Fixed

### Changed
* csproj format to `Microsoft.NET.Sdk`.
* upgrade `log4net` to version 2.0.12.
