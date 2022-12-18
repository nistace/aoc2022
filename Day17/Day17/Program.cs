using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Day17 {
    internal static class Program {
        private static void Main() {
            Console.WriteLine($"Part 1: {Part1()}");
            Console.WriteLine("\n-----------------\n");
            Console.WriteLine($"Part 2: {Part2()}");
        }

        private static IReadOnlyList<RockShape> rockShapes { get; } = new[] {
            new RockShape(new[] {Vector2Int.zero, Vector2Int.right, 2 * Vector2Int.right, 3 * Vector2Int.right}),
            new RockShape(new[] {Vector2Int.right, Vector2Int.up, Vector2Int.upRight, Vector2Int.upRight + Vector2Int.right, Vector2Int.upRight + Vector2Int.up}),
            new RockShape(new[] {Vector2Int.zero, Vector2Int.right, 2 * Vector2Int.right, 2 * Vector2Int.right + Vector2Int.up, 2 * Vector2Int.upRight}),
            new RockShape(new[] {Vector2Int.zero, Vector2Int.up, 2 * Vector2Int.up, 3 * Vector2Int.up}),
            new RockShape(new[] {Vector2Int.zero, Vector2Int.up, Vector2Int.right, Vector2Int.upRight})
        };


        private static string Part1() {
            var gasDirections = ReadInput().ToArray();

            var height = -1;
            var rockPositions = new HashSet<Vector2Int>();
            var gasDirectionIndex = 0;

            for (var rockIndex = 0; rockIndex < 2022; rockIndex++) {
                height = SimulateRock(gasDirections, height, rockShapes[rockIndex % rockShapes.Count], rockPositions, ref gasDirectionIndex);
            }

            return $"{height + 1}{Environment.NewLine}{Display(rockPositions, 20)}";
        }


        private static string Part2() {
            var gasDirections = ReadInput().ToArray();
            var (repeatingSequenceLength, repeatingSequenceStartIndex, repeatingSequenceGainedHeight) = FindRepeatingSequence(gasDirections);

            var height = -1;
            var rockPositions = new HashSet<Vector2Int>();
            var gasDirectionIndex = 0;

            var outsideOfRepeatingSequence = 1000000000000L % repeatingSequenceLength;
            var repeatingSequenceCount = 1000000000000L / repeatingSequenceLength;
            var heightGainedByRepeatingSequence = repeatingSequenceCount * repeatingSequenceGainedHeight;

            // Simulate before repeating sequence
            for (var rockIndex = 0; rockIndex < outsideOfRepeatingSequence; rockIndex++) {
                height = SimulateRock(gasDirections, height, rockShapes[rockIndex % rockShapes.Count], rockPositions, ref gasDirectionIndex);
            }

            return $"{height + heightGainedByRepeatingSequence + 1}"
                   + $"{Environment.NewLine}height: {height}"
                   + $"{Environment.NewLine}repeatingSequenceLength: {repeatingSequenceLength}"
                   + $"{Environment.NewLine}repeatingSequenceStartIndex: {repeatingSequenceStartIndex}"
                   + $"{Environment.NewLine}repeatingSequenceGainedHeight: {repeatingSequenceGainedHeight}"
                   + $"{Environment.NewLine}outsideOfRepeatingSequence: {outsideOfRepeatingSequence}"
                   + $"{Environment.NewLine}repeatingSequenceCount: {repeatingSequenceCount}"
                   + $"{Environment.NewLine}heightGainedByRepeatingSequence: {heightGainedByRepeatingSequence}";
        }

        private static (int length, int startIndex, int gainedHeight) FindRepeatingSequence(IReadOnlyList<Vector2Int> gasDirections) {
            // Keeps track of the index of the next gas direction when starting a new sequence of rock shapes.
            var sequence = new List<int>();
            var heights = new Dictionary<int, int>();
            var currentHeight = -1;
            var rockPositions = new HashSet<Vector2Int>();

            var rockIndex = 0;
            var gasDirectionIndex = 0;

            while (true) {
                if (rockIndex == 0) {
                    heights.Add(sequence.Count, currentHeight);
                    sequence.Add(gasDirectionIndex);
                    if (TryFindRepeatingSequence(sequence, out var sequenceLength, out var startIndex)) {
                        return (sequenceLength * rockShapes.Count, startIndex * rockShapes.Count, heights[startIndex + sequenceLength] - heights[startIndex]);
                    }
                }

                currentHeight = SimulateRock(gasDirections, currentHeight, rockShapes[rockIndex], rockPositions, ref gasDirectionIndex);
                rockIndex = (rockIndex + 1) % rockShapes.Count;
            }
        }

        private static int SimulateRock(IReadOnlyList<Vector2Int> gasDirections, int currentHeight, RockShape rock, HashSet<Vector2Int> rockPositions, ref int gasDirectionIndex) {
            var rockPosition = new Vector2Int(2, currentHeight + 4);
            var resting = false;
            while (!resting) {
                var blowDirection = gasDirections[gasDirectionIndex];
                gasDirectionIndex = (gasDirectionIndex + 1) % gasDirections.Count;
                if (rock.TryMoveTo(rockPosition + blowDirection, rockPositions, 0)) rockPosition += blowDirection;
                if (rock.TryMoveTo(rockPosition + Vector2Int.down, rockPositions, 0)) rockPosition += Vector2Int.down;
                else resting = true;
            }

            var restingRockWorldPositions = rock.ToWorldPositions(rockPosition).ToArray();
            currentHeight = Math.Max(currentHeight, restingRockWorldPositions.Max(t => t.y));
            foreach (var position in restingRockWorldPositions) rockPositions.Add(position);
            return currentHeight;
        }

        private static bool TryFindRepeatingSequence(IReadOnlyCollection<int> sequence, out int repeatingSequenceLength, out int repeatingSequenceStartIndex) {
            // If we find a repeating sequence, then the sequence must be like ABB, where A contains a number of elements so that, when removed from the sequence, an even number of elements remain.
            for (repeatingSequenceStartIndex = sequence.Count % 2; repeatingSequenceStartIndex < sequence.Count; repeatingSequenceStartIndex += 2) {
                repeatingSequenceLength = (sequence.Count - repeatingSequenceStartIndex) / 2;
                var sub1 = sequence.Skip(repeatingSequenceStartIndex).Take(repeatingSequenceLength).ToArray();
                var sub2 = sequence.Skip(repeatingSequenceStartIndex + repeatingSequenceLength).ToArray();
                if (sub1.SequenceEqual(sub2)) return true;
            }

            repeatingSequenceLength = default;
            return false;
        }

        private static string Display(IReadOnlySet<Vector2Int> rockPositions, int maxY) {
            var builder = new StringBuilder();
            for (var y = maxY; y >= 0; --y) {
                builder.Append('|');
                for (var x = 0; x < 7; ++x) {
                    builder.Append(rockPositions.Contains((x, y)) ? '#' : '.');
                }

                builder.Append('|');

                builder.Append(Environment.NewLine);
            }

            builder.Append("+-------+");
            return builder.ToString();
        }


        private static IEnumerable<Vector2Int> ReadInput() =>
            new Queue<Vector2Int>(File.ReadAllText("input.txt")
                .Where(t => t == '<' || t == '>')
                .Select(t => t switch {'<' => Vector2Int.left, '>' => Vector2Int.right, _ => Vector2Int.zero})
            );


        private class RockShape {
            // All positions are relative to the bottom left corner of the shape
            private IReadOnlyList<Vector2Int> localPositions { get; }

            public RockShape(IEnumerable<Vector2Int> rockPositions) {
                localPositions = rockPositions.ToList();
            }

            public bool TryMoveTo(Vector2Int bottomLeftWorldPosition, IReadOnlySet<Vector2Int> forbiddenPositions, long minY) {
                foreach (var worldPosition in ToWorldPositions(bottomLeftWorldPosition)) {
                    if (worldPosition.x < 0) return false;
                    if (worldPosition.x > 6) return false;
                    if (worldPosition.y < minY) return false;
                    if (forbiddenPositions.Contains(worldPosition)) return false;
                }

                return true;
            }

            public IEnumerable<Vector2Int> ToWorldPositions(Vector2Int bottomLeftWorldPosition) => localPositions.Select(t => t + bottomLeftWorldPosition);
        }
    }
}