using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Hyphen.Tools.ProjectRenamer
{
    class Program
    {
        public static string STRING_TO_REPLACE = "";
        public static string NEW_STRING = "";
        public static string DIRECTORY = @"";
        public static string[] WHITELIST = new string[] { ".git", ".vs", "bin", "obj" };

        static void Main(string[] args)
        {
            ShowMenu();
            var chose = AskVariables();

            if (chose)
            {
                Console.WriteLine(@"");
                ReplaceFiles(DIRECTORY);
            }

            Console.WriteLine(@"");
            Console.WriteLine(@"Process finished, press a key to exit.");
            Console.ReadKey();
        }

        public static void ShowMenu()
        {
            Console.WriteLine(@"    __                __             ");
            Console.WriteLine(@"   / /_  __  ______  / /_  ___  ____ ");
            Console.WriteLine(@"  / __ \/ / / / __ \/ __ \/ _ \/ __ \");
            Console.WriteLine(@" / / / / /_/ / /_/ / / / /  __/ / / /");
            Console.WriteLine(@"/_/ /_/\__, / .___/_/ /_/\___/_/ /_/");
            Console.WriteLine(@"      /____/_/");
            Console.WriteLine(@"");
            Console.WriteLine(@"Hyphen.Tools.ProjectRenamer - A tool to rename files and folders");
            Console.WriteLine(@"https://github.com/makehyphen/Hyphen.Tools.ProjectRenamer");
            Console.WriteLine(@"");
        }

        public static bool AskVariables()
        {
            Console.Write(@"String to replace: ");
            STRING_TO_REPLACE = Console.ReadLine();

            Console.Write(@"New string: ");
            NEW_STRING = Console.ReadLine();

            Console.Write(@"Directory to work on: ");
            DIRECTORY = Console.ReadLine();

            Console.WriteLine(@"");
            Console.Write(@"Continue (y/n*): ");
            string chose = Console.ReadLine();

            switch (chose.ToLower())
            {
                case "y":
                    return true;
                case "n":
                default:
                    return false;
            }


        }

        public static void ReplaceFiles(string path)
        {
            // First directories
            var directories = Directory.GetDirectories(path);
            foreach (string directory in directories)
            {
                var actualDirectory = directory.Split("\\").Last();
                if (!WHITELIST.Any(s => actualDirectory.Contains(s) || actualDirectory.StartsWith(".")))
                {
                    // Replace inner files
                    ReplaceFiles(directory);

                    // Replace folder
                    var split = directory.Split("\\").ToList();

                    if (split.Last().Contains(STRING_TO_REPLACE))
                    {
                        split.RemoveAt(split.Count - 1);
                        split.Add(directory.Split("\\").Last().Replace(STRING_TO_REPLACE, NEW_STRING));

                        var newFinalName = String.Join("\\", split.ToArray());
                        Directory.Move(directory, newFinalName);

                        Console.WriteLine($"[{DateTime.Now}] - Renaming folder {directory} to {newFinalName}");
                    }
                }
            }

            // Then files in the current directory
            var files = Directory.GetFiles(path);
            foreach (string file in files)
            {
                var actualFilename = file.Split("\\").Last();

                if (!WHITELIST.Any(s => actualFilename.Contains(s) || actualFilename.StartsWith(".")))
                {
                    // Get inner
                    var inner = File.ReadAllText(file);

                    if (inner.Contains(STRING_TO_REPLACE))
                    {
                        // Replace inner content 
                        var newInner = inner.Replace(STRING_TO_REPLACE, NEW_STRING);
                        File.WriteAllText(file, newInner);

                        Console.WriteLine($"[{DateTime.Now}] - Replacing content from {file}");
                    }

                    if (actualFilename.Contains(STRING_TO_REPLACE))
                    {
                        // Replace string only on the last part
                        var split = file.Split("\\").ToList();
                        split.RemoveAt(split.Count - 1);
                        split.Add(file.Split("\\").Last().Replace(STRING_TO_REPLACE, NEW_STRING));

                        var newFinalName = String.Join("\\", split.ToArray());
                        File.Move(file, newFinalName);

                        Console.WriteLine($"[{DateTime.Now}] - Renaming file from {file} to {newFinalName}");
                    }
                }
            }
        }

        public static void RenameFolders(string path)
        {

            var directories = Directory.GetDirectories(path);

            if (directories.Length > 0)
            {
                // Each directory first
                foreach (string directory in directories)
                {
                    ReplaceFiles(directory);
                }
            }
            else
            {
                // Rename
                var newName = path.Replace(STRING_TO_REPLACE, NEW_STRING);
                File.Move(path, newName);
            }
        }
    }
}
