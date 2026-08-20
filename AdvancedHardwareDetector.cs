using System;
using System.Management;
using System.Text;

namespace HardwareDetector
{
    class AdvancedHardwareDetector
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Advanced Hardware Detection Utility");
            Console.WriteLine("=====================================");
            
            try
            {
                // Detect various types of hardware devices
                DetectUSBDevices();
                Console.WriteLine();
                
                DetectNetworkAdapters();
                Console.WriteLine();
                
                DetectGraphicsAdapters();
                Console.WriteLine();
                
                DetectStorageDevices();
                Console.WriteLine();
                
                // Display summary
                DisplaySystemSummary();
                
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
            Console.WriteLine("USB Devices:");
            Console.WriteLine("----------------------------------------");

            try
            {
                ManagementObjectSearcher searcher = new ManagementObjectSearcher(
                    "SELECT * FROM Win32_PnPEntity WHERE DeviceID LIKE '%USB%'");
                
                int deviceCount = 0;
                foreach (ManagementObject usbDevice in searcher.Get())
                {
                    string deviceId = GetProperty(usbDevice, "DeviceID");
                    string description = GetProperty(usbDevice, "Description");
                    string name = GetProperty(usbDevice, "Name");
                    string service = GetProperty(usbDevice, "Service");
                    
                    // Filter out USB hubs and focus on actual devices
                    if (deviceId.Contains("USB") && !string.IsNullOrEmpty(description) && 
                        !description.Contains("USB Root Hub"))
                    {
                        Console.WriteLine($"Device: {name}");
                        Console.WriteLine($"Description: {description}");
                        Console.WriteLine($"Service: {service}");
                        Console.WriteLine($"ID: {deviceId}");
                        Console.WriteLine("----------------------------------------");
                        deviceCount++;
                    }
                }
                
                if (deviceCount == 0)
                {
                    Console.WriteLine("No USB devices detected.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error detecting USB devices: {ex.Message}");
            }
        }

        static void DetectNetworkAdapters()
        {
            Console.WriteLine("Network Adapters:");
            Console.WriteLine("----------------------------------------");

            try
            {
                ManagementObjectSearcher searcher = new ManagementObjectSearcher(
                    "SELECT * FROM Win32_NetworkAdapter WHERE NetEnabled = True");
                
                foreach (ManagementObject adapter in searcher.Get())
                {
                    string description = GetProperty(adapter, "Description");
                    string name = GetProperty(adapter, "Name");
                    string macAddress = GetProperty(adapter, "MACAddress");
                    string speed = GetProperty(adapter, "Speed");
                    
                    Console.WriteLine($"Adapter: {name}");
                    Console.WriteLine($"Description: {description}");
                    Console.WriteLine($"MAC Address: {macAddress}");
                    Console.WriteLine($"Speed: {speed} bps");
                    Console.WriteLine("----------------------------------------");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error detecting network adapters: {ex.Message}");
            }
        }

        static void DetectGraphicsAdapters()
        {
            Console.WriteLine("Graphics Adapters:");
            Console.WriteLine("----------------------------------------");

            try
            {
                ManagementObjectSearcher searcher = new ManagementObjectSearcher(
                    "SELECT * FROM Win32_VideoController");
                
                foreach (ManagementObject adapter in searcher.Get())
                {
                    string description = GetProperty(adapter, "Description");
                    string name = GetProperty(adapter, "Name");
                    string videoMemory = GetProperty(adapter, "AdapterRAM");
                    string driverVersion = GetProperty(adapter, "DriverVersion");
                    
                    Console.WriteLine($"Adapter: {name}");
                    Console.WriteLine($"Description: {description}");
                    Console.WriteLine($"Video Memory: {videoMemory} bytes");
                    Console.WriteLine($"Driver Version: {driverVersion}");
                    Console.WriteLine("----------------------------------------");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error detecting graphics adapters: {ex.Message}");
            }
        }

        static void DetectStorageDevices()
        {
            Console.WriteLine("Storage Devices:");
            Console.WriteLine("----------------------------------------");

            try
            {
                ManagementObjectSearcher searcher = new ManagementObjectSearcher(
                    "SELECT * FROM Win32_DiskDrive");
                
                foreach (ManagementObject drive in searcher.Get())
                {
                    string model = GetProperty(drive, "Model");
                    string size = GetProperty(drive, "Size");
                    string interfaceType = GetProperty(drive, "InterfaceType");
                    string deviceId = GetProperty(drive, "DeviceID");
                    
                    Console.WriteLine($"Drive: {model}");
                    Console.WriteLine($"Size: {size} bytes");
                    Console.WriteLine($"Interface Type: {interfaceType}");
                    Console.WriteLine($"Device ID: {deviceId}");
                    Console.WriteLine("----------------------------------------");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error detecting storage devices: {ex.Message}");
            }
        }

        static void DisplaySystemSummary()
        {
            Console.WriteLine("System Summary:");
            Console.WriteLine("----------------------------------------");
            
            try
            {
                // Get system information
                ManagementObjectSearcher searcher = new ManagementObjectSearcher(
                    "SELECT * FROM Win32_ComputerSystem");
                
                foreach (ManagementObject computer in searcher.Get())
                {
                    string name = GetProperty(computer, "Name");
                    string manufacturer = GetProperty(computer, "Manufacturer");
                    string model = GetProperty(computer, "Model");
                    
                    Console.WriteLine($"Computer: {name}");
                    Console.WriteLine($"Manufacturer: {manufacturer}");
                    Console.WriteLine($"Model: {model}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting system information: {ex.Message}");
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