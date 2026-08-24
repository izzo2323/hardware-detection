using System;
using System.Management;
using System.Text;
using System.Runtime.InteropServices;

namespace HardwareDetector
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hardware Detection Utility");
            Console.WriteLine("===========================");

            // Check if we're running on Windows
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                Console.WriteLine("Warning: This application uses Windows Management Instrumentation (WMI) which is only available on Windows platforms.");
                Console.WriteLine("Some hardware information will not be displayed.");
                Console.WriteLine();
            }

            try
            {
                // Detect operating system information
                Console.WriteLine("\nOperating System:");
                Console.WriteLine("----------------------------------------");
                DetectOperatingSystemInfo();

                // Detect motherboard and BIOS information
                Console.WriteLine("\nMotherboard:");
                Console.WriteLine("----------------------------------------");
                DetectMotherboardInfo();

                Console.WriteLine("\nBIOS:");
                Console.WriteLine("----------------------------------------");
                DetectBIOSInfo();

                // Detect USB devices
                DetectUSBDevices();

                // Detect CPU information
                Console.WriteLine("\nCPU Information:");
                Console.WriteLine("----------------------------------------");
                DetectCPUInfo();

                // Detect RAM information
                Console.WriteLine("\nRAM Information:");
                Console.WriteLine("----------------------------------------");
                DetectRAMInfo();

                // Detect disk drives
                Console.WriteLine("\nDisk Drives:");
                Console.WriteLine("----------------------------------------");
                DetectDiskDrives();

                // Detect graphics adapters
                Console.WriteLine("\nGraphics Adapters:");
                Console.WriteLine("----------------------------------------");
                DetectGraphicsAdapters();

                // Detect network adapters
                Console.WriteLine("\nNetwork Adapters:");
                Console.WriteLine("----------------------------------------");
                DetectNetworkAdapters();

                // Detect monitors
                Console.WriteLine("\nMonitors:");
                Console.WriteLine("----------------------------------------");
                DetectMonitors();

                Console.WriteLine("\nPress any key to exit...");
                Console.ReadKey();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                Console.WriteLine("Press any key to exit...");
                Console.ReadKey();
            }
        }

        static void DetectUSBDevices()
        {
            Console.WriteLine("\nUSB Devices:");
            Console.WriteLine("----------------------------------------");

            try
            {
                ManagementObjectSearcher searcher = new ManagementObjectSearcher(
                    "SELECT * FROM Win32_USBHub");

                foreach (ManagementObject usbDevice in searcher.Get())
                {
                    string deviceId = GetProperty(usbDevice, "DeviceID");
                    string description = GetProperty(usbDevice, "Description");
                    string name = GetProperty(usbDevice, "Name");
                    string caption = GetProperty(usbDevice, "Caption");

                    Console.WriteLine($"Device ID: {deviceId}");
                    Console.WriteLine($"Description: {description}");
                    Console.WriteLine($"Name: {name}");
                    Console.WriteLine($"Caption: {caption}");
                    Console.WriteLine("----------------------------------------");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error detecting USB devices: {ex.Message}");
            }

            // Also try to get more detailed USB controller information
            DetectUSBControllers();
        }

        static void DetectUSBControllers()
        {
            Console.WriteLine("\nUSB Controllers:");
            Console.WriteLine("----------------------------------------");

            try
            {
                ManagementObjectSearcher searcher = new ManagementObjectSearcher(
                    "SELECT * FROM Win32_USBController");

                foreach (ManagementObject controller in searcher.Get())
                {
                    string deviceID = GetProperty(controller, "DeviceID");
                    string description = GetProperty(controller, "Description");
                    string name = GetProperty(controller, "Name");
                    string driverVersion = GetProperty(controller, "DriverVersion");

                    Console.WriteLine($"Controller: {name}");
                    Console.WriteLine($"Description: {description}");
                    Console.WriteLine($"Device ID: {deviceID}");
                    Console.WriteLine($"Driver Version: {driverVersion}");
                    Console.WriteLine("----------------------------------------");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error detecting USB controllers: {ex.Message}");
            }
        }

        static void DetectCPUInfo()
        {
            try
            {
                ManagementObjectSearcher searcher = new ManagementObjectSearcher(
                    "SELECT * FROM Win32_Processor");

                foreach (ManagementObject cpu in searcher.Get())
                {
                    string name = GetProperty(cpu, "Name");
                    string manufacturer = GetProperty(cpu, "Manufacturer");
                    string cores = GetProperty(cpu, "NumberOfCores");
                    string threads = GetProperty(cpu, "NumberOfLogicalProcessors");
                    string maxClockSpeed = GetProperty(cpu, "MaxClockSpeed");

                    Console.WriteLine($"Name: {name}");
                    Console.WriteLine($"Manufacturer: {manufacturer}");
                    Console.WriteLine($"Cores: {cores}");
                    Console.WriteLine($"Logical Processors (Threads): {threads}");
                    Console.WriteLine($"Max Clock Speed (MHz): {maxClockSpeed}");
                    Console.WriteLine("----------------------------------------");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error detecting CPU info: {ex.Message}");
            }
        }

        static void DetectRAMInfo()
        {
            try
            {
                ManagementObjectSearcher searcher = new ManagementObjectSearcher(
                    "SELECT * FROM Win32_ComputerSystem");

                foreach (ManagementObject computer in searcher.Get())
                {
                    string totalPhysicalMemory = GetProperty(computer, "TotalPhysicalMemory");
                    
                    // Convert bytes to GB for better readability
                    double memoryGB = double.Parse(totalPhysicalMemory) / (1024 * 1024 * 1024);
                    
                    Console.WriteLine($"Total Physical Memory: {memoryGB:F2} GB");
                    Console.WriteLine("----------------------------------------");
                }
                
                // Also get detailed RAM information
                ManagementObjectSearcher memorySearcher = new ManagementObjectSearcher(
                    "SELECT * FROM Win32_PhysicalMemory");
                
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
                
                if (memoryModules > 0)
                {
                    double totalMemoryGB = totalMemoryBytes / (1024.0 * 1024.0 * 1024.0);
                    Console.WriteLine($"Total Memory from Physical Memory Modules: {totalMemoryGB:F2} GB");
                    Console.WriteLine($"Number of Memory Modules: {memoryModules}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error detecting RAM info: {ex.Message}");
            }
        }

        static void DetectDiskDrives()
        {
            try
            {
                ManagementObjectSearcher searcher = new ManagementObjectSearcher(
                    "SELECT * FROM Win32_DiskDrive");

                foreach (ManagementObject disk in searcher.Get())
                {
                    string model = GetProperty(disk, "Model");
                    string manufacturer = GetProperty(disk, "Manufacturer");
                    string size = GetProperty(disk, "Size");
                    
                    // Convert bytes to GB for better readability
                    double sizeGB = double.Parse(size) / (1024 * 1024 * 1024);
                    
                    Console.WriteLine($"Model: {model}");
                    Console.WriteLine($"Manufacturer: {manufacturer}");
                    Console.WriteLine($"Size: {sizeGB:F2} GB");
                    Console.WriteLine("----------------------------------------");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error detecting disk drives: {ex.Message}");
            }
        }

        static void DetectGraphicsAdapters()
        {
            try
            {
                ManagementObjectSearcher searcher = new ManagementObjectSearcher(
                    "SELECT * FROM Win32_DisplayControllerConfiguration");

                foreach (ManagementObject adapter in searcher.Get())
                {
                    string adapterRAM = GetProperty(adapter, "AdapterRAM");
                    string driverVersion = GetProperty(adapter, "DriverVersion");
                    string videoModeDescription = GetProperty(adapter, "VideoModeDescription");
                    
                    // Convert bytes to MB for better readability
                    double adapterRAMMB = double.Parse(adapterRAM) / (1024 * 1024);
                    
                    Console.WriteLine($"Adapter RAM: {adapterRAMMB:F2} MB");
                    Console.WriteLine($"Driver Version: {driverVersion}");
                    Console.WriteLine($"Video Mode: {videoModeDescription}");
                    Console.WriteLine("----------------------------------------");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error detecting graphics adapters: {ex.Message}");
            }
        }

        static void DetectOperatingSystemInfo()
        {
            try
            {
                ManagementObjectSearcher searcher = new ManagementObjectSearcher(
                    "SELECT * FROM Win32_OperatingSystem");

                foreach (ManagementObject os in searcher.Get())
                {
                    string caption = GetProperty(os, "Caption");
                    string version = GetProperty(os, "Version");
                    string architecture = GetProperty(os, "OSArchitecture");
                    string installDate = GetProperty(os, "InstallDate");

                    Console.WriteLine($"Name: {caption}");
                    Console.WriteLine($"Version: {version}");
                    Console.WriteLine($"Architecture: {architecture}");
                    Console.WriteLine($"Install Date: {installDate}");
                    Console.WriteLine("----------------------------------------");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error detecting operating system info: {ex.Message}");
            }
        }

        static void DetectMotherboardInfo()
        {
            try
            {
                ManagementObjectSearcher searcher = new ManagementObjectSearcher(
                    "SELECT * FROM Win32_BaseBoard");

                foreach (ManagementObject board in searcher.Get())
                {
                    string manufacturer = GetProperty(board, "Manufacturer");
                    string product = GetProperty(board, "Product");
                    string serialNumber = GetProperty(board, "SerialNumber");

                    Console.WriteLine($"Manufacturer: {manufacturer}");
                    Console.WriteLine($"Model: {product}");
                    Console.WriteLine($"Serial Number: {serialNumber}");
                    Console.WriteLine("----------------------------------------");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error detecting motherboard info: {ex.Message}");
            }
        }

        static void DetectBIOSInfo()
        {
            try
            {
                ManagementObjectSearcher searcher = new ManagementObjectSearcher(
                    "SELECT * FROM Win32_BIOS");

                foreach (ManagementObject bios in searcher.Get())
                {
                    string manufacturer = GetProperty(bios, "Manufacturer");
                    string version = GetProperty(bios, "SMBIOSBIOSVersion");
                    string releaseDate = GetProperty(bios, "ReleaseDate");

                    Console.WriteLine($"Manufacturer: {manufacturer}");
                    Console.WriteLine($"Version: {version}");
                    Console.WriteLine($"Release Date: {releaseDate}");
                    Console.WriteLine("----------------------------------------");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error detecting BIOS info: {ex.Message}");
            }
        }

        static void DetectNetworkAdapters()
        {
            try
            {
                ManagementObjectSearcher searcher = new ManagementObjectSearcher(
                    "SELECT * FROM Win32_NetworkAdapter WHERE PhysicalAdapter = True");

                foreach (ManagementObject adapter in searcher.Get())
                {
                    string name = GetProperty(adapter, "Name");
                    string macAddress = GetProperty(adapter, "MACAddress");
                    string adapterType = GetProperty(adapter, "AdapterType");
                    string netEnabled = GetProperty(adapter, "NetEnabled");

                    Console.WriteLine($"Name: {name}");
                    Console.WriteLine($"MAC Address: {macAddress}");
                    Console.WriteLine($"Adapter Type: {adapterType}");
                    Console.WriteLine($"Connected: {netEnabled}");
                    Console.WriteLine("----------------------------------------");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error detecting network adapters: {ex.Message}");
            }
        }

        static void DetectMonitors()
        {
            try
            {
                ManagementObjectSearcher searcher = new ManagementObjectSearcher(
                    "SELECT * FROM Win32_DesktopMonitor");

                foreach (ManagementObject monitor in searcher.Get())
                {
                    string name = GetProperty(monitor, "Name");
                    string screenWidth = GetProperty(monitor, "ScreenWidth");
                    string screenHeight = GetProperty(monitor, "ScreenHeight");

                    Console.WriteLine($"Name: {name}");
                    Console.WriteLine($"Resolution: {screenWidth} x {screenHeight}");
                    Console.WriteLine("----------------------------------------");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error detecting monitors: {ex.Message}");
            }
        }

        static string GetProperty(ManagementObject mo, string propertyName)
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