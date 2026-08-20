using System;
using System.Collections.Generic;
using System.Management;
using System.Text;

namespace HardwareDetector
{
    // Class to represent a device and its physical location
    public class DeviceLocation
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int LocationX { get; set; }
        public int LocationY { get; set; }
        public string DeviceId { get; set; }
        public string ConnectorType { get; set; }
    }

    // Class for visualizing device locations on a machine back panel
    public class DeviceLayoutVisualizer
    {
        private const int BACK_PANEL_WIDTH = 80;
        private const int BACK_PANEL_HEIGHT = 24;
        
        // Predefined connector types to map with their locations on computer back panel
        public static Dictionary<string, (int x, int y)> ConnectorLocations { get; } = 
            new Dictionary<string, (int x, int y)>
            {
                {"USB 2.0", (5, 4)},
                {"USB 3.0", (15, 4)},
                {"USB 3.1", (25, 4)},
                {"Ethernet", (5, 10)},
                {"Audio In", (5, 16)},
                {"Audio Out", (15, 16)},
                {"Video Out", (25, 16)},
                {"PS/2 Keyboard", (35, 4)},
                {"PS/2 Mouse", (45, 4)},
                {"VGA", (55, 4)},
                {"DVI", (55, 10)},
                {"HDMI", (65, 10)},
                {"DisplayPort", (65, 16)}
            };

        public static void DisplayDeviceLayout(List<DeviceLocation> devices)
        {
            // Create a simple ASCII representation of the back of a computer
            char[,] panel = new char[BACK_PANEL_HEIGHT, BACK_PANEL_WIDTH];
            
            // Initialize with empty spaces
            for (int y = 0; y < BACK_PANEL_HEIGHT; y++)
            {
                for (int x = 0; x < BACK_PANEL_WIDTH; x++)
                {
                    panel[y, x] = ' ';
                }
            }
            
            // Draw the computer back panel frame
            DrawFrame(panel);
            
            // Mark connector locations with labels
            foreach (var kvp in ConnectorLocations)
            {
                string connector = kvp.Key;
                var location = kvp.Value;
                if (location.x < BACK_PANEL_WIDTH && location.y < BACK_PANEL_HEIGHT)
                {
                    // Draw connector name and coordinates
                    int index = 0;
                    foreach (char c in connector)
                    {
                        if (location.x + index >= BACK_PANEL_WIDTH) break;
                        panel[location.y, location.x + index] = c;
                        index++;
                    }
                }
            }
            
            // Place devices on the layout based on their location
            foreach (var device in devices)
            {
                var location = GetDevicePositionByType(device.ConnectorType);
                
                if (location != null && 
                    location.Value.x < BACK_PANEL_WIDTH && 
                    location.Value.y < BACK_PANEL_HEIGHT)
                {
                    // Place an indicator character for the device at its position
                    panel[location.Value.y, location.Value.x] = 'D';
                    
                    // Add a label showing device type for better understanding
                    string deviceLabel = $"{device.Description.Substring(0, Math.Min(4, device.Description.Length))}";
                    int labelX = location.Value.x + 2;
                    if (labelX < BACK_PANEL_WIDTH)
                    {
                        int index = 0;
                        foreach (char c in deviceLabel)
                        {
                            if (labelX + index >= BACK_PANEL_WIDTH) break;
                            panel[location.Value.y, labelX + index] = c;
                            index++;
                        }
                    }
                }
            }
            
            // Display the visualization
            Console.WriteLine("\nDevice Layout Visualization:");
            Console.WriteLine("============================");
            for (int y = 0; y < BACK_PANEL_HEIGHT; y++)
            {
                StringBuilder line = new StringBuilder();
                for (int x = 0; x < BACK_PANEL_WIDTH; x++)
                {
                    line.Append(panel[y, x]);
                }
                Console.WriteLine(line.ToString());
            }
        }
        
        private static (int x, int y)? GetDevicePositionByType(string deviceType)
        {
            // Map USB devices to standard positions
            if (deviceType.Contains("USB"))
            {
                return ConnectorLocations.GetValueOrDefault(deviceType, (0, 0));
            }
            
            // For other connectors, use the predefined mapping or a standard position if not found
            return ConnectorLocations.GetValueOrDefault(deviceType, null);
        }
        
        private static void DrawFrame(char[,] panel)
        {
            // Draw top border
            for (int x = 0; x < BACK_PANEL_WIDTH; x++)
            {
                panel[0, x] = '-';
            }
            
            // Draw bottom border
            for (int x = 0; x < BACK_PANEL_WIDTH; x++)
            {
                panel[BACK_PANEL_HEIGHT - 1, x] = '-';
            }
            
            // Draw left and right borders
            for (int y = 0; y < BACK_PANEL_HEIGHT; y++)
            {
                panel[y, 0] = '|';
                panel[y, BACK_PANEL_WIDTH - 1] = '|';
            }
            
            // Draw corners
            panel[0, 0] = '+';
            panel[0, BACK_PANEL_WIDTH - 1] = '+';
            panel[BACK_PANEL_HEIGHT - 1, 0] = '+';
            panel[BACK_PANEL_HEIGHT - 1, BACK_PANEL_WIDTH - 1] = '+';
        }
    }
}