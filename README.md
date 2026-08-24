# Hardware Detector

A C# application that detects and displays hardware information on Windows systems using WMI (Windows Management Instrumentation). It can run either as a one-shot console report or as a local web console.

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

### Console report

Run on a Windows machine to print a one-shot hardware report:

```bash
dotnet run
```

### Web console

Start a local web server with a live dashboard instead:

```bash
dotnet run -- --web
```

Then open http://localhost:8090/ in a browser. The dashboard fetches its data from a JSON API at `/api/hardware`, which is regenerated on every request (each page load or click of Refresh re-queries WMI). Use `--port=<number>` to run on a different port, e.g. `dotnet run -- --web --port=9000`.

## Important Note

This application only works on Windows systems as it uses Windows Management Instrumentation (WMI) for hardware queries. On non-Windows platforms, it will show a warning and skip the hardware detection operations.
