using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Day13 {
    internal static class Program {
        private static void Main() {
            Console.WriteLine($"Part 1: {Part1()}");
            Console.WriteLine("\n-----------------\n");
            Console.WriteLine($"Part 2: {Part2()}");
        }

        private static string Part1() {
            var packagePairs = ReadInput().ToArray();
            return $"{packagePairs.Select((t, i) => (packagePair: t, index: i + 1)).Where(t => t.packagePair.left.CompareTo(t.packagePair.right) < 0).Sum(t => t.index)}" +
                   $"{Environment.NewLine}{string.Join("\n", packagePairs.Select(t => $"{t.left} {t.left.CompareTo(t.right) switch {-1 => "<<<", 0 => "===", 1 => ">>>", _ => "ERR"}} {t.right}"))}";
        }


        private static string Part2() {
            var dividers = new[] {new PackageList(new PackageList(2)), new PackageList(new PackageList(6))};
            var packages = ReadInput().SelectMany(t => new[] {t.left, t.right}).Union(dividers);
            var orderedPackages = packages.ToList();
            orderedPackages.Sort((t, u) => t.CompareTo(u));
            return $"{dividers.Select(t => orderedPackages.IndexOf(t) + 1).Aggregate(1, (x, y) => x * y)}\n{string.Join("\n", orderedPackages)}";
        }

        private static IEnumerable<(IPackageNode left, IPackageNode right)> ReadInput() =>
            File.ReadAllText("input.txt").Trim().Split($"{Environment.NewLine}{Environment.NewLine}")
                .Select(t => (PackageList.Parse(t.Split(Environment.NewLine)[0]), PackageList.Parse(t.Split(Environment.NewLine)[1])));


        private interface IPackageNode {
            /// <summary>-1 if this is before, 0 if equal, 1 if after</summary>
            int CompareTo(IPackageNode other);
        }

        private class PackageList : IPackageNode {
            private List<IPackageNode> children { get; } = new List<IPackageNode>();

            private PackageList(IEnumerable<IPackageNode> items) => children.AddRange(items);
            public PackageList(IPackageNode item) => children.Add(item);
            public PackageList(int value) => children.Add(new PackageValue(value));


            public static IPackageNode Parse(string str) {
                if (PackageValue.TryParse(str, out var value)) return value;
                if (str.StartsWith("[") && str.EndsWith("]")) return new PackageList(SplitOnCommasLevel0(str.Substring(1, str.Length - 2)).Where(t => !string.IsNullOrEmpty(t)).Select(Parse));
                Console.WriteLine($"Didn't understand input {str}");
                return null;
            }

            public int CompareTo(IPackageNode other) {
                switch (other) {
                    case PackageList otherAsList: return CompareTo(otherAsList);
                    case PackageValue _: return CompareTo(new PackageList(other));
                    default:
                        Console.WriteLine($"Unhandled other of type {other.GetType().Name}");
                        return -2;
                }
            }

            private int CompareTo(PackageList otherList) {
                for (var i = 0; i < children.Count; ++i) {
                    if (otherList.children.Count <= i) return 1;
                    var compareChild = children[i].CompareTo(otherList.children[i]);
                    if (compareChild != 0) return compareChild;
                }

                return children.Count < otherList.children.Count ? -1 : 0;
            }

            private static IEnumerable<string> SplitOnCommasLevel0(string str) {
                var parts = new List<string>();
                var level = 0;
                var builder = new StringBuilder();
                foreach (var c in str) {
                    if (level == 0 && c == ',') {
                        parts.Add(builder.ToString());
                        builder.Clear();
                    }
                    else {
                        builder.Append(c);
                        if (c == '[') level++;
                        if (c == ']') level--;
                    }
                }

                parts.Add(builder.ToString());

                return parts;
            }

            public override string ToString() => $"[{string.Join(",", children)}]";
        }


        private class PackageValue : IPackageNode {
            private int value { get; }


            public PackageValue(int value) => this.value = value;

            public static bool TryParse(string input, out PackageValue result) => (result = int.TryParse(input, out var intValue) ? new PackageValue(intValue) : null) != null;

            public override string ToString() => $"{value}";

            public int CompareTo(IPackageNode other) {
                switch (other) {
                    case PackageValue otherAsValue: return Math.Clamp(value - otherAsValue.value, -1, 1);
                    case PackageList _: return new PackageList(this).CompareTo(other);
                    default:
                        Console.WriteLine($"Unhandled other of type {other.GetType().Name}");
                        return -2;
                }
            }
        }
    }
}