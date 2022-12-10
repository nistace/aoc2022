using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Day10 {
    internal static class Program {
        private static void Main() {
            Console.WriteLine($"Part 1: {Part1()}");
            Console.WriteLine("\n-----------------\n");
            Console.WriteLine($"Part 2: {Part2()}");
        }

        private static string Part1() {
            var interestingFrames = new[] {20, 60, 100, 140, 180, 220};
            var frames = ReadFrames();
            return $"{interestingFrames.Select(t => t * frames[t - 1]).Sum()}{Environment.NewLine}(" + frames.Count + $" frames){Environment.NewLine}{string.Join(Environment.NewLine, interestingFrames.Select(t => $"{t}\t{t * frames[t - 1]}"))}";
        }

        private static string Part2() {
            var frames = ReadFrames();
            return string.Join("", frames.Select((t, i) => (sprite: t, pixel: i % 40 - 1)).Select(t => $"{(t.pixel == -1 ? Environment.NewLine : string.Empty)}{(t.sprite >= t.pixel && t.sprite < t.pixel + 3 ? "#" : " ")}"));
        }

        private static List<int> ReadFrames() {
            var frames = new List<int> {1};
            foreach (var instruction in File.ReadAllLines("input.txt").Where(t => t == "noop" || t.StartsWith("addx "))) {
                frames.Add(frames.Last());
                if (instruction.StartsWith("addx ")) frames.Add(frames.Last() + int.Parse(instruction.Substring("addx ".Length)));
            }

            return frames;
        }
    }
}