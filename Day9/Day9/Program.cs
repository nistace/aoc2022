using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Day9
{
    internal static class Program
    {
        private static void Main()
        {
            Console.WriteLine($"Part 1: {Part(2)}");
            Console.WriteLine("\n-----------------\n");
            Console.WriteLine($"Part 2: {Part2(10)}");
        }

        private static string Part(int knotCount)
        {
            var knots = Enumerable.Repeat(Vector2Int.zero, knotCount).ToArray();
            var visited = new HashSet<Vector2Int> {knots.Last()};
            foreach (var move in ReadInput()) visited.Add(DoMoveAndGetLast(ref knots, move));
            return $"{visited.Count}{Environment.NewLine}{string.Join(" ", visited.Take(30))}";
        }


        private static string Part2()
        {
            var allMoves = ReadInput();
            var knots = Enumerable.Repeat(Vector2Int.zero, 10).ToArray();
            var visited = new HashSet<Vector2Int> {knots.Last()};
            foreach (var move in allMoves) visited.Add(DoMoveAndGetLast(ref knots, move));
            return $"{visited.Count}{Environment.NewLine}{string.Join(" ", visited.Take(30))}";
        }

        private static Vector2Int DoMoveAndGetLast(ref Vector2Int[] knots, Vector2Int move)
        {
            knots[0] += move;
            for (var i = 1; i < knots.Length; ++i)
            {
                var current = knots[i];
                var previous = knots[i - 1];
                knots[i] += previous.GridDistance(current) switch
                {
                    0 => Vector2Int.zero,
                    1 => Vector2Int.zero,
                    2 when previous.x == current.x || previous.y == current.y => (previous - current).clampedMinusOneOne,
                    2 => Vector2Int.zero,
                    _ => (previous - current).clampedMinusOneOne
                };
            }

            return knots.Last();
        }

        private static IEnumerable<Vector2Int> ReadInput()
        {
            return File.ReadAllLines("input.txt").Select(t => Regex.Match(t, "(R|L|U|D) *(\\d+)")).Where(t => t.Success)
                .Select(t => (move: t.Groups[1].Value switch {"R" => (1, 0), "L" => (-1, 0), "U" => (0, 1), "D" => (0, -1), _ => (0, 0)}, times: int.Parse(t.Groups[2].Value))).SelectMany(t => Enumerable.Repeat((Vector2Int) t.move, t.times))
                .ToArray();
        }


        private struct Vector2Int
        {
            public static Vector2Int zero => new Vector2Int(0, 0);

            public int x { get; }
            public int y { get; }


            public Vector2Int clampedMinusOneOne => new Vector2Int(Math.Clamp(x, -1, 1), Math.Clamp(y, -1, 1));

            private Vector2Int(int x, int y)
            {
                this.x = x;
                this.y = y;
            }


            public int GridDistance(Vector2Int other) => Math.Abs(other.x - x) + Math.Abs(other.y - y);

            public static implicit operator Vector2Int((int x, int y) d) => new Vector2Int(d.x, d.y);
            public static Vector2Int operator +(Vector2Int left, Vector2Int right) => new Vector2Int(left.x + right.x, left.y + right.y);
            public static Vector2Int operator -(Vector2Int left, Vector2Int right) => new Vector2Int(left.x - right.x, left.y - right.y);
            public static bool operator ==(Vector2Int left, Vector2Int right) => left.x == right.x && left.y == right.y;
            public static bool operator !=(Vector2Int left, Vector2Int right) => !(left == right);
            private bool Equals(Vector2Int other) => x == other.x && y == other.y;
            public override bool Equals(object obj) => obj is Vector2Int other && Equals(other);
            public override int GetHashCode() => (x * 397) ^ y;
            public override string ToString() => $"({x:000},{y:000})";
        }
    }
}