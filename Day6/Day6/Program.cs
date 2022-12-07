using System;
using System.IO;
using System.Linq;

namespace Day6
{
    internal static class Program
    {
        private static void Main()
        {
            Console.WriteLine($"Part 1: {Part1()}");
            Console.WriteLine("\n-----------------\n");
            Console.WriteLine($"Part 2: {Part2()}");
        }

        private static string Part1() => FindMarker(4);
        private static string Part2() => FindMarker(14);

        private static string FindMarker(int markerSize)
        {
            var input = File.ReadAllText("input.txt");
            return $"{input.Select((t, i) => i).First(index => index >= markerSize && input.Substring(index - markerSize, markerSize).Distinct().Count() == markerSize)}";
        }
    }
}