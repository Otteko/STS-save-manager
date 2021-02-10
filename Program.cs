using System.IO;
using System;
namespace ConsoleApp1
    //Hello code warrior :)
{
    public static class Program
    {
        public static void Main(String[] args)
        {
            Console.WriteLine("Slay the Spire save transfer: PC <-> Mobile. Tested as of 2/9/2021");
            Console.WriteLine("This is open source software, find me on github at:");
            Console.WriteLine("https://github.com/Otteko/STS-save-manager");
            Console.WriteLine("While this tool was tested, it's impossible to find all bugs");
            Console.WriteLine("Therefore, I do not provide a warranty on your computer or your saves");
            Console.WriteLine("This tool is not guarenteed to work and could *potentially* ruin saves. Back up your saves first!");
            Console.WriteLine("Currently not supported features: Transfer in progress run, transfer run history");
            Console.WriteLine("The file structure are different for these files and will cause crashes. This is being worked on");
            if (!dialogueOption("Do you want to continue?"))
            {
                System.Environment.Exit(0);
            }
            Console.Clear();
            Console.WriteLine("First, I want to find your PC's Slay the Spire installation, I will try to automatically detect it now...");
            var gameFound = false;
            String path = "";
            string[] directories = {"C:\\Program Files (x86)\\Steam\\steamapps\\common\\SlayTheSpire", "D:\\Steam\\steamapps\\common\\SlayTheSpire"};
            for (int i = 0; i < directories.Length && !gameFound; i++)
            {
                if (Directory.Exists(directories[i])) 
                {
                    path = directories[i];
                    Console.WriteLine("I've found your Slay the Spire installation at " + path + ".");
                    if (!dialogueOption("Is this the installation you would like to use?"))
                    {
                        //Need to locate path
                        Console.WriteLine("Drag and drop your Slay the Spire folder into this window and press enter.");
                        Console.WriteLine("For Steam users, you can navigate to the game in your Steam library -> right click Slay the Spire -> Local Files -> Browse.");
                        while (!gameFound)
                        {
                            Console.WriteLine("Drag and drop your Slay the Spire folder into this window and press enter.");
                            path = Console.ReadLine();
                            if (Directory.Exists(path + "\\saves"))
                            {
                                gameFound = true;
                                Console.WriteLine("Thanks, I've found your game!");
                            }
                            else
                            {
                                Console.WriteLine("I couldn't locate your save data, try again please.");
                            }
                        }
                    }
                }
            }
            if (path == "")
            {
                Console.WriteLine("I could not find your Slay the Spire installation automatically.");
                Console.WriteLine("Drag and drop your Slay the Spire folder into this window and press enter.");
                Console.WriteLine("For Steam users, you can navigate to the game in your Steam library -> right click Slay the Spire -> Local Files -> Browse.");
                while (!gameFound)
                {
                    Console.WriteLine("Drag and drop your Slay the Spire folder into this window and press enter.");
                    path = Console.ReadLine();
                    if (Directory.Exists(path + "\\preferences"))
                    {
                        gameFound = true;
                        Console.WriteLine("Thanks, I've found your game at "+ path);
                        Console.WriteLine("Press any key to continue");
                        Console.ReadKey();
                    }
                    else
                    {
                        Console.WriteLine("I couldn't locate your save data, try again please.");
                    }
                }
            }
            Console.Clear();
            Console.WriteLine("Next, I will look for your mobile phone's save. If it isn't plugged in, plug it in now.");
            mtpBrowser mtp = new mtpBrowser();
            mtp.openDevice();
            Console.WriteLine("I've found your device, let me look for the Slay the Spire app...");
         findgame:
            string mobilePath = mtp.findGame();
            if (mobilePath == "")
            {
                Console.WriteLine("Sorry, I could not find your mobile game installation. Try:");
                showHelp();
                goto findgame;
            }
         
            Console.WriteLine("Great! I've found your Slay the Spire installation and mobile app!");
            Console.WriteLine("Press any key to go to main menu...");
            Console.ReadKey();
            Console.Clear();
        UserSelection:
            Console.WriteLine("What would you like to do?");
            Console.WriteLine("[!] WARNING: TRANSFERRING SAVES OVERWRITES THAT SAVE, CONSIDER BACKING UP FIRST! [!]");
            Console.WriteLine("1: Steam -> Mobile");
            Console.WriteLine("2: Mobile -> Steam");
            Console.WriteLine("3: Backup saves");
            Console.WriteLine("4: Delete phone save game");
            Console.WriteLine("5: Help!");
            Console.WriteLine("6: Exit");
            int userResponse = 0;
            try
            {
                userResponse = Convert.ToInt32(Console.ReadLine());
            }
            catch
            {
                goto UserSelection;
            }
            if (userResponse == 1)
            {
                //Steam -> Mobile
                mtp.uploadFolder(path + "\\preferences\\", mobilePath + "preferences");
                Console.Clear();
                Console.WriteLine("✓ Uploaded game save to phone ✓ ");
                goto UserSelection;
            } else if (userResponse == 2)
            {
                mtp.downloadFolder(mobilePath + "preferences\\", path + "\\preferences");
                Console.Clear();
                Console.WriteLine("✓ Downloaded game save from phone to Steam ✓" + path);
                goto UserSelection;
            } else if (userResponse == 3)
            {
                //Backup
                var backupLocation = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\Desktop\\Slay the Spire backups";
                mtp.downloadFolder(mobilePath, backupLocation + "\\Mobile");
                Console.WriteLine(path);
                Directory.CreateDirectory(backupLocation);
                Directory.CreateDirectory(backupLocation + "\\Steam\\preferences");
                Directory.CreateDirectory(backupLocation + "\\Steam\\runs");
                Directory.CreateDirectory(backupLocation + "\\Steam\\saves");
                copyWinFolder(path + "\\preferences", backupLocation + "\\Steam\\preferences");
                copyWinFolder(path + "\\runs", backupLocation + "\\Steam\\runs");
                copyWinFolder(path + "\\saves", backupLocation + "\\Steam\\saves");
                Console.Clear();
                Console.WriteLine("✓ Created a backup of saves to: " + backupLocation + " ✓");
                goto UserSelection;
            } else if (userResponse == 4)
            {
                if (dialogueOption("[!] THIS WILL IRREVERSIBLY WIPE YOUR PHONE'S SLAY THE SPIRE SAVE, CONTINUE? [!]"))
                {
                    try
                    {
                        mtp.device.DeleteDirectory(mobilePath, true);
                        Console.Clear();
                        Console.WriteLine("✓ Deleted phone save ✓");
                        goto UserSelection;
                    } catch
                    {
                        Console.Clear();
                        Console.WriteLine("☒ Failed to delete phone save, not sure why :/ Delete and reinstall the app if you are having problems ☒");
                        goto UserSelection;
                    }
                }
            } else if (userResponse == 5)
            {
                Console.Clear();
                showHelp();
                goto UserSelection;
            } else if (userResponse == 6)
            {
                //Exit
                System.Environment.Exit(0);
            }
            Console.ReadKey(); //Keep this for pause
        }
        static Boolean dialogueOption(String message)
        {
            ConsoleKey userResponse;
            do
            {
                Console.Write(message + " [y/n]");
                userResponse = Console.ReadKey(false).Key;
                if (userResponse != ConsoleKey.Enter)
                {
                    Console.WriteLine();
                }

            } while (userResponse != ConsoleKey.Y && userResponse != ConsoleKey.N);
            if (userResponse == ConsoleKey.Y)
            {
                return true;
            } else
            {
                return false;
            }
        }
        static void copyWinFolder(string source, string destination)
        {
            foreach (string dirPath in Directory.GetDirectories(source, "*",
            SearchOption.AllDirectories))
                Directory.CreateDirectory(dirPath.Replace(source, destination));

            //Copy all the files & Replaces any files with the same name
            
            foreach (string newPath in Directory.GetFiles(source, "*.*",
                SearchOption.AllDirectories))
                File.Copy(newPath, newPath.Replace(source, destination), true);

        }
        static void showHelp()
        {
            Console.WriteLine("Having trouble? Try the following steps:");
            Console.WriteLine("1. Make sure phone is plugged in and UNLOCKED, Especially if you are using a passcode.");
            Console.WriteLine("2. Make sure your phone is set to MTP or PTP mode. Do NOT use \"USB File transfer\" even if you have the option");
            Console.WriteLine("3. Make sure the Slay the Spire app has been opened once, especially after using 4: Reset phone save game");
            Console.WriteLine("4. Make sure the Slay the Spire app is NOT currently running (Open multitasking -> Swipe out application, wait a few moments)");
            Console.WriteLine("5. Try a different USB port and different USB cable");
            Console.WriteLine("6. Turn off USB debugging and make sure no USB services are taking over the connection (Advanced users)");
            Console.WriteLine("Press any key to continue..");
            Console.ReadKey();
        }
    }
}