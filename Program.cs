using System;
using System.Management;
using System.Text;

namespace HardwareDetector
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hardware Detection Utility");
            Console.WriteLine("===========================");
            
            try
            {
                // Detect USB devices
                DetectUSBDevices();
                
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