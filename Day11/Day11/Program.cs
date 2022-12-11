using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Day11 {
    internal static class Program {
        private static void Main() {
            Console.WriteLine($"Part 1: {Part1()}");
            Console.WriteLine("\n-----------------\n");
            Console.WriteLine($"Part 2: {Part2()}");
        }

        private static string Part1() {
            var monkeys = ReadInput();
            for (var round = 0; round < 20; ++round) {
                foreach (var monkeyTurn in monkeys) {
                    while (monkeyTurn.InspectAndThrowNext(true, out var targetMonkeyIndex, out var itemWorryLevel)) {
                        monkeys[targetMonkeyIndex].Catch(itemWorryLevel);
                    }
                }
            }

            return $"{monkeys.OrderByDescending(t => t.inspectedItemCount).Take(2).Aggregate(1L, (t, monkey) => t * monkey.inspectedItemCount)}{Environment.NewLine}{string.Join("\n", monkeys)}";
        }

        private static string Part2() {
            var monkeys = ReadInput();

            for (var round = 0; round < 10000; ++round) {
                foreach (var monkeyTurn in monkeys) {
                    while (monkeyTurn.InspectAndThrowNext(false, out var targetMonkeyIndex, out var itemWorryLevel)) {
                        monkeys[targetMonkeyIndex].Catch(itemWorryLevel);
                    }
                }
            }

            return $"{monkeys.OrderByDescending(t => t.inspectedItemCount).Take(2).Aggregate(1L, (t, monkey) => t * monkey.inspectedItemCount)}{Environment.NewLine}{string.Join("\n", monkeys)}";
        }


        private static List<Monkey> ReadInput() {
            return File.ReadAllText("input.txt").Replace(Environment.NewLine, "").Split("Monkey")
                .Select(t => Regex.Match(t, ".*Starting.*:([\\d, ]+)Operation:.*(new = [old \\+\\*\\d]+).*Test.*by *(\\d+).*true.*monkey (\\d+).*false.*monkey (\\d+)"))
                .Where(t => t.Success)
                .Select(monkeyInputMatch => new Monkey(
                    monkeyInputMatch.Groups[1].Value.Split(",").Select(t => new WorryModSet(int.Parse(t.Trim()), 25)),
                    ParseOperation(monkeyInputMatch.Groups[2].Value),
                    int.Parse(monkeyInputMatch.Groups[3].Value),
                    int.Parse(monkeyInputMatch.Groups[4].Value),
                    int.Parse(monkeyInputMatch.Groups[5].Value))
                ).ToList();
        }

        private static Func<long, long> ParseOperation(string operation) {
            if (operation.Trim() == "new = old * old") return t => t * t;
            if (operation.Trim().StartsWith("new = old * ")) return t => t * int.Parse(operation.Trim().Substring("new = old * ".Length).Trim());
            if (operation.Trim().StartsWith("new = old + ")) return t => t + int.Parse(operation.Trim().Substring("new = old + ".Length).Trim());
            Console.WriteLine("!!! Operation not understood: " + operation);
            return t => t;
        }

        private class Monkey {
            private List<WorryModSet> items { get; } = new List<WorryModSet>();
            private Func<long, long> operation { get; }
            private int divisibleBy { get; }
            private int targetMonkeyWhenTrue { get; }
            private int targetMonkeyWhenFalse { get; }

            public long inspectedItemCount { get; private set; }

            public Monkey(IEnumerable<WorryModSet> startingItems, Func<long, long> operation, int divisibleBy, int targetMonkeyWhenTrue, int targetMonkeyWhenFalse) {
                items.AddRange(startingItems);
                this.operation = operation;
                this.divisibleBy = divisibleBy;
                this.targetMonkeyWhenTrue = targetMonkeyWhenTrue;
                this.targetMonkeyWhenFalse = targetMonkeyWhenFalse;
            }


            public override string ToString() => $"[{string.Join(", ", items)}], (3 => {operation(3)}, 9 => {operation(9)}), %{divisibleBy} ? {targetMonkeyWhenTrue} : {targetMonkeyWhenFalse}, inspected {inspectedItemCount}";

            public bool InspectAndThrowNext(bool withRelief, out int targetMonkey, out WorryModSet item) {
                targetMonkey = default;
                item = default;
                if (items.Count == 0) return false;
                item = items[0];
                item.DoOperation(operation);
                if (withRelief) item.DivideBy3();
                targetMonkey = item.IsDivisibleBy(divisibleBy) ? targetMonkeyWhenTrue : targetMonkeyWhenFalse;
                items.RemoveAt(0);
                inspectedItemCount++;
                return true;
            }

            public void Catch(WorryModSet item) => items.Add(item);
        }

        private class WorryModSet {
            private long number { get; set; }
            private long[] modSet { get; }

            public WorryModSet(long number, int modsCount) {
                modSet = Enumerable.Repeat(0L, modsCount).ToArray();
                DoOperation(t => t + number);
            }

            public void DoOperation(Func<long, long> operation) {
                number = operation(number);

                for (var i = 0; i < modSet.Length; ++i) {
                    modSet[i] = operation(modSet[i]) % (i + 1);
                }
            }

            public void DivideBy3() {
                number /= 3;

                for (var i = 0; i < modSet.Length; ++i) {
                    modSet[i] = number % (i + 1);
                }
            }

            public bool IsDivisibleBy(int amount) => modSet[amount - 1] == 0;

            public override string ToString() {
                return $"~{modSet.Select((t, i) => (mod: t, num: i + 1)).Where(t => t.mod == 0).Aggregate(1, (a, b) => a * b.num)} ({string.Join(", ", modSet.Select((t, i) => (mod: t, num: i + 1)).Where(t => t.mod == 0).Select(t => t.num))})";
            }
        }
    }
}