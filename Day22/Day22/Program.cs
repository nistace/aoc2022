using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Day22 {
    internal static class Program {
        private enum Cell {
            Nothing,
            Path,
            Obstacle
        }

        private static Dictionary<Vector2Int, int> scorePerDirection { get; } = new Dictionary<Vector2Int, int> {{Vector2Int.right, 0}, {Vector2Int.down, 1}, {Vector2Int.left, 2}, {Vector2Int.up, 3}};
        private static Dictionary<Vector2Int, Vector2Int> rightRotations { get; } = new Dictionary<Vector2Int, Vector2Int> {{Vector2Int.right, Vector2Int.down}, {Vector2Int.down, Vector2Int.left}, {Vector2Int.left, Vector2Int.up}, {Vector2Int.up, Vector2Int.right}};
        private static Dictionary<Vector2Int, Vector2Int> leftRotations { get; } = new Dictionary<Vector2Int, Vector2Int> {{Vector2Int.right, Vector2Int.up}, {Vector2Int.up, Vector2Int.left}, {Vector2Int.left, Vector2Int.down}, {Vector2Int.down, Vector2Int.right}};


        private static void Main() {
            Console.WriteLine($"Part 1: {Part1()}");
            Console.WriteLine("\n-----------------\n");
            Console.WriteLine($"Part 2: {Part2()}");
        }


        private static string Part1() {
            ReadInput(out var grid, out var stepsForwards, out var directions);
            var position = grid.origin;
            for (var i = 0; i < stepsForwards.Count; ++i) {
                position = grid.Move(position, directions[i], stepsForwards[i]);
            }

            return $"{1000 * (-position.y + 1) + 4 * (position.x + 1) + scorePerDirection[directions[^1]]}";
        }

        private static string Part2() {
            return "";
        }

        private static void ReadInput(out Grid grid, out IReadOnlyList<int> stepsForwards, out IReadOnlyList<Vector2Int> directions) {
            grid = new Grid();
            var gridInputLines = File.ReadAllText("input.txt").Split(Environment.NewLine + "" + Environment.NewLine)[0].Split(Environment.NewLine);
            foreach (var (content, x, y) in gridInputLines.SelectMany((t, y) => t.Select((u, x) => (content: u, x, y)).ToArray())) {
                grid[(x, -y)] = content switch {
                    ' ' => Cell.Nothing,
                    '#' => Cell.Obstacle,
                    '.' => Cell.Path,
                    _ => Cell.Nothing
                };
            }

            var instructionInput = File.ReadAllText("input.txt").Trim().Split(Environment.NewLine + "" + Environment.NewLine)[1];
            stepsForwards = Regex.Matches(instructionInput, "(\\d+)").Select(t => int.Parse(t.Groups[1].Value)).ToArray();
            directions = Regex.Matches(instructionInput, "([RL])").Aggregate(new List<Vector2Int> {Vector2Int.right}, (t, u) => {
                t.Add(u.Groups[1].Value == "R" ? rightRotations[t.Last()] : leftRotations[t.Last()]);
                return t;
            });
        }


        private class Grid {
            private Dictionary<Vector2Int, Cell> cells { get; } = new Dictionary<Vector2Int, Cell>();


            public Cell this[Vector2Int coordinates] {
                get => cells.ContainsKey(coordinates) ? cells[coordinates] : Cell.Nothing;
                set => cells[coordinates] = value;
            }

            public Vector2Int origin => cells.Where(t => t.Key.y == 0 && t.Value == Cell.Path).OrderBy(t => t.Key.x).First().Key;

            public Vector2Int Move(Vector2Int from, Vector2Int direction, int times) {
                var position = from;
                for (var i = 0; i < times; ++i) {
                    switch (this[position + direction]) {
                        case Cell.Obstacle:
                            return position;
                        case Cell.Nothing: {
                            if (!TryLoop(from, direction, out var newPosition)) return position;
                            position = newPosition;
                            break;
                        }
                        default:
                            position += direction;
                            break;
                    }
                }

                return position;
            }

            private bool TryLoop(in Vector2Int position, Vector2Int movingDirection, out Vector2Int otherBound) {
                otherBound = position;
                while (this[otherBound - movingDirection] != Cell.Nothing) otherBound -= movingDirection;
                return this[otherBound] == Cell.Path;
            }
        }
    }
}