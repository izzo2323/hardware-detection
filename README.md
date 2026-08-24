# Hardware Detector

A C# console application that detects and displays hardware information on Windows systems using WMI (Windows Management Instrumentation).

## Features

- Operating system information (name, version, architecture, install date)
- Motherboard information (manufacturer, model, serial number)
- BIOS information (manufacturer, version, release date)
- Detection of USB devices and controllers
- CPU information (name, manufacturer, cores, threads, clock speed)
- RAM information (total memory, memory modules)
- Disk drive information (model, manufacturer, size)
- Graphics adapter information (adapter RAM, driver version, video mode)
- Network adapter information (name, MAC address, adapter type, connection status)
- Monitor information (name, resolution)

## Requirements

- Windows operating system with WMI support
- .NET 8.0 SDK

## Usage

Run the application on a Windows machine to display hardware information:

```bash
dotnet run
```

## Important Note

This application only works on Windows systems as it uses Windows Management Instrumentation (WMI) for hardware queries. On non-Windows platforms, it will show a warning and skip the hardware detection operations.
