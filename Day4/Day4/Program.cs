using System;
using System.IO;
using System.Linq;

namespace Day4
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
            var assignmentPairs = File.ReadAllText("input.txt").Trim().Split(Environment.NewLine).Select(t => new Pair(t));
            var overlappingPairs = assignmentPairs.Where(t => t.IsFullyContaining()).ToArray();
            var score = overlappingPairs.Length;
            return $"{score}\n{string.Join("\n", overlappingPairs.Take(5))}...";
        }

        private static string Part2()
        {
            var assignmentPairs = File.ReadAllText("input.txt").Trim().Split(Environment.NewLine).Select(t => new Pair(t));
            var overlappingPairs = assignmentPairs.Where(t => t.IsOverlapping()).ToArray();
            var score = overlappingPairs.Length;
            return $"{score}\n{string.Join("\n", overlappingPairs.Take(5))}...";
        }

        private class Pair
        {
            private Assignment left { get; }
            private Assignment right { get; }

            public Pair(string str)
            {
                left = new Assignment(str.Split(",")[0]);
                right = new Assignment(str.Split(",")[1]);
            }

            public bool IsFullyContaining() => left.Contains(right) || right.Contains(left);
            public bool IsOverlapping() => left.IsOverlappingUnder(right) || right.IsOverlappingUnder(left);
            public override string ToString() => $"{left},{right}";

            public class Assignment
            {
                private int min { get; }
                private int max { get; }

                public Assignment(string str)
                {
                    min = int.Parse(str.Split("-")[0]);
                    max = int.Parse(str.Split("-")[1]);
                }

                public bool Contains(Assignment other) => min <= other.min && max >= other.max;
                public bool IsOverlappingUnder(Assignment other) => other.min >= min && other.min <= max;
                public override string ToString() => $"{min}-{max}";
            }
        }
    }
}