using System;
using System.Collections.Generic;

namespace HardwareDetector
{
    public class OperatingSystemInfo
    {
        public string Name { get; set; } = "N/A";
        public string Version { get; set; } = "N/A";
        public string Architecture { get; set; } = "N/A";
        public string InstallDate { get; set; } = "N/A";
    }

    public class MotherboardInfo
    {
        public string Manufacturer { get; set; } = "N/A";
        public string Model { get; set; } = "N/A";
        public string SerialNumber { get; set; } = "N/A";
    }

    public class BiosInfo
    {
        public string Manufacturer { get; set; } = "N/A";
        public string Version { get; set; } = "N/A";
        public string ReleaseDate { get; set; } = "N/A";
    }

    public class UsbDeviceInfo
    {
        public string DeviceId { get; set; } = "N/A";
        public string Description { get; set; } = "N/A";
        public string Name { get; set; } = "N/A";
        public string Caption { get; set; } = "N/A";
    }

    public class UsbControllerInfo
    {
        public string Name { get; set; } = "N/A";
        public string Description { get; set; } = "N/A";
        public string DeviceId { get; set; } = "N/A";
        public string DriverVersion { get; set; } = "N/A";
    }

    public class CpuInfo
    {
        public string Name { get; set; } = "N/A";
        public string Manufacturer { get; set; } = "N/A";
        public string Cores { get; set; } = "N/A";
        public string Threads { get; set; } = "N/A";
        public string MaxClockSpeedMHz { get; set; } = "N/A";
    }

    public class RamInfo
    {
        public double TotalPhysicalMemoryGB { get; set; }
        public double TotalMemoryFromModulesGB { get; set; }
        public int MemoryModules { get; set; }
    }

    public class DiskDriveInfo
    {
        public string Model { get; set; } = "N/A";
        public string Manufacturer { get; set; } = "N/A";
        public double SizeGB { get; set; }
    }

    public class GraphicsAdapterInfo
    {
        public double AdapterRamMB { get; set; }
        public string DriverVersion { get; set; } = "N/A";
        public string VideoMode { get; set; } = "N/A";
    }

    public class NetworkAdapterInfo
    {
        public string Name { get; set; } = "N/A";
        public string MacAddress { get; set; } = "N/A";
        public string AdapterType { get; set; } = "N/A";
        public string Connected { get; set; } = "N/A";
    }

    public class MonitorInfo
    {
        public string Name { get; set; } = "N/A";
        public string ScreenWidth { get; set; } = "N/A";
        public string ScreenHeight { get; set; } = "N/A";
    }

    public class HardwareSnapshot
    {
        public bool IsWindows { get; set; }
        public DateTime GeneratedAt { get; set; }

        public OperatingSystemInfo? OperatingSystem { get; set; }
        public MotherboardInfo? Motherboard { get; set; }
        public BiosInfo? Bios { get; set; }
        public List<UsbDeviceInfo> UsbDevices { get; set; } = new();
        public List<UsbControllerInfo> UsbControllers { get; set; } = new();
        public List<CpuInfo> Cpus { get; set; } = new();
        public RamInfo? Ram { get; set; }
        public List<DiskDriveInfo> DiskDrives { get; set; } = new();
        public List<GraphicsAdapterInfo> GraphicsAdapters { get; set; } = new();
        public List<NetworkAdapterInfo> NetworkAdapters { get; set; } = new();
        public List<MonitorInfo> Monitors { get; set; } = new();

        // Keyed by section name (e.g. "CPU"), set when that section's WMI query fails.
        public Dictionary<string, string> SectionErrors { get; set; } = new();
    }
}
