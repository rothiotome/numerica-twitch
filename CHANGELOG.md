# Changelog
All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [1.6.0] - 2023-08-18

### Added
- New Palette System to customize colors

### Changed
- Limitation in timeouts to avoid unvoluntary bans and timeouts larger than 2 weeks
- New TwitchChat namespace
- Updated TwitchChat package

## [1.5.1] - 2023-08-16

### Added
- Editor extension to customize TwitchOauth and make it easier to get ready
- Added TwitchAPI namespace

### Changed
- Login success and fail messages customizable from inspector
- Token obfuscation through useless UUID is now optional but encouraged

### Removed
- Secrets folder is no longer necessary

## [1.5.0] - 2023-08-08

### Added
- Remove the UniqueChatFilter and the NonModeratorChatDelay when the app connects to the chat

### Changed
- Several ports might be used instead of 8080. It also looks for a free port from a list
- Added useless UUID to response URL in order to hide the access token and avoid it to be leaked if someone is recording the screen

## [1.4.0] - 2023-07-31

### Changed
- New authentication method that uses OpenID Connect (OIDC) instead Oauth to avoid using secrets

### Removed
- Removed a few classes related to code authentication

## [1.3.2] - 2023-07-26

### Changed
- Fix recursive infinite loop on timeout

## [1.3.1] - 2023-07-24

### Changed
- Fix failed login after a few hours of use

## [1.2] - 2023-07-12

### Added
- Added temporary VIP mechanics
- Added Timeout immunity to MODS

### Changed
- Refactor CounterTwitchGame.cs so I don't feel embarrased of the bad code
- Adapt to Very Simple Twitch Controller chatter class
- Add itch.io integration so it updates the new version text when there's one available
