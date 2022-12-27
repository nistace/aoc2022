using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Day22;

namespace Day23 {
    internal static class Program {
        private static Dictionary<Vector2Int, Vector2Int[]> moveChecks { get; } = new Dictionary<Vector2Int, Vector2Int[]> {
            {Vector2Int.up, new[] {Vector2Int.up, Vector2Int.upLeft, Vector2Int.upRight}},
            {Vector2Int.left, new[] {Vector2Int.left, Vector2Int.upLeft, Vector2Int.downLeft}},
            {Vector2Int.right, new[] {Vector2Int.right, Vector2Int.upRight, Vector2Int.downRight}},
            {Vector2Int.down, new[] {Vector2Int.down, Vector2Int.downLeft, Vector2Int.downRight}}
        };


        private static void Main() {
            Console.WriteLine($"Part 1: {Part1()}");
            Console.WriteLine("\n-----------------\n");
            Console.WriteLine($"Part 2: {Part2()}");
        }


        private static string Part1() {
            var elvesPosition = ReadInput();
            var movePriorities = new Queue<Vector2Int>(new[] {Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right});

            for (var i = 0; i < 10; ++i) {
                var moves = Round1(elvesPosition, movePriorities, out _);
                elvesPosition = Round2(moves);
                movePriorities.Enqueue(movePriorities.Dequeue());
            }

            return $"{Display(elvesPosition).Count(t => t == '.')}{Environment.NewLine}{Display(elvesPosition)}";
        }


        private static string Part2() {
            var elvesPosition = ReadInput();
            var movePriorities = new Queue<Vector2Int>(new[] {Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right});

            var turn = 0;
            var keepMoving = true;
            while (keepMoving) {
                var moves = Round1(elvesPosition, movePriorities, out var wronglyPositionedCount);
                keepMoving = wronglyPositionedCount > 0;
                elvesPosition = Round2(moves);
                movePriorities.Enqueue(movePriorities.Dequeue());
                Console.WriteLine($"{turn}: {wronglyPositionedCount}");
                turn++;
            }

            return $"{turn}{Environment.NewLine}{Display(elvesPosition)}";
        }

        private static string Display(IReadOnlyList<Vector2Int> positions) {
            var minY = positions.Min(t => t.y);
            var maxY = positions.Max(t => t.y);
            var minX = positions.Min(t => t.x);
            var maxX = positions.Max(t => t.x);

            var result = new StringBuilder();

            for (var y = maxY; y >= minY; --y) {
                for (var x = minX; x <= maxX; ++x) {
                    result.Append(positions.Contains((x, y)) ? '#' : '.');
                }

                result.Append(Environment.NewLine);
            }

            return result.ToString();
        }

        private static IReadOnlyList<Vector2Int> Round2(IReadOnlyDictionary<Vector2Int, Vector2Int> moves) => moves.Select(t => moves.Values.Count(u => u == t.Value) > 1 ? t.Key : t.Value).ToArray();

        private static IReadOnlyDictionary<Vector2Int, Vector2Int> Round1(IReadOnlyCollection<Vector2Int> currentPositions, IReadOnlyCollection<Vector2Int> movePriorities, out int wronglyPositionedCount) {
            var result = new Dictionary<Vector2Int, Vector2Int>();
            wronglyPositionedCount = 0;
            foreach (var position in currentPositions) {
                if (!position.Get8Neighbours().Any(currentPositions.Contains)) {
                    result.Add(position, position);
                }
                else if (TryChooseMove(position, currentPositions, movePriorities, out var move)) {
                    result.Add(position, position + move);
                    wronglyPositionedCount++;
                }
                else {
                    result.Add(position, position);
                    wronglyPositionedCount++;
                }
            }

            return result;
        }

        private static bool TryChooseMove(Vector2Int position, IReadOnlyCollection<Vector2Int> allPositions, IEnumerable<Vector2Int> movePriorities, out Vector2Int move) {
            foreach (var moveOption in movePriorities) {
                if (moveChecks[moveOption].Any(t => allPositions.Contains(position + t))) continue;
                move = moveOption;
                return true;
            }

            move = default;
            return false;
        }

        private static IReadOnlyList<Vector2Int> ReadInput() => File.ReadLines("input.txt").SelectMany((t, y) => t.Select((c, x) => (c, x, y: -y))).Where(t => t.c == '#').Select(t => (Vector2Int) (t.x, t.y)).ToList();
    }
}