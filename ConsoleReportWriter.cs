using System;

namespace HardwareDetector
{
    public static class ConsoleReportWriter
    {
        private const string Divider = "----------------------------------------";

        public static void Write(HardwareSnapshot snapshot)
        {
            Console.WriteLine("Hardware Detection Utility");
            Console.WriteLine("===========================");

            if (!snapshot.IsWindows)
            {
                Console.WriteLine("Warning: This application uses Windows Management Instrumentation (WMI) which is only available on Windows platforms.");
                Console.WriteLine("Some hardware information will not be displayed.");
                Console.WriteLine();
            }

            WriteSection("Operating System", snapshot.SectionErrors, "Operating System", () =>
            {
                if (snapshot.OperatingSystem is { } os)
                {
                    Console.WriteLine($"Name: {os.Name}");
                    Console.WriteLine($"Version: {os.Version}");
                    Console.WriteLine($"Architecture: {os.Architecture}");
                    Console.WriteLine($"Install Date: {os.InstallDate}");
                    Console.WriteLine(Divider);
                }
            });

            WriteSection("Motherboard", snapshot.SectionErrors, "Motherboard", () =>
            {
                if (snapshot.Motherboard is { } board)
                {
                    Console.WriteLine($"Manufacturer: {board.Manufacturer}");
                    Console.WriteLine($"Model: {board.Model}");
                    Console.WriteLine($"Serial Number: {board.SerialNumber}");
                    Console.WriteLine(Divider);
                }
            });

            WriteSection("BIOS", snapshot.SectionErrors, "BIOS", () =>
            {
                if (snapshot.Bios is { } bios)
                {
                    Console.WriteLine($"Manufacturer: {bios.Manufacturer}");
                    Console.WriteLine($"Version: {bios.Version}");
                    Console.WriteLine($"Release Date: {bios.ReleaseDate}");
                    Console.WriteLine(Divider);
                }
            });

            WriteSection("USB Devices", snapshot.SectionErrors, "USB Devices", () =>
            {
                foreach (var device in snapshot.UsbDevices)
                {
                    Console.WriteLine($"Device ID: {device.DeviceId}");
                    Console.WriteLine($"Description: {device.Description}");
                    Console.WriteLine($"Name: {device.Name}");
                    Console.WriteLine($"Caption: {device.Caption}");
                    Console.WriteLine(Divider);
                }
            });

            WriteSection("USB Controllers", snapshot.SectionErrors, "USB Controllers", () =>
            {
                foreach (var controller in snapshot.UsbControllers)
                {
                    Console.WriteLine($"Controller: {controller.Name}");
                    Console.WriteLine($"Description: {controller.Description}");
                    Console.WriteLine($"Device ID: {controller.DeviceId}");
                    Console.WriteLine($"Driver Version: {controller.DriverVersion}");
                    Console.WriteLine(Divider);
                }
            });

            WriteSection("CPU Information", snapshot.SectionErrors, "CPU", () =>
            {
                foreach (var cpu in snapshot.Cpus)
                {
                    Console.WriteLine($"Name: {cpu.Name}");
                    Console.WriteLine($"Manufacturer: {cpu.Manufacturer}");
                    Console.WriteLine($"Cores: {cpu.Cores}");
                    Console.WriteLine($"Logical Processors (Threads): {cpu.Threads}");
                    Console.WriteLine($"Max Clock Speed (MHz): {cpu.MaxClockSpeedMHz}");
                    Console.WriteLine(Divider);
                }
            });

            WriteSection("RAM Information", snapshot.SectionErrors, "RAM", () =>
            {
                if (snapshot.Ram is { } ram)
                {
                    Console.WriteLine($"Total Physical Memory: {ram.TotalPhysicalMemoryGB:F2} GB");
                    Console.WriteLine(Divider);
                    Console.WriteLine($"Total Memory from Physical Memory Modules: {ram.TotalMemoryFromModulesGB:F2} GB");
                    Console.WriteLine($"Number of Memory Modules: {ram.MemoryModules}");
                }
            });

            WriteSection("Disk Drives", snapshot.SectionErrors, "Disk Drives", () =>
            {
                foreach (var disk in snapshot.DiskDrives)
                {
                    Console.WriteLine($"Model: {disk.Model}");
                    Console.WriteLine($"Manufacturer: {disk.Manufacturer}");
                    Console.WriteLine($"Size: {disk.SizeGB:F2} GB");
                    Console.WriteLine(Divider);
                }
            });

            WriteSection("Graphics Adapters", snapshot.SectionErrors, "Graphics Adapters", () =>
            {
                foreach (var adapter in snapshot.GraphicsAdapters)
                {
                    Console.WriteLine($"Adapter RAM: {adapter.AdapterRamMB:F2} MB");
                    Console.WriteLine($"Driver Version: {adapter.DriverVersion}");
                    Console.WriteLine($"Video Mode: {adapter.VideoMode}");
                    Console.WriteLine(Divider);
                }
            });

            WriteSection("Network Adapters", snapshot.SectionErrors, "Network Adapters", () =>
            {
                foreach (var adapter in snapshot.NetworkAdapters)
                {
                    Console.WriteLine($"Name: {adapter.Name}");
                    Console.WriteLine($"MAC Address: {adapter.MacAddress}");
                    Console.WriteLine($"Adapter Type: {adapter.AdapterType}");
                    Console.WriteLine($"Connected: {adapter.Connected}");
                    Console.WriteLine(Divider);
                }
            });

            WriteSection("Monitors", snapshot.SectionErrors, "Monitors", () =>
            {
                foreach (var monitor in snapshot.Monitors)
                {
                    Console.WriteLine($"Name: {monitor.Name}");
                    Console.WriteLine($"Resolution: {monitor.ScreenWidth} x {monitor.ScreenHeight}");
                    Console.WriteLine(Divider);
                }
            });
        }

        private static void WriteSection(string heading, System.Collections.Generic.Dictionary<string, string> errors, string errorKey, Action writeBody)
        {
            Console.WriteLine($"\n{heading}:");
            Console.WriteLine(Divider);

            if (errors.TryGetValue(errorKey, out var error))
            {
                Console.WriteLine($"Error detecting {heading.ToLowerInvariant()}: {error}");
                return;
            }

            writeBody();
        }
    }
}
