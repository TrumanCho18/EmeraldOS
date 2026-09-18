using Cosmos.System.Network.Config;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using System.Net.Security;
using System.Text;
using Sys = Cosmos.System;

namespace EmeraldOS
{
    public class Kernel : Sys.Kernel
    {
        String version = "1.1.0";
        String titleTxt = " /$$$$$$$$ /$$      /$$ /$$$$$$$$ /$$$$$$$   /$$$$$$  /$$       /$$$$$$$ \r\n| $$_____/| $$$    /$$$| $$_____/| $$__  $$ /$$__  $$| $$      | $$__  $$\r\n| $$      | $$$$  /$$$$| $$      | $$  \\ $$| $$  \\ $$| $$      | $$  \\ $$\r\n| $$$$$   | $$ $$/$$ $$| $$$$$   | $$$$$$$/| $$$$$$$$| $$      | $$  | $$\r\n| $$__/   | $$  $$$| $$| $$__/   | $$__  $$| $$__  $$| $$      | $$  | $$\r\n| $$      | $$\\  $ | $$| $$      | $$  \\ $$| $$  | $$| $$      | $$  | $$\r\n| $$$$$$$$| $$ \\/  | $$| $$$$$$$$| $$  | $$| $$  | $$| $$$$$$$$| $$$$$$$/\r\n|________/|__/     |__/|________/|__/  |__/|__/  |__/|________/|_______/ ";

        ArrayList Users = new ArrayList();
        User currentUser;

        Directory root = new Directory("root", null);
        ArrayList dirs = new ArrayList();
        Directory currentDir;
        File currentFile;
        String path = "";
        String mode = "cmdLine";

        EMInterpreter interpreter = new EMInterpreter();
        protected override void BeforeRun()
        {
            dirs.Add(root);
            InitUsers();
            Console.Clear();
            Console.WriteLine(titleTxt + "\nVERSION: " + version);
        }

        protected override void Run()
        {
            Boolean loggedIn = false;
            while (!loggedIn)
            {
                //INITIALIZE LOGIN
                Console.Write("User: ");
                var input = Console.ReadLine();

                for (int i = 0; i < Users.Count; i++)
                {
                    if (Users[i] is User)
                    {
                        User check = (User)Users[i];
                        if (check.name == input)
                        {
                            currentUser = check;
                            break;
                        }
                    }
                }

                //Register new User
                if (currentUser == null)
                {
                    Console.WriteLine("User not found.");
                } else
                {
                    Console.Write("Password:");
                    String pw = GetMaskedPassword();

                    if (pw == currentUser.pw)
                    {
                        loggedIn = true;
                        Console.WriteLine("\nLogged in as " + currentUser.name + " at " + "(IP)");
                        currentDir = root;
                    } else
                    {
                        currentUser = null;
                        Console.WriteLine("\nLogin failed");
                    }
                }
            }

            while (true)
            {

                switch (mode)
                {

                    case "cmdLine":

                            Console.Write("E:" + currentUser.name + ">");
                            String input = Console.ReadLine();

                            switch (input) {

                            case "":
                                break;

                            case "help":
                                Console.WriteLine("IT WORKS");
                                break;

                            case "ls":
                                for (int i = 0; i < currentDir.Children.Count; i++)
                                {
                                    if (currentDir.Children[i] is Directory)
                                    {
                                        Directory idx = (Directory)currentDir.Children[i];
                                        Console.WriteLine(idx.name);
                                    } else if (currentDir.Children[i] is Cache)
                                    {
                                        Cache idx = (Cache)currentDir.Children[i];
                                        Console.WriteLine("$" + idx.name);
                                    } else if (currentDir.Children[i] is File)
                                    {
                                        File idx = (File)currentDir.Children[i];
                                        Console.WriteLine(idx.name);
                                    }
                                }
                                break;

                            case string str when (str.Length >= 2 && str.Substring(0, 2) == "cd"):
                                if (str.Split(" ").Length > 1)
                                {
                                    String targetDir = str.Split(" ")[1];

                                    if (targetDir == "..")
                                    {
                                        if (currentDir.Parent != null)
                                        {
                                            currentDir = currentDir.Parent;
                                            path = path.Substring(0, path.Length - currentDir.name.Length);
                                        }
                                        else
                                        {
                                            Console.WriteLine("The current Directory does not have a parent! (you are probably in the root directory)");
                                        }
                                    }
                                    else if (GetDirectory(targetDir, currentDir) != null)
                                    {
                                        currentDir = GetDirectory(targetDir, currentDir);
                                        path += "/" + targetDir;
                                    } else
                                    {
                                        Console.WriteLine("Directory \"" + targetDir + "\" not found in system memory");
                                    }

                                } else
                                {
                                    Console.WriteLine("Directory \"\" not found in system memory");
                                }
                                break;

                            case "pwd":
                                Console.WriteLine(GetPath(currentDir));
                                break;

                            case "clear":
                                Console.Clear();
                                break;

                            case "current":
                                Console.WriteLine(currentDir.name);
                                break;

                            case string str when (str.Length >= 2 && str.Substring(0, 2) == "em"):
                                if (str.Split(" ").Length == 2)
                                {
                                    String target = str.Split(" ")[1];
                                    if (GetFile(target, currentDir) != null)
                                    {
                                        interpreter.inp = GetFile(target, currentDir);
                                        interpreter.Interpret();
                                    }
                                    else
                                    {
                                        Console.WriteLine("Value " + target + " not found!");
                                    }

                                } else if (str.Split(" ").Length == 3)
                                {
                                    String arg = str.Split(" ")[1];
                                    if (arg == "-t")
                                    {
                                        String target = str.Split(" ")[2];
                                        if (GetFile(target, currentDir) != null)
                                        {
                                            interpreter.inp = GetFile(target, currentDir);
                                            interpreter.TokenList();
                                        }
                                        else
                                        {
                                            Console.WriteLine("Value " + target + " not found!");
                                        }
                                    }
                                }
                                break;

                            case string str when (str.Length >= 3 && str.Substring(0, 3) == "val"):
                                if (str.Split(" ").Length > 1)
                                {
                                    String target = str.Split(" ")[1];
                                    if (GetCache(target, currentDir) != null)
                                    {
                                        Console.WriteLine(GetCache(target, currentDir).value);
                                    }
                                    else
                                    {
                                        Console.WriteLine("Value " + target + " not found!");
                                    }

                                }
                                break;

                            case string str when (str.Length >= 3 && str.Substring(0, 3) == "cat"):
                                if (str.Split(" ").Length > 1)
                                {
                                    String target = str.Split(" ")[1];
                                    if (GetFile(target, currentDir) != null)
                                    {
                                        for (int i = 0; i < GetFile(target, currentDir).body.Count; i++)
                                        {
                                            Console.WriteLine(GetFile(target, currentDir).body[i]);

                                        }

                                    }
                                    else
                                    {
                                        Console.WriteLine("File " + target + " not found!");
                                    }
                                }
                                break;

                            case string str when (str.Length >= 4 && str.Substring(0, 4) == "info"):
                                if (str.Split(" ").Length > 1)
                                {
                                    String target = str.Split(" ")[1];
                                    if (GetCache(target, currentDir) != null)
                                    {
                                        Console.WriteLine("Name: " + GetCache(target, currentDir).name);
                                        Console.WriteLine("Type: " + GetCache(target, currentDir).type);
                                        Console.WriteLine("Value: " + GetCache(target, currentDir).value);
                                    } else
                                    {
                                        Console.WriteLine("Value " + target + " not found!");
                                    }
                                }
                                break;

                            case string str when (str.Length >= 4 && str.Substring(0, 4) == "oras"):
                                if (str.Split(" ").Length > 1)
                                {
                                    String target = str.Split(" ")[1];
                                    if (GetFile(target, currentDir) != null)
                                    {
                                        Console.Clear();
                                        currentFile = GetFile(target, currentDir);
                                        mode = "oras";
                                    }
                                    else
                                    {
                                        Console.WriteLine("File " + target + " not found!");
                                    }
                                }
                                break;

                            case string str when (str.Length >= 5 && str.Substring(0, 5) == "mkdir"):
                                if (str.Split(" ").Length > 1)
                                {
                                    String dirName = str.Split(" ")[1];
                                    Directory newDir = new Directory(dirName, currentDir);
                                    dirs.Add(newDir);
                                    currentDir.Children.Add(newDir);
                                }
                                break;

                            case string str when (str.Length >= 5 && str.Substring(0, 5) == "touch"):
                                if (str.Split(" ").Length > 1)
                                {
                                    String fileName = str.Split(" ")[1];
                                    File file = new File(fileName);
                                    file.Parent = currentDir;
                                    currentDir.Children.Add(file);
                                }
                                break;

                            case string str when (str.Length >= 5 && str.Substring(0, 5) == "store"):
                                if (str.Split(" ").Length > 2)
                                {
                                    String varName = str.Split(" ")[1];
                                    String varVal = str.Split(" ")[2];
                                    Cache newCache = new Cache(varName, varVal);
                                    currentDir.Children.Add(newCache);
                                }
                                break;

                            default:
                                Console.WriteLine("Command \"" + input + "\" not found");
                                break;
                        }
                            break;

                    case "oras":
                            

                        Oras(1, currentFile);
                        string inp = "";
                        ConsoleKeyInfo key;

                        do
                        {
                            key = Console.ReadKey(true);

                            if (key.Key != ConsoleKey.Backspace && key.Key != ConsoleKey.Enter && key.Key != ConsoleKey.Escape && key.Key != ConsoleKey.Tab && key.Key != ConsoleKey.Delete)
                            {
                                inp += key.KeyChar;
                                Console.Write(key.KeyChar);
                            } else if (key.Key == ConsoleKey.Escape)
                            {   
                                mode = "cmdLine";
                            } else if (key.Key == ConsoleKey.Tab)
                            {
                                inp += "   ";
                                Console.Write("   ");
                            }
                            else if (key.Key == ConsoleKey.Backspace)
                            {

                                if (inp.Length > 0)
                                {
                                    inp = inp.Remove(inp.Length - 1);
                                }
                                Console.Clear();
                                Oras(1, currentFile);
                                Console.Write(inp);
                            } else if (key.Key == ConsoleKey.Delete)
                            { 
                                if (inp == "")
                                {
                                    currentFile.Delete();

                                } else
                                {
                                    inp = "";
                                }
                                Console.Clear();
                                Oras(1, currentFile);
                            }

                        }
                        while (key.Key != ConsoleKey.Enter);

                        currentFile.body.Add(inp);
                        Console.Clear();

                        break;
                        

                } 
            }

        }
        

        public void InitUsers()
        {
            User Trumanc = new User("Trumanc", "234118453");
            Directory trumancHome = new Directory("home", root);
            dirs.Add(trumancHome);
            root.Children.Add(trumancHome);
            Users.Add(Trumanc);

            File welcomeFile = new File("ReadMe");       
            welcomeFile.Parent = trumancHome;
            trumancHome.Children.Add(welcomeFile);
        }

        public void Oras(int min, File file)
        {
            for (int i = 0; i < file.body.Count; i++)
            {
                Console.WriteLine(":" + file.body[i]);
            }
            Console.Write(":");
        }

        private static string GetMaskedPassword()
        {
            string pass = "";
            ConsoleKeyInfo key;

            do
            {
                key = Console.ReadKey(intercept: true);

                if (key.Key == ConsoleKey.Backspace && pass.Length > 0)
                {
                    pass = pass.Substring(0, pass.Length - 1);
                    Console.Write("\b \b");
                }
                
                else if (!char.IsControl(key.KeyChar))
                {
                    pass += key.KeyChar;
                    Console.Write("*"); 
                }
            }
            while (key.Key != ConsoleKey.Enter);

            return pass;
        }

        private static String GetPath(Directory Dir)
        {
            String ans = "";
            ArrayList ansList = new ArrayList();
            ansList.Add(Dir.name);

            Directory traveller = Dir;

            while (traveller.Parent != null)
            {
                ansList.Add(traveller.Parent.name);
                traveller = traveller.Parent;
            }

            for (int i = ansList.Count - 1; i >= 0; i--) 
            {
                ans += "/" + ansList[i];
            }

            return ans;
  
        }

        private Cache GetCache(String str, Directory dir)
        {
            for (int i = 0; i < dir.Children.Count; i++)
            {
                if (dir.Children[i] is Cache)
                {
                    Cache idx = (Cache)dir.Children[i];
                    if (idx.name == str)
                    {
                        return idx;
                    }
                }
            }
            return null;
        }

        private File GetFile(String str, Directory dir)
        {
            for (int i = 0; i < dir.Children.Count; i++)
            {
                if (dir.Children[i] is File)
                {
                    File idx = (File)dir.Children[i];
                    if (idx.name == str)
                    {
                        return idx;
                    }
                }
            }
            return null;
        }
        private Directory GetDirectory(String str, Directory dir)
        {
            for (int i = 0; i < dir.Children.Count; i++) {
                if (dir.Children[i] is Directory)
                {
                    Directory idx = (Directory)dir.Children[i];
                    if (idx.name == str)
                    {
                        return idx;
                    }
                }
            }
            return null;
        }
    }
}

