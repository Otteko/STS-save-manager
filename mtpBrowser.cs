using System;
using System.Linq;
using MediaDevices;
using System.Diagnostics;
using System.IO;

namespace ConsoleApp1
{
    class mtpBrowser
    {
        public MediaDevice device;

        public mtpBrowser(int deviceNumber)
        {

            this.device = getQuickDevice(deviceNumber);
        }

        public mtpBrowser()
        {
            this.device = getDevice();
        }

        public void openDevice()
        {
            this.device.Connect();
        }
        public void closeDevice()
        {
            this.device.Disconnect();
        }

        public String findGame()
        {
            String[] locations = { "\\Phone\\Android\\data\\com.humble.SlayTheSpire\\files\\", "\\Card\\Android\\data\\com.humble.SlayTheSpire\\files\\" };
            for (int i = 0; i < locations.Length; i++)
            {
                if (device.DirectoryExists(locations[i]))
                {
                    return locations[i];
                }
            }
            return "";
        }
        public void downloadFolder(String source, String destination)
        {
            device.DownloadFolder(source, destination, true);
        }
        public void uploadFolder(String source, String destination)
        {
            
            device.DeleteDirectory(destination, true);
            device.UploadFolder(source, destination, true);
        }
        public MediaDevice getDevice()
        {
        ScanDevices:
            Console.WriteLine("Scanning for devices...");
            var devices = MediaDevice.GetDevices();
            int i = 0;
            foreach (var deviceList in devices)
            {
                i++;
                Console.WriteLine(i + ": " + deviceList.FriendlyName + ".");
            }
            Console.WriteLine("Done.");
            Console.WriteLine("Using your keyboard, type in the number of the device you would like to transfer saves: (1 - " + i + ")");
            Console.WriteLine("Press 0 to rescan for devices.");
        UserSelection:
            int selectedDevice = 0;
            try
            {
                selectedDevice = Convert.ToInt32(Console.ReadLine());
            }
            catch
            {
                Console.Clear();
                goto UserSelection;
            }
            if (selectedDevice == 0)
            {
                Console.Clear();
                goto ScanDevices;
            }
            else if (selectedDevice > i || selectedDevice < 0)
            {
                Console.WriteLine("Invalid entry selection");
                goto UserSelection;
            }
            var device = devices.ElementAt(selectedDevice - 1);
            Console.Clear();
            Console.WriteLine("Selected: " + device.FriendlyName);
            device.Connect();
            return device;
        }
        public MediaDevice getQuickDevice(int item)
        {
            var devices = MediaDevice.GetDevices();
            var device = devices.ElementAt(item);
            Console.WriteLine("Quickly selected: " + device.FriendlyName);
            return device;
        }
    }
}
