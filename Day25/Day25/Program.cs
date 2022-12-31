using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Day25 {
    internal static class Program {
        private static void Main() {
            Console.WriteLine($"Part 1: {Part1()}");
            Console.WriteLine("\n-----------------\n");
            Console.WriteLine($"Part 2: {Part2()}");
        }


        private static string Part1() {
            var input = ReadInput();

            var sum = input.Aggregate(Snafu.zero, (t, u) => t + u);

            return $"{sum}{Environment.NewLine}{sum}\t{sum.ToLong()}{Environment.NewLine}{string.Join(Environment.NewLine, input.Select(t => t))}";
        }


        private static string Part2() {
            return "";
        }

        private static IReadOnlyList<Snafu> ReadInput() => File.ReadAllLines("input.txt").Where(t => !string.IsNullOrEmpty(t) && Snafu.TryParse(t, out _)).Select(Snafu.Parse).ToArray();
    }
}