using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Day7
{
    internal static class Program
    {
        private static void Main()
        {
            Console.WriteLine($"Part 1: {Part1()}");
            Console.WriteLine("\n-----------------\n");
            Console.WriteLine($"Part 2: {Part2()}");
        }

        private static string Part1()
        {
            var rootDirectory = ParseInput();
            return $"{rootDirectory.GetWholeTreeDirectories().Select(t => t.size).Where(t => t < 100000).Sum():0}{Environment.NewLine}{rootDirectory}";
        }

        private static string Part2()
        {
            var rootDirectory = ParseInput();
            var spaceToFreeUp = rootDirectory.size - 40000000;
            return $"{rootDirectory.GetWholeTreeDirectories().Select(t => t.size).Where(t => t >= spaceToFreeUp).OrderBy(t => t).First():0}";
        }

        private static Directory ParseInput()
        {
            var root = new Directory(null);
            var currentDirectory = root;
            foreach (var cmdGroup in File.ReadAllText("input.txt").Split("$").Select(t => t.Split(Environment.NewLine).Select(u => u.Trim()).ToArray()))
            {
                switch (cmdGroup.First())
                {
                    case "cd /":
                        currentDirectory = root;
                        break;
                    case "cd ..":
                        currentDirectory = currentDirectory.parent;
                        break;
                    default:
                    {
                        Match match;
                        if ((match = Regex.Match(cmdGroup.First(), "cd (.+)")).Success)
                        {
                            currentDirectory = currentDirectory.GetDirectory(match.Groups[1].Value);
                        }
                        else if (cmdGroup.First() == "ls")
                        {
                            foreach (var lsResultLine in cmdGroup.Skip(1))
                            {
                                if ((match = Regex.Match(lsResultLine, "dir (.+)")).Success) currentDirectory.AddDirectory(match.Groups[1].Value);
                                else if ((match = Regex.Match(lsResultLine, "(\\d+) (.+)")).Success) currentDirectory.AddFile(int.Parse(match.Groups[1].Value), match.Groups[2].Value);
                                else if (!string.IsNullOrEmpty(lsResultLine)) Console.Error.WriteLine($"Couldn't parse '{lsResultLine}'");
                            }
                        }

                        break;
                    }
                }
            }

            return root;
        }

        private class Directory
        {
            public Directory parent { get; }
            private Dictionary<string, Directory> subDirectories { get; } = new Dictionary<string, Directory>();
            private Dictionary<string, int> files { get; } = new Dictionary<string, int>();

            public int size => files.Values.Sum() + subDirectories.Values.Sum(t => t.size);

            public Directory(Directory parent)
            {
                this.parent = parent;
            }

            public Directory GetDirectory(string name) => subDirectories[name];
            public void AddDirectory(string value) => subDirectories.Add(value, new Directory(this));
            public void AddFile(int fileSize, string name) => files.Add(name, fileSize);
            public IEnumerable<Directory> GetWholeTreeDirectories() => new[] {this}.Union(subDirectories.Values.SelectMany(t => t.GetWholeTreeDirectories()));

            public override string ToString() => $"DIR / {size:0}{Environment.NewLine}{ToString(2)}";

            private string ToString(int indent)
                => string.Join("", subDirectories.OrderBy(t => t.Key).Select(t => $"DIR {new string(' ', indent)}{t.Key} ({t.Value.size:0}){Environment.NewLine}{t.Value.ToString(indent + 2)}"))
                   + string.Join("", files.OrderBy(t => t.Key).Select(t => $"FIL {new string(' ', indent)}{t.Key} ({t.Value:0}){Environment.NewLine}"));
        }
    }
}