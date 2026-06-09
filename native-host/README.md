# TASHBOARD Native Bridge

This helper replaces the old `localhost:3001` Node bridge and `run_bridge.bat`.
The publish output is self-contained, so the user does not need Node.js or a separate .NET runtime.

## Build

```powershell
powershell -ExecutionPolicy Bypass -File .\native-host\build_bridge.ps1
```

## Build one-file setup for distribution

Replace `<extension-id>` with the final Chrome extension ID.

```powershell
powershell -ExecutionPolicy Bypass -File .\native-host\build_setup.ps1 -ExtensionId <extension-id>
```

The final user-facing file is:

```text
dist\TashboardBridgeSetup.exe
```

## Install for the current Windows user

Replace `<extension-id>` with the Chrome extension ID shown on `chrome://extensions`.

```powershell
powershell -ExecutionPolicy Bypass -File .\native-host\install_bridge.ps1 -ExtensionId <extension-id>
```

The installer writes a Native Messaging manifest and registers it under:

```text
HKCU\Software\Google\Chrome\NativeMessagingHosts\com.dashboard_zen.bridge
```

No Node.js install, terminal window, or always-running `.bat` process is required.
When distributing the bridge, keep the full `native-host\publish` folder together.
