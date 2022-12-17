using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Day14 {
    internal static class Program {
        private static void Main() {
            Console.WriteLine($"Part 1: {Part1()}");
            Console.WriteLine("\n-----------------\n");
            Console.WriteLine($"Part 2: {Part2()}");
        }


        private enum Material {
            Rock = 1,
            Sand = 2
        }

        private static string Part1() {
            var grid = ReadInput();
            var lowerRockY = grid.Keys.Min(t => t.y);
            var sandSpawn = new Vector2Int(500, 0);
            var fallingForever = false;
            while (!fallingForever) {
                var sandPosition = sandSpawn;
                var resting = false;
                while (!fallingForever && !resting) {
                    if (sandPosition.y < lowerRockY) fallingForever = true;
                    else if (!grid.ContainsKey(sandPosition + Vector2Int.down)) sandPosition += Vector2Int.down;
                    else if (!grid.ContainsKey(sandPosition + Vector2Int.downLeft)) sandPosition += Vector2Int.downLeft;
                    else if (!grid.ContainsKey(sandPosition + Vector2Int.downRight)) sandPosition += Vector2Int.downRight;
                    else {
                        resting = true;
                        grid.Add(sandPosition, Material.Sand);
                    }
                }
            }


            return $"{grid.Values.Count(t => t == Material.Sand)}{Environment.NewLine}{DisplayGrid(grid)}";
        }

        private static string Part2() {
            var grid = ReadInput();
            var lowerY = grid.Keys.Min(t => t.y);
            var sandSpawn = new Vector2Int(500, 0);
            while (!grid.ContainsKey(sandSpawn)) {
                var sandPosition = sandSpawn;
                var resting = false;
                while (!resting) {
                    if (sandPosition.y <= lowerY) resting = true;
                    if (!grid.ContainsKey(sandPosition + Vector2Int.down)) sandPosition += Vector2Int.down;
                    else if (!grid.ContainsKey(sandPosition + Vector2Int.downLeft)) sandPosition += Vector2Int.downLeft;
                    else if (!grid.ContainsKey(sandPosition + Vector2Int.downRight)) sandPosition += Vector2Int.downRight;
                    else resting = true;
                }

                grid.Add(sandPosition, Material.Sand);
            }


            return $"{grid.Values.Count(t => t == Material.Sand)}{Environment.NewLine}{DisplayGrid(grid)}";
        }


        private static string DisplayGrid(Dictionary<Vector2Int, Material> grid) {
            var minX = grid.Keys.Min(t => t.x);
            var maxX = grid.Keys.Max(t => t.x);
            var minY = grid.Keys.Min(t => t.y);
            const int maxY = 0;

            var result = new StringBuilder();
            for (var y = maxY; y >= minY; --y) {
                for (var x = minX; x <= maxX; ++x) {
                    if (!grid.ContainsKey((x, y))) result.Append('.');
                    else if (grid[(x, y)] == Material.Rock) result.Append('#');
                    else if (grid[(x, y)] == Material.Sand) result.Append('O');
                }

                result.Append(Environment.NewLine);
            }

            return result.ToString();
        }


        private static Dictionary<Vector2Int, Material> ReadInput() {
            var result = new Dictionary<Vector2Int, Material>();
            foreach (var rockChain in File.ReadAllLines("input.txt").Where(t => !string.IsNullOrEmpty(t)).Select(t => t.Trim().Split(" -> "))) {
                Vector2Int? previousRockChainPoint = default;
                foreach (var rockChainPoint in rockChain.Select(t => (Vector2Int) (int.Parse(t.Split(",")[0]), -int.Parse(t.Split(",")[1])))) {
                    if (!result.ContainsKey(rockChainPoint)) result.Add(rockChainPoint, Material.Rock);
                    if (previousRockChainPoint != null) {
                        for (var step = rockChainPoint; step != previousRockChainPoint; step = step.StepTowards(previousRockChainPoint.Value)) {
                            if (!result.ContainsKey(step)) result.Add(step, Material.Rock);
                        }
                    }

                    previousRockChainPoint = rockChainPoint;
                }
            }

            return result;
        }
    }
}