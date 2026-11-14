# StayAwake

A lightweight Windows desktop utility that asks Windows to remain awake using the native `SetThreadExecutionState` API. It does not change power-plan settings, simulate mouse or keyboard activity, and does not require administrator elevation.

## Features

- Keep the system awake indefinitely or for 30 minutes, 1, 2, 4, or 8 hours.
- Optionally request that the display remain on.
- System-tray menu and countdown.
- Releases its execution-state request when stopped or when the app exits.
- Runs as the current user (`asInvoker` manifest).

## Requirements

- Windows 10/11
- .NET 10 SDK to build from source

## Build

```powershell
dotnet build .\StayAwake\StayAwake.csproj -c Release
```

## Publish a portable single-file executable

Run `.\publish.ps1` from the repository root. Output: `artifacts\win-x64\StayAwake.exe`.

```powershell
.\publish.ps1
```

This prevents automatic idle sleep while active. It does not override lid-close or power-button behavior, forced shutdown, hibernation policy, or administrator-enforced restrictions.

## Privacy

No network access, telemetry, analytics, or account sign-in.
