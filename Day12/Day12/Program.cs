using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Day12 {
    internal static class Program {
        private static void Main() {
            Console.WriteLine($"Part 1: {Part1()}");
            Console.WriteLine("\n-----------------\n");
            Console.WriteLine($"Part 2: {Part2()}");
        }

        private static string Part1() {
            var heightMap = ReadInput(out var start, out var end);
            var weights = FindPaths(new[] {start}, heightMap, end);
            return $"{weights[end]}";
        }

        private static string Part2() {
            var heightMap = ReadInput(out _, out var end);
            var weights = FindPaths(heightMap.Where(t => t.Value == 0).Select(t => t.Key), heightMap, end);
            return $"{weights[end]}";
        }

        private static Dictionary<Vector2Int, int> FindPaths(IEnumerable<Vector2Int> startPositions, IReadOnlyDictionary<Vector2Int, int> heightMap, Vector2Int end) {
            var directions = new[] {Vector2Int.up, Vector2Int.down, Vector2Int.right, Vector2Int.left};
            var open = startPositions.ToList();
            var closed = new List<Vector2Int>();
            var weights = open.ToDictionary(t => t, t => 0);
            while (open.Count > 0 && !closed.Contains(end)) {
                var node = open[0];
                open.RemoveAt(0);
                closed.Add(node);

                foreach (var openedNode in directions.Select(t => node + t)
                    .Where(t => heightMap.ContainsKey(t) && heightMap[t] <= heightMap[node] + 1)
                    .Where(t => !closed.Contains(t))
                    .Where(t => !weights.ContainsKey(t) || weights[t] > weights[node] + 1)) {
                    open.Remove(openedNode);
                    if (weights.ContainsKey(openedNode)) weights.Remove(openedNode);
                    weights.Add(openedNode, weights[node] + 1);
                    open.Insert(open.TakeWhile(t => weights[t] < weights[openedNode]).Count(), openedNode);
                }
            }

            return weights;
        }


        private static Dictionary<Vector2Int, int> ReadInput(out Vector2Int startPosition, out Vector2Int endPosition) {
            var inputLines = File.ReadAllLines("input.txt");

            startPosition = inputLines.Select((t, i) => (t, i)).Where(t => t.t.Contains('S')).Select(t => (t.t.IndexOf('S'), t.i)).First();
            endPosition = inputLines.Select((t, i) => (t, i)).Where(t => t.t.Contains('E')).Select(t => (t.t.IndexOf('E'), t.i)).First();

            return File.ReadAllLines("input.txt")
                .SelectMany((line, y) => line.Select((col, x) => (h: col switch {'S' => 'a', 'E' => 'z', _ => col} - 'a', x, y)))
                .ToDictionary(t => (Vector2Int) (t.x, t.y), t => t.h);
        }


        private struct Vector2Int {
            public static Vector2Int up { get; } = new Vector2Int(0, 1);
            public static Vector2Int down { get; } = new Vector2Int(0, -1);
            public static Vector2Int right { get; } = new Vector2Int(1, 0);
            public static Vector2Int left { get; } = new Vector2Int(-1, 0);

            public int x { get; }
            public int y { get; }


            private Vector2Int(int x, int y) {
                this.x = x;
                this.y = y;
            }


            public static implicit operator Vector2Int((int x, int y) d) => new Vector2Int(d.x, d.y);
            public static Vector2Int operator +(Vector2Int first, Vector2Int second) => new Vector2Int(first.x + second.x, first.y + second.y);
            public static Vector2Int operator -(Vector2Int first, Vector2Int second) => new Vector2Int(first.x - second.x, first.y - second.y);
            public static bool operator ==(Vector2Int first, Vector2Int second) => first.x == second.x && first.y == second.y;
            public static bool operator !=(Vector2Int first, Vector2Int second) => !(first == second);
            private bool Equals(Vector2Int other) => x == other.x && y == other.y;
            public override bool Equals(object obj) => obj is Vector2Int other && Equals(other);
            public override int GetHashCode() => (x * 397) ^ y;
            public override string ToString() => $"({x:000},{y:000})";
        }
    }
}