using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Day3
{
    internal static class Program
    {
        private static void Main()
        {
            Console.WriteLine($"Part 1: {Part1()}");
            Console.WriteLine($"Part 2: {Part2()}");
        }

        private static string Part1()
        {
            var rucksackCompartments = File.ReadAllText("input.txt").Trim().Split(Environment.NewLine).Select(t => (t.Substring(0, t.Length / 2), t.Substring(t.Length / 2))).ToArray();
            var sharedItemPerRucksack = rucksackCompartments.Select(t => t.Item1.First(u => t.Item2.Contains(u))).ToArray();
            var score = sharedItemPerRucksack.Sum(GetItemScore);
            return $"{score} ({string.Join(", ", sharedItemPerRucksack)})";
        }

        private static string Part2()
        {
            var rucksackGroups = new List<(string, string, string)>();
            var rucksacks = File.ReadAllText("input.txt").Trim().Split(Environment.NewLine).ToArray();
            for (var i = 0; i < rucksacks.Length; i += 3) rucksackGroups.Add((rucksacks[i], rucksacks[i + 1], rucksacks[i + 2]));
            var sharedItemPerGroup = rucksackGroups.Select(t => t.Item1.First(u => t.Item2.Contains(u) && t.Item3.Contains(u))).ToArray();
            var score = sharedItemPerGroup.Sum(GetItemScore);
            return $"{score} ({string.Join(", ", sharedItemPerGroup)})";
        }

        private static int GetItemScore(char itemCharacter) => itemCharacter >= 'a' && itemCharacter <= 'z' ? 1 + itemCharacter - 'a' : 27 + itemCharacter - 'A';
    }
}