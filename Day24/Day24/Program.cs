using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Day22;

namespace Day24 {
    internal static class Program {
        private static IEnumerable<Vector2Int> directions { get; } = new[] {Vector2Int.down, Vector2Int.right, Vector2Int.zero, Vector2Int.up, Vector2Int.left};


        private static void Main() {
            Console.WriteLine($"Part 1: {Part1()}");
            Console.WriteLine("\n-----------------\n");
            Console.WriteLine($"Part 2: {Part2()}");
        }


        private static string Part1() {
            var grid = ReadInput();
            var score = FindShortestPath(grid, new[] {grid.exit});
            return $"{score}";
        }

        private static string Part2() {
            var grid = ReadInput();
            var score = FindShortestPath(grid, new[] {grid.exit, grid.entry, grid.exit});
            return $"{score}";
        }


        private static int FindShortestPath(Grid grid, IEnumerable<Vector2Int> goals) {
            var previousSolutions = new HashSet<Vector2Int> {grid.entry};
            var currentSolutions = previousSolutions;

            var timeElapsed = 0;
            foreach (var goal in goals) {
                while (!currentSolutions.Contains(goal)) {
                    timeElapsed++;
                    grid.SimulateBlizzard();
                    currentSolutions = new HashSet<Vector2Int>(previousSolutions.SelectMany(t => directions.Select(u => t + u)).Where(grid.IsEmpty));
                    previousSolutions = currentSolutions;
                }

                previousSolutions = new HashSet<Vector2Int> {goal};
                currentSolutions = previousSolutions;
            }

            return timeElapsed;
        }


        private static Grid ReadInput() {
            var input = File.ReadAllLines("input.txt").Where(t => !string.IsNullOrEmpty(t)).ToArray();
            return new Grid(
                (0, 1 - input.Length),
                (input[0].Length - 1, 0),
                (1, 0),
                (input[0].Length - 2, 1 - input.Length),
                input.SelectMany((t, y) => t.Select((c, x) => new Blizzard((x, -y), c switch {
                    '>' => Vector2Int.right, '<' => Vector2Int.left, 'v' => Vector2Int.down, '^' => Vector2Int.up, _ => Vector2Int.zero
                }))).Where(t => t.direction != Vector2Int.zero)
            );
        }


        private class Blizzard {
            public Vector2Int position { get; set; }
            public Vector2Int direction { get; }

            public Blizzard(Vector2Int position, Vector2Int direction) {
                this.position = position;
                this.direction = direction;
            }
        }

        private class Grid {
            private IReadOnlyList<Blizzard> blizzards { get; }
            private Vector2Int minBound { get; }
            private Vector2Int maxBound { get; }
            public Vector2Int entry { get; }
            public Vector2Int exit { get; }

            public Grid(Vector2Int minBound, Vector2Int maxBound, Vector2Int entry, Vector2Int exit, IEnumerable<Blizzard> blizzards) {
                this.blizzards = blizzards.ToList();
                this.minBound = minBound;
                this.maxBound = maxBound;
                this.entry = entry;
                this.exit = exit;
            }

            public void SimulateBlizzard() {
                foreach (var blizzard in blizzards) {
                    blizzard.position += blizzard.direction;
                    if (blizzard.position.x == minBound.x) blizzard.position = (maxBound.x - 1, blizzard.position.y);
                    if (blizzard.position.x == maxBound.x) blizzard.position = (minBound.x + 1, blizzard.position.y);
                    if (blizzard.position.y == minBound.y) blizzard.position = (blizzard.position.x, maxBound.y - 1);
                    if (blizzard.position.y == maxBound.y) blizzard.position = (blizzard.position.x, minBound.y + 1);
                }
            }

            public bool IsEmpty(Vector2Int position) {
                if (position == entry) return true;
                if (position == exit) return true;
                if (position.x <= minBound.x || position.x >= maxBound.x) return false;
                if (position.y <= minBound.y || position.y >= maxBound.y) return false;
                return blizzards.All(t => t.position != position);
            }
        }
    }
}