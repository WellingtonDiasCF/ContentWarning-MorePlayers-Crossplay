# Changelog

## 1.1.0

- Added an on-screen status card when the host enters a room or loads a world.
- The card shows that HostOnlyLobby is active and displays the configured player limit.
- Added `ShowStatusIndicator` and `IndicatorSeconds` interface settings.
- Delayed the card until the world is visible so loading screens do not consume its display time.

## 1.0.1

- Added startup diagnostics for all seven host-side patches.
- Added a Windows installer with Steam discovery, dependency downloads and SHA-256 verification.
- Added automatic disabling of conflicting Virality DLLs without deleting them.
- Added configurable lobby sizes from 5 to 16 players.
- Added headless integration tests for the host-side lobby rules.

## 1.0.0

- Initial host-only lobby expansion.
