using System;
using System.IO;
using System.Linq;

namespace Day2 {
	internal static class Program {
		private static void Main() {
			Console.WriteLine($"Part 1: {Part1()}");
			Console.WriteLine($"Part 2: {Part2()}");
		}

		private static string Part1() {
			var plays = File.ReadAllText("input.txt").Trim().Split(Environment.NewLine).Select(t => (t[0] - 'A', t[2] - 'X')).ToArray();
			var pointsForFigurePlayed = plays.Sum(t => t.Item2 + 1);
			var pointsForVictories = plays.Count(t => t.Item2 == (t.Item1 + 1) % 3) * 6;
			var pointsForDraws = plays.Count(t => t.Item2 == t.Item1) * 3;
			var points = pointsForFigurePlayed + pointsForVictories + pointsForDraws;
			return $"{points:0}: {pointsForFigurePlayed} (figures), {pointsForVictories} (victories), {pointsForDraws} (draws)";
		}

		private static string Part2() {
			var plays = File.ReadAllText("input.txt").Trim().Split(Environment.NewLine).Select(t => (t[0] - 'A', t[2] - 'X' + 2)).ToArray();
			var pointsForFigurePlayed = plays.Sum(t => (t.Item1 + t.Item2) % 3 + 1);
			var pointsForVictories = plays.Count(t => t.Item2 == 4) * 6;
			var pointsForDraws = plays.Count(t => t.Item2 == 3) * 3;
			var points = pointsForFigurePlayed + pointsForVictories + pointsForDraws;
			return $"{points:0}: {pointsForFigurePlayed} (figures), {pointsForVictories} (victories), {pointsForDraws} (draws)";
		}
	}
}