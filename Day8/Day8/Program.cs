using System;
using System.IO;
using System.Linq;

namespace Day8
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
            var treesAndNeighbours = ReadInput();
            var count = treesAndNeighbours.Count(IsVisible);
            return $"{count}{Environment.NewLine}{string.Join(Environment.NewLine, treesAndNeighbours.Where(t => !IsVisible(t)).Take(30).Select(t => $"H {t.h}: <{t.sideLines[0]} >{t.sideLines[1]} ^{t.sideLines[2]} v{t.sideLines[3]}"))}";
        }


        private static string Part2()
        {
            var treesAndNeighbours = ReadInput();
            var scenicScores = treesAndNeighbours.Select(t => t.sideLines.Select(line => Math.Min(line.Length, line.TakeWhile(neighbour => neighbour < t.h).Count() + 1)).Aggregate(1, (x, y) => x * y)).ToArray();
            return $"{scenicScores.Max()}";
        }

        private static (char h, string[] sideLines)[] ReadInput()
        {
            var rows = File.ReadAllLines("input.txt").Where(t => !string.IsNullOrEmpty(t)).ToArray();
            var treesAndNeighbours = rows.SelectMany((row, y) => row.Select((cell, x) =>
                (h: cell,
                    sideLines: new[]
                    {
                        x < 1 ? string.Empty : string.Join("", row.Substring(0, x).Reverse()), // all trees on the left
                        x >= row.Length ? string.Empty : row.Substring(x + 1), // all trees on the right
                        y < 1 ? string.Empty : string.Join("", rows.Take(y).Reverse().Select(t => t[x])), // all trees up
                        y >= rows.Length ? string.Empty : string.Join("", rows.Skip(y + 1).Select(t => t[x])) // all trees down
                    }
                ))).ToArray();
            return treesAndNeighbours;
        }

        private static bool IsVisible((char h, string[] sideLines) tree) => tree.sideLines.Any(neighbourLine => neighbourLine.All(neighbourTree => neighbourTree < tree.h));
    }
}