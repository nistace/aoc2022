using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Day15 {
    internal static class Program {
        private static void Main() {
            Console.WriteLine($"Part 1: {Part1()}");
            Console.WriteLine("\n-----------------\n");
            Console.WriteLine($"Part 2: {Part2()}");
        }


        private enum CellContent {
            Sensor = 1,
            Beacon = 2,
            Nothing = 3
        }

        private static string Part1() {
            const int line = 10;
            var lineContent = new Dictionary<int, CellContent>();
            foreach (var (sensor, beacon, manhattanDistance) in ReadInput()) {
                if (beacon.y == line) {
                    if (!lineContent.ContainsKey(beacon.x)) lineContent.Add(beacon.x, CellContent.Beacon);
                    else if (lineContent[beacon.x] == CellContent.Nothing) lineContent[beacon.x] = CellContent.Beacon;
                }

                for (var x = sensor.x - manhattanDistance; x <= sensor.x + manhattanDistance; ++x) {
                    if (sensor.GetManhattanDistance((x, line)) <= manhattanDistance && !lineContent.ContainsKey(x))
                        lineContent.Add(x, CellContent.Nothing);
                }
            }

            return $"{lineContent.Count(t => t.Value == CellContent.Nothing || t.Value == CellContent.Sensor)}";
        }

        private static string Part2() {
            const int max = 4000000;
            var pairs = ReadInput().ToArray();
            var candidates = pairs
                .SelectMany(t => t.sensor.FindAllAtManhattanDistance(t.manhattanDistance + 1).Where(p => p.x >= 0 && p.x <= max && p.y >= 0 && p.y <= max))
                .GroupBy(t => t).Where(t => t.Count() > 3)
                .Select(t => t.Key)
                .ToArray();
            foreach (var candidate in candidates) {
                if (pairs.All(u => candidate.GetManhattanDistance(u.sensor) > u.manhattanDistance)) {
                    return $"{candidate.x * 4000000L + candidate.y}{Environment.NewLine}{candidate}";
                }
            }

            return "ERROR, no solution.";
        }


        private static IEnumerable<(Vector2Int sensor, Vector2Int beacon, int manhattanDistance)> ReadInput() {
            var result = new HashSet<(Vector2Int, Vector2Int, int)>();
            foreach (var match in File.ReadAllLines("input.txt").Select(t => Regex.Match(t.Trim(), "Sensor.*x=([\\-\\d]+).*y=([\\-\\d]+).*x=([\\-\\d]+).*y=([\\-\\d]+)")).Where(t => t.Success)) {
                var sensor = (Vector2Int) (int.Parse(match.Groups[1].Value), int.Parse(match.Groups[2].Value));
                var beacon = (Vector2Int) (int.Parse(match.Groups[3].Value), int.Parse(match.Groups[4].Value));
                result.Add((sensor, beacon, sensor.GetManhattanDistance(beacon)));
            }

            return result;
        }
    }
}