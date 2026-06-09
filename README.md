# TASHBOARD Bridge

Windows Native Messaging bridge for the TASHBOARD Chrome extension.

[Privacy Policy](PRIVACY_POLICY.md)

The bridge enables user-initiated local features that Chrome extensions cannot
perform directly:

- choose and open local folders
- save and restore project configuration files
- show whether linked folders are open

## Install

Download `TashboardBridgeSetup.exe` from the latest release and run it once.
The installer registers the Native Messaging host for the TASHBOARD extension
under the current Windows user.

## Build

```powershell
powershell -ExecutionPolicy Bypass -File .\native-host\build_setup.ps1 -ExtensionId <chrome-extension-id>
```

The installer is created at:

```text
dist\TashboardBridgeSetup.exe
```

## Security

The bridge only responds to the TASHBOARD extension ID embedded during the
installer build. Native Messaging requests and responses use Chrome's standard
JSON-over-stdio protocol.
