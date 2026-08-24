using System;
using System.Collections.Generic;
using System.Management;
using System.Runtime.InteropServices;

namespace HardwareDetector
{
    public static class HardwareInspector
    {
        public static HardwareSnapshot Collect()
        {
            var snapshot = new HardwareSnapshot
            {
                IsWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows),
                GeneratedAt = DateTime.Now
            };

            if (!snapshot.IsWindows)
            {
                snapshot.SectionErrors["General"] =
                    "This utility uses Windows Management Instrumentation (WMI), which is only available on Windows. Hardware data is unavailable on this platform.";
                return snapshot;
            }

            snapshot.OperatingSystem = CollectOperatingSystemInfo(snapshot.SectionErrors);
            snapshot.Motherboard = CollectMotherboardInfo(snapshot.SectionErrors);
            snapshot.Bios = CollectBiosInfo(snapshot.SectionErrors);
            snapshot.UsbDevices = CollectUsbDevices(snapshot.SectionErrors);
            snapshot.UsbControllers = CollectUsbControllers(snapshot.SectionErrors);
            snapshot.Cpus = CollectCpus(snapshot.SectionErrors);
            snapshot.Ram = CollectRamInfo(snapshot.SectionErrors);
            snapshot.DiskDrives = CollectDiskDrives(snapshot.SectionErrors);
            snapshot.GraphicsAdapters = CollectGraphicsAdapters(snapshot.SectionErrors);
            snapshot.NetworkAdapters = CollectNetworkAdapters(snapshot.SectionErrors);
            snapshot.Monitors = CollectMonitors(snapshot.SectionErrors);

            return snapshot;
        }

        private static OperatingSystemInfo? CollectOperatingSystemInfo(Dictionary<string, string> errors)
        {
            try
            {
                var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_OperatingSystem");
                foreach (ManagementObject os in searcher.Get())
                {
                    return new OperatingSystemInfo
                    {
                        Name = GetProperty(os, "Caption"),
                        Version = GetProperty(os, "Version"),
                        Architecture = GetProperty(os, "OSArchitecture"),
                        InstallDate = GetProperty(os, "InstallDate")
                    };
                }
            }
            catch (Exception ex)
            {
                errors["Operating System"] = ex.Message;
            }
            return null;
        }

        private static MotherboardInfo? CollectMotherboardInfo(Dictionary<string, string> errors)
        {
            try
            {
                var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_BaseBoard");
                foreach (ManagementObject board in searcher.Get())
                {
                    return new MotherboardInfo
                    {
                        Manufacturer = GetProperty(board, "Manufacturer"),
                        Model = GetProperty(board, "Product"),
                        SerialNumber = GetProperty(board, "SerialNumber")
                    };
                }
            }
            catch (Exception ex)
            {
                errors["Motherboard"] = ex.Message;
            }
            return null;
        }

        private static BiosInfo? CollectBiosInfo(Dictionary<string, string> errors)
        {
            try
            {
                var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_BIOS");
                foreach (ManagementObject bios in searcher.Get())
                {
                    return new BiosInfo
                    {
                        Manufacturer = GetProperty(bios, "Manufacturer"),
                        Version = GetProperty(bios, "SMBIOSBIOSVersion"),
                        ReleaseDate = GetProperty(bios, "ReleaseDate")
                    };
                }
            }
            catch (Exception ex)
            {
                errors["BIOS"] = ex.Message;
            }
            return null;
        }

        private static List<UsbDeviceInfo> CollectUsbDevices(Dictionary<string, string> errors)
        {
            var devices = new List<UsbDeviceInfo>();
            try
            {
                var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_USBHub");
                foreach (ManagementObject usbDevice in searcher.Get())
                {
                    devices.Add(new UsbDeviceInfo
                    {
                        DeviceId = GetProperty(usbDevice, "DeviceID"),
                        Description = GetProperty(usbDevice, "Description"),
                        Name = GetProperty(usbDevice, "Name"),
                        Caption = GetProperty(usbDevice, "Caption")
                    });
                }
            }
            catch (Exception ex)
            {
                errors["USB Devices"] = ex.Message;
            }
            return devices;
        }

        private static List<UsbControllerInfo> CollectUsbControllers(Dictionary<string, string> errors)
        {
            var controllers = new List<UsbControllerInfo>();
            try
            {
                var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_USBController");
                foreach (ManagementObject controller in searcher.Get())
                {
                    controllers.Add(new UsbControllerInfo
                    {
                        Name = GetProperty(controller, "Name"),
                        Description = GetProperty(controller, "Description"),
                        DeviceId = GetProperty(controller, "DeviceID"),
                        DriverVersion = GetProperty(controller, "DriverVersion")
                    });
                }
            }
            catch (Exception ex)
            {
                errors["USB Controllers"] = ex.Message;
            }
            return controllers;
        }

        private static List<CpuInfo> CollectCpus(Dictionary<string, string> errors)
        {
            var cpus = new List<CpuInfo>();
            try
            {
                var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_Processor");
                foreach (ManagementObject cpu in searcher.Get())
                {
                    cpus.Add(new CpuInfo
                    {
                        Name = GetProperty(cpu, "Name"),
                        Manufacturer = GetProperty(cpu, "Manufacturer"),
                        Cores = GetProperty(cpu, "NumberOfCores"),
                        Threads = GetProperty(cpu, "NumberOfLogicalProcessors"),
                        MaxClockSpeedMHz = GetProperty(cpu, "MaxClockSpeed")
                    });
                }
            }
            catch (Exception ex)
            {
                errors["CPU"] = ex.Message;
            }
            return cpus;
        }

        private static RamInfo? CollectRamInfo(Dictionary<string, string> errors)
        {
            try
            {
                var info = new RamInfo();

                var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_ComputerSystem");
                foreach (ManagementObject computer in searcher.Get())
                {
                    string totalPhysicalMemory = GetProperty(computer, "TotalPhysicalMemory");
                    if (double.TryParse(totalPhysicalMemory, out double bytes))
                    {
                        info.TotalPhysicalMemoryGB = bytes / (1024 * 1024 * 1024);
                    }
                }

                var memorySearcher = new ManagementObjectSearcher("SELECT * FROM Win32_PhysicalMemory");
                long totalMemoryBytes = 0;
                int memoryModules = 0;

                foreach (ManagementObject memory in memorySearcher.Get())
                {
                    string capacity = GetProperty(memory, "Capacity");
                    if (long.TryParse(capacity, out long memCapacity))
                    {
                        totalMemoryBytes += memCapacity;
                        memoryModules++;
                    }
                }

                info.TotalMemoryFromModulesGB = totalMemoryBytes / (1024.0 * 1024.0 * 1024.0);
                info.MemoryModules = memoryModules;

                return info;
            }
            catch (Exception ex)
            {
                errors["RAM"] = ex.Message;
            }
            return null;
        }

        private static List<DiskDriveInfo> CollectDiskDrives(Dictionary<string, string> errors)
        {
            var drives = new List<DiskDriveInfo>();
            try
            {
                var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_DiskDrive");
                foreach (ManagementObject disk in searcher.Get())
                {
                    string size = GetProperty(disk, "Size");
                    double.TryParse(size, out double sizeBytes);

                    drives.Add(new DiskDriveInfo
                    {
                        Model = GetProperty(disk, "Model"),
                        Manufacturer = GetProperty(disk, "Manufacturer"),
                        SizeGB = sizeBytes / (1024 * 1024 * 1024)
                    });
                }
            }
            catch (Exception ex)
            {
                errors["Disk Drives"] = ex.Message;
            }
            return drives;
        }

        private static List<GraphicsAdapterInfo> CollectGraphicsAdapters(Dictionary<string, string> errors)
        {
            var adapters = new List<GraphicsAdapterInfo>();
            try
            {
                var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_DisplayControllerConfiguration");
                foreach (ManagementObject adapter in searcher.Get())
                {
                    string adapterRAM = GetProperty(adapter, "AdapterRAM");
                    double.TryParse(adapterRAM, out double adapterRamBytes);

                    adapters.Add(new GraphicsAdapterInfo
                    {
                        AdapterRamMB = adapterRamBytes / (1024 * 1024),
                        DriverVersion = GetProperty(adapter, "DriverVersion"),
                        VideoMode = GetProperty(adapter, "VideoModeDescription")
                    });
                }
            }
            catch (Exception ex)
            {
                errors["Graphics Adapters"] = ex.Message;
            }
            return adapters;
        }

        private static List<NetworkAdapterInfo> CollectNetworkAdapters(Dictionary<string, string> errors)
        {
            var adapters = new List<NetworkAdapterInfo>();
            try
            {
                var searcher = new ManagementObjectSearcher(
                    "SELECT * FROM Win32_NetworkAdapter WHERE PhysicalAdapter = True");
                foreach (ManagementObject adapter in searcher.Get())
                {
                    adapters.Add(new NetworkAdapterInfo
                    {
                        Name = GetProperty(adapter, "Name"),
                        MacAddress = GetProperty(adapter, "MACAddress"),
                        AdapterType = GetProperty(adapter, "AdapterType"),
                        Connected = GetProperty(adapter, "NetEnabled")
                    });
                }
            }
            catch (Exception ex)
            {
                errors["Network Adapters"] = ex.Message;
            }
            return adapters;
        }

        private static List<MonitorInfo> CollectMonitors(Dictionary<string, string> errors)
        {
            var monitors = new List<MonitorInfo>();
            try
            {
                var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_DesktopMonitor");
                foreach (ManagementObject monitor in searcher.Get())
                {
                    monitors.Add(new MonitorInfo
                    {
                        Name = GetProperty(monitor, "Name"),
                        ScreenWidth = GetProperty(monitor, "ScreenWidth"),
                        ScreenHeight = GetProperty(monitor, "ScreenHeight")
                    });
                }
            }
            catch (Exception ex)
            {
                errors["Monitors"] = ex.Message;
            }
            return monitors;
        }

        private static string GetProperty(ManagementObject mo, string propertyName)
        {
            try
            {
                return mo[propertyName]?.ToString() ?? "N/A";
            }
            catch
            {
                return "N/A";
            }
        }
    }
}
