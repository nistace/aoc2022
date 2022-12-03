using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Day1 {
	public static class Program {
		private static void Main() {
			Console.WriteLine($"Part 1: {Part1()}");
			Console.WriteLine($"Part 2: {Part2()}");
		}

		private static string Part1() {
			var totalPerElf = GetTotalPerElf();
			var max = totalPerElf.Max();
			return $"{max} (elf {totalPerElf.FindIndex(t => t == max) + 1:0})";
		}

		private static string Part2() {
			var totalPerElf = GetTotalPerElf();
			var orderedTotalsDesc = totalPerElf.OrderByDescending(t => t).ToList();
			var sumFirstThree = orderedTotalsDesc.Take(3).Sum();
			return $"{sumFirstThree} (elves {totalPerElf.FindIndex(t => t == orderedTotalsDesc[0]) + 1:0}, "
					+ $"{totalPerElf.FindIndex(t => t == orderedTotalsDesc[1]) + 1:0}, and {totalPerElf.FindIndex(t => t == orderedTotalsDesc[2]) + 1:0})";
		}

		private static List<int> GetTotalPerElf() {
			var input = File.ReadAllText("input.txt").Trim();
			var mealsPerElf = input.Split($"{Environment.NewLine}{Environment.NewLine}").ToArray();
			return mealsPerElf.Select(t => t.Split(Environment.NewLine).Select(int.Parse).Sum()).ToList();
		}
	}
}