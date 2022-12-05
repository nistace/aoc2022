using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Day5
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
            var inputParts = File.ReadAllText("input.txt").Split($"{Environment.NewLine}{Environment.NewLine}");
            var cargo = new Cargo(inputParts[0]);
            var moveInstructions = Regex.Matches(inputParts[1], "move (\\d+) from (\\d+) to (\\d+)");
            foreach (var match in moveInstructions.ToImmutableArray()) cargo.Move9000(int.Parse(match.Groups[1].Value), int.Parse(match.Groups[2].Value), int.Parse(match.Groups[3].Value));
            return $"{cargo.PeekTops()}\n{cargo}";
        }

        private static string Part2()
        {
            var inputParts = File.ReadAllText("input.txt").Split($"{Environment.NewLine}{Environment.NewLine}");
            var cargo = new Cargo(inputParts[0]);
            var moveInstructions = Regex.Matches(inputParts[1], "move (\\d+) from (\\d+) to (\\d+)");
            foreach (var match in moveInstructions.ToImmutableArray()) cargo.Move9001(int.Parse(match.Groups[1].Value), int.Parse(match.Groups[2].Value), int.Parse(match.Groups[3].Value));
            return $"{cargo.PeekTops()}\n{cargo}";
        }

        private class Cargo
        {
            private Dictionary<int, List<char>> stacks { get; } = new Dictionary<int, List<char>>();


            public Cargo(string input)
            {
                var lines = input.Split(Environment.NewLine).ToList();
                var stackNumberLine = lines.Last();
                lines.RemoveAt(lines.Count - 1);
                while (stackNumberLine.Length > 0)
                {
                    if (stackNumberLine[0] != ' ')
                    {
                        stacks.Add(int.Parse($"{stackNumberLine[0]}"), lines.Where(crate => crate[0] != ' ').Select(t => t[0]).ToList());
                    }

                    stackNumberLine = stackNumberLine.Substring(1);
                    for (var i = 0; i < lines.Count; ++i)
                    {
                        if (lines[i].Length > 0) lines[i] = lines[i].Substring(1);
                    }
                }
            }


            public void Move9000(int timesCount, int from, int to)
            {
                for (var i = 0; i < timesCount; i++)
                {
                    stacks[to].Insert(0, stacks[from][0]);
                    stacks[from].RemoveAt(0);
                }
            }

            public void Move9001(int timesCount, int from, int to)
            {
                for (var i = 0; i < timesCount; i++)
                {
                    stacks[to].Insert(i, stacks[from][0]);
                    stacks[from].RemoveAt(0);
                }
            }

            public override string ToString() => string.Join("\n", stacks.Select(t => (t.Key, t.Value)).OrderBy(t => t.Key).Select(t => $"{t.Key}: {string.Join(", ", ((IEnumerable<char>) t.Value).Reverse())}"));

            public string PeekTops() => string.Join("", stacks.Select(t => (t.Key, t.Value[0])).OrderBy(t => t.Key).Select(t => t.Item2));
        }
    }
}