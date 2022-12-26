using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;

namespace Day19 {
    internal static class Program {
        private static Dictionary<BluePrint, int> bluePrintScore { get; } = new Dictionary<BluePrint, int>();

        private static HashSet<Thread> threads { get; } = new HashSet<Thread>();

        private static void Main() {
            Console.WriteLine($"Part 1: {Part1()}");
            Console.WriteLine("\n-----------------\n");
            Console.WriteLine($"Part 2: {Part2()}");
        }


        private static string Part1() {
            var blueprints = ReadInput();
            bluePrintScore.Clear();
            var index = 0;
            foreach (var bluePrint in blueprints) {
                var thread = new Thread(() => GetBlueprintMaxProduction(bluePrint)) {Name = $"T{index++}"};
                thread.Start();
                threads.Add(thread);
            }

            while (threads.Any()) {
                if (threads.All(t => t.IsAlive)) continue;
                Console.WriteLine($"Done: {string.Join(", ", threads.Where(t => !t.IsAlive).Select(t => t.Name))} {(DateTime.UtcNow - Process.GetCurrentProcess().StartTime.ToUniversalTime())}");
                threads.RemoveWhere(t => !t.IsAlive);
                Console.WriteLine($"Still running: {string.Join(", ", threads.Select(t => t.Name))}");
            }

            return $"{bluePrintScore.Sum(t => t.Key.ProductionToScore(t.Value))}{Environment.NewLine}" + string.Join(Environment.NewLine, bluePrintScore.Select(t => $"{t.Key}: {t.Value}"));
        }

        private static string Part2() {
            return "";
        }


        private static void GetBlueprintMaxProduction(BluePrint bluePrint) {
            var initialProduction = new ResourceSet(new[] {(Resource.Ore, 1)});
            var score = Enum.GetValues<Resource>().Select(t => GetBlueprintMaxProduction(bluePrint, t, ResourceSet.zero.Duplicate(), initialProduction.Duplicate(), 24)).Max();
            bluePrintScore.Add(bluePrint, score);
        }

        private static int GetBlueprintMaxProduction(BluePrint bluePrint, Resource nextRobotToBuild, ResourceSet producedResources, ResourceSet production, int remainingTime) {
            while (!producedResources.IsEnoughToPay(bluePrint.GetRobotCost(nextRobotToBuild))) {
                producedResources.Add(production);
                remainingTime--;
                if (remainingTime <= 0) return producedResources[Resource.Geode];
            }

            producedResources.Pay(bluePrint.GetRobotCost(nextRobotToBuild));
            producedResources.Add(production);
            production.Add(nextRobotToBuild, 1);
            remainingTime--;
            if (remainingTime <= 0) return producedResources[Resource.Geode];

            return Enum.GetValues<Resource>().Select(resource => GetBlueprintMaxProduction(bluePrint, resource, producedResources.Duplicate(), production.Duplicate(), remainingTime)).Concat(new[] {0}).Max();
        }

        private static IEnumerable<BluePrint> ReadInput() =>
            File.ReadAllLines("input.txt").Select(t => Regex.Match(t, "Blueprint (\\d+):.*ore.*costs([ \\da-z]+)\\..*clay.*costs([ \\da-z]+)\\..*obsidian.*costs([ \\da-z]+)\\..*geode.*costs([ \\da-z]+)\\.")).Where(t => t.Success)
                .Select(t =>
                    new BluePrint(int.Parse(t.Groups[1].Value), new[] {
                            (Resource.Ore, ParseCost(t.Groups[2].Value)),
                            (Resource.Clay, ParseCost(t.Groups[3].Value)),
                            (Resource.Obsidian, ParseCost(t.Groups[4].Value)),
                            (Resource.Geode, ParseCost(t.Groups[5].Value)),
                        }
                    ));

        private static IReadResourceSet ParseCost(string value) =>
            new ResourceSet(Regex.Matches(value, "(\\d+) *(ore|clay|obsidian|geode)").ToImmutableArray().Select(
                t => (Enum.Parse<Resource>(t.Groups[2].Value, true), int.Parse(t.Groups[1].Value))
            ));

        private class BluePrint {
            private int id { get; }
            private IReadResourceSet[] robotCosts { get; } = Enumerable.Repeat(ResourceSet.zero, Enum.GetValues<Resource>().Length).ToArray();

            public BluePrint(int id, IEnumerable<(Resource, IReadResourceSet)> robotCosts) {
                this.id = id;
                foreach (var (resource, cost) in robotCosts) this.robotCosts[(int) resource] = cost;
            }

            public IReadResourceSet GetRobotCost(Resource resource) => robotCosts[(int) resource];

            public override string ToString() => $"{id}: {string.Join(", ", Enum.GetValues<Resource>().Select(t => $"{t}[{robotCosts[(int) t]}]"))}";

            public long ProductionToScore(int production) => (long) id * production;
        }

        private enum Resource {
            Ore = 0,
            Clay = 1,
            Obsidian = 2,
            Geode = 3
        }

        private interface IReadResourceSet {
            int this[Resource resource] { get; }

            bool IsEnoughToPay(IReadResourceSet other);
            ResourceSet Duplicate();
        }


        private class ResourceSet : IReadResourceSet {
            public static IReadResourceSet zero { get; } = new ResourceSet(Array.Empty<(Resource, int)>());

            private int[] amounts { get; } = Enumerable.Repeat(0, Enum.GetValues<Resource>().Length).ToArray();

            public int this[Resource resource] {
                get => amounts[(int) resource];
                private set => amounts[(int) resource] = value;
            }


            public ResourceSet(IEnumerable<(Resource resource, int value)> amounts) {
                foreach (var (resource, value) in amounts) this[resource] += value;
            }

            public bool IsEnoughToPay(IReadResourceSet other) => Enum.GetValues<Resource>().All(t => this[t] >= other[t]);

            public ResourceSet Duplicate() => new ResourceSet(Enum.GetValues<Resource>().Select(t => (t, this[t])).ToArray());
            public override string ToString() => string.Join(" + ", Enum.GetValues<Resource>().Where(t => this[t] > 0).Select(t => $"{this[t]} {t}"));

            public void Add(Resource resource, int amount) => this[resource] += amount;

            public void Add(IReadResourceSet production) {
                foreach (var resource in Enum.GetValues<Resource>()) {
                    this[resource] += production[resource];
                }
            }

            public void Pay(IReadResourceSet production) {
                foreach (var resource in Enum.GetValues<Resource>()) {
                    this[resource] -= production[resource];
                }
            }
        }
    }
}