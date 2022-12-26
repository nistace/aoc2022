using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Day21 {
    internal static class Program {
        private enum Operation {
            None,
            Sum,
            Difference,
            Product,
            Division
        }

        private static void Main() {
            Console.WriteLine($"Part 1: {Part1()}");
            Console.WriteLine("\n-----------------\n");
            Console.WriteLine($"Part 2: {Part2()}");
        }


        private static string Part1() {
            var monkeys = ReadInput();
            return $"{monkeys["root"].GetValue(monkeys)}{Environment.NewLine}{monkeys["root"].ToString(0, monkeys).Substring(0, 300)}...";
        }

        private static string Part2() {
            const string humanName = "humn";
            const string rootName = "root";
            var monkeys = ReadInput();

            var reversedMonkeys = new Dictionary<string, IMonkey>();
            if (monkeys[rootName] is OperationMonkey rootMonkey) {
                if (monkeys[rootMonkey.left].IsOrInvolves(humanName, monkeys)) {
                    reversedMonkeys.Add(rootMonkey.left, new AliasMonkey(rootMonkey.left, rootMonkey.right));
                }
                else {
                    reversedMonkeys.Add(rootMonkey.right, new AliasMonkey(rootMonkey.right, rootMonkey.left));
                }
            }

            foreach (var monkey in monkeys.Values) {
                if (monkey.name == humanName || monkey.name == rootName) continue;
                if (!(monkey is OperationMonkey operationMonkey)) {
                    reversedMonkeys.Add(monkey.name, monkey);
                    continue;
                }

                if (monkeys[operationMonkey.left].IsOrInvolves(humanName, monkeys)) reversedMonkeys.Add(operationMonkey.left, operationMonkey.ReverseOnLeft());
                else if (monkeys[operationMonkey.right].IsOrInvolves(humanName, monkeys)) reversedMonkeys.Add(operationMonkey.right, operationMonkey.ReverseOnRight());
                else reversedMonkeys.Add(operationMonkey.name, operationMonkey);
            }


            return $"{reversedMonkeys[humanName].GetValue(reversedMonkeys)}{Environment.NewLine}{reversedMonkeys[humanName].ToString(0, reversedMonkeys).Substring(0, 300)}...";
        }

        private static IReadOnlyDictionary<string, IMonkey> ReadInput() {
            var lines = File.ReadLines("input.txt").ToArray();
            return lines.Select(t => Regex.Match(t, "([a-z]+): (\\d+)")).Where(t => t.Success)
                .Select(t => (IMonkey) new ValueMonkey(t.Groups[1].Value, long.Parse(t.Groups[2].Value)))
                .Union(lines.Select(t => Regex.Match(t, "([a-z]+): ([a-z]+) ([\\+\\-\\*\\/]) ([a-z]+)")).Where(t => t.Success)
                    .Select(t => (IMonkey) new OperationMonkey(t.Groups[1].Value, t.Groups[2].Value, t.Groups[4].Value,
                        t.Groups[3].Value switch {"+" => Operation.Sum, "-" => Operation.Difference, "*" => Operation.Product, "/" => Operation.Division, _ => Operation.None}
                    ))
                ).ToDictionary(t => t.name, t => t);
        }


        private interface IMonkey {
            string name { get; }
            long GetValue(IReadOnlyDictionary<string, IMonkey> monkeys);
            bool IsOrInvolves(string monkeyName, IReadOnlyDictionary<string, IMonkey> monkeys);
            string ToString(int indent, IReadOnlyDictionary<string, IMonkey> monkeys);
        }

        private class ValueMonkey : IMonkey {
            public string name { get; }
            private long value { get; }

            public ValueMonkey(string name, long value) {
                this.name = name;
                this.value = value;
            }

            public long GetValue(IReadOnlyDictionary<string, IMonkey> monkeys) => value;
            public bool IsOrInvolves(string monkeyName, IReadOnlyDictionary<string, IMonkey> monkeys) => name == monkeyName;

            public string ToString(int indent, IReadOnlyDictionary<string, IMonkey> monkeys) => $"{new string(' ', indent * 2)} > {name} = {value} ({GetValue(monkeys)})";
        }

        private class OperationMonkey : IMonkey {
            public string name { get; }
            public string left { get; }
            public string right { get; }
            private Operation operation { get; }

            public OperationMonkey(string name, string left, string right, Operation operation) {
                this.name = name;
                this.left = left;
                this.right = right;
                this.operation = operation;
            }

            public long GetValue(IReadOnlyDictionary<string, IMonkey> monkeys) {
                return operation switch {
                    Operation.Sum => (monkeys[left].GetValue(monkeys) + monkeys[right].GetValue(monkeys)),
                    Operation.Difference => (monkeys[left].GetValue(monkeys) - monkeys[right].GetValue(monkeys)),
                    Operation.Product => (monkeys[left].GetValue(monkeys) * monkeys[right].GetValue(monkeys)),
                    Operation.Division => (monkeys[left].GetValue(monkeys) / monkeys[right].GetValue(monkeys)),
                    _ => throw new ArgumentOutOfRangeException()
                };
            }

            public bool IsOrInvolves(string monkeyName, IReadOnlyDictionary<string, IMonkey> monkeys) {
                return name == monkeyName || monkeys[left].IsOrInvolves(monkeyName, monkeys) || monkeys[right].IsOrInvolves(monkeyName, monkeys);
            }

            public IMonkey ReverseOnLeft() {
                return new OperationMonkey(left, name, right,
                    operation switch {
                        Operation.Sum => Operation.Difference,
                        Operation.Difference => Operation.Sum,
                        Operation.Product => Operation.Division,
                        Operation.Division => Operation.Product,
                        _ => Operation.None
                    });
            }


            public IMonkey ReverseOnRight() {
                return operation switch {
                    Operation.Sum => new OperationMonkey(right, name, left, Operation.Difference),
                    Operation.Difference => new OperationMonkey(right, left, name, Operation.Difference),
                    Operation.Product => new OperationMonkey(right, name, left, Operation.Division),
                    Operation.Division => new OperationMonkey(right, left, name, Operation.Division),
                    _ => new OperationMonkey(right, left, name, Operation.None),
                };
            }

            public string ToString(int indent, IReadOnlyDictionary<string, IMonkey> monkeys) {
                var builder = new StringBuilder();
                for (var i = 0; i < indent; ++i) builder.Append("  ");
                builder.Append($" > {name} = {left} {operation switch {Operation.Sum => "+", Operation.Difference => "-", Operation.Product => "*", Operation.Division => "/", _ => "?"}} {right} ({GetValue(monkeys)})");
                builder.Append(Environment.NewLine);
                builder.Append(monkeys[left].ToString(indent + 1, monkeys));
                builder.Append(Environment.NewLine);
                builder.Append(monkeys[right].ToString(indent + 1, monkeys));
                return builder.ToString();
            }
        }

        private class AliasMonkey : IMonkey {
            public string name { get; }
            private string other { get; }

            public AliasMonkey(string name, string other) {
                this.name = name;
                this.other = other;
            }

            public long GetValue(IReadOnlyDictionary<string, IMonkey> monkeys) => monkeys[other].GetValue(monkeys);
            public bool IsOrInvolves(string monkeyName, IReadOnlyDictionary<string, IMonkey> monkeys) => name == monkeyName || monkeys[other].IsOrInvolves(monkeyName, monkeys);

            public string ToString(int indent, IReadOnlyDictionary<string, IMonkey> monkeys) {
                var builder = new StringBuilder();
                for (var i = 0; i < indent; ++i) builder.Append("  ");
                builder.Append($" > {name} = {other} ({GetValue(monkeys)})");
                builder.Append(Environment.NewLine);
                builder.Append(monkeys[other].ToString(indent + 1, monkeys));
                return builder.ToString();
            }
        }
    }
}