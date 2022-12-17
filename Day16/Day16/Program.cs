using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Day16 {
    internal static class Program {
        private static void Main() {
            Console.WriteLine($"Part 1: {Part1()}");
            Console.WriteLine("\n-----------------\n");
            Console.WriteLine($"Part 2: {Part2()}");
        }

        private static string Part1() {
            var graph = ReadInput();
            if (graph.origin.flowRate > 0) Console.WriteLine("Assumed that origin has a flow rate of 0, which is not true. Algorithm will probably fail.");
            var openedValves = new Dictionary<Node, int> {{graph.origin, 0}};
            return $"{GetMaxReleasedPressure2(new[] {new Worker(graph.origin, 30)}, graph, openedValves)}";
        }

        private static string Part2() {
            var graph = ReadInput();
            if (graph.origin.flowRate > 0) Console.WriteLine("Assumed that origin has a flow rate of 0, which is not true. Algorithm will probably fail.");
            var openedValves = new Dictionary<Node, int> {{graph.origin, 0}};
            return $"{GetMaxReleasedPressure2(new[] {new Worker(graph.origin, 26), new Worker(graph.origin, 26)}, graph, openedValves)}";
        }

        private static int GetMaxReleasedPressure2(IReadOnlyList<Worker> workers, Graph graph, IDictionary<Node, int> pressureReleasedByVisitedNodes) {
            if (!workers.Any(t => t.remainingTime > 0)) return 0;
            var thisWorker = workers.OrderByDescending(t => t.remainingTime).First();
            var max = pressureReleasedByVisitedNodes.Sum(t => t.Value);
            if (thisWorker.remainingTime == 0 || graph.countUsefulNodes == pressureReleasedByVisitedNodes.Count) {
                return max;
            }

            var origin = thisWorker.currentNode;
            foreach (var (candidate, travelTimeCost) in graph.GetTunnelsFrom(origin).Where(t => !pressureReleasedByVisitedNodes.ContainsKey(t.Key) && t.Value + 1 < thisWorker.remainingTime)) {
                thisWorker.currentNode = candidate;
                thisWorker.remainingTime -= travelTimeCost + 1;
                pressureReleasedByVisitedNodes.Add(thisWorker.currentNode, thisWorker.remainingTime * candidate.flowRate);
                max = Math.Max(max, GetMaxReleasedPressure2(workers, graph, pressureReleasedByVisitedNodes));
                pressureReleasedByVisitedNodes.Remove(candidate);
                thisWorker.currentNode = origin;
                thisWorker.remainingTime += travelTimeCost + 1;
            }

            return max;
        }

        private class Worker {
            public Node currentNode { get; set; }
            public int remainingTime { get; set; }

            public Worker(Node origin, int initialTime) {
                currentNode = origin;
                remainingTime = initialTime;
            }
        }

        private static Graph ReadInput() {
            var inputMatches = File.ReadAllLines("input.txt").Select(t => Regex.Match(t, "Valve *([A-Z]{2}).*rate=(\\d+);.*valves? *([A-Z, ]*)")).Where(t => t.Success).ToArray();
            var nodes = inputMatches.ToDictionary(t => t.Groups[1].Value, t => new Node(int.Parse(t.Groups[2].Value)));
            var connections = inputMatches.ToDictionary(t => nodes[t.Groups[1].Value], t => (IReadOnlyList<Node>) t.Groups[3].Value.Split(", ").Select(u => nodes[u]).ToArray());
            return new Graph("AA", nodes, connections);
        }

        private class Graph {
            private Node[] usefulNodes { get; }
            public Node origin { get; }
            private Dictionary<Node, IReadOnlyDictionary<Node, int>> costPerTunnel { get; } = new Dictionary<Node, IReadOnlyDictionary<Node, int>>();
            public int countUsefulNodes => usefulNodes.Length;


            public Graph(string initialPoint, IReadOnlyDictionary<string, Node> allNodes, IReadOnlyDictionary<Node, IReadOnlyList<Node>> connections) {
                usefulNodes = allNodes.Where(t => t.Value.flowRate > 0 || t.Key == initialPoint).Select(t => t.Value).ToArray();
                origin = allNodes[initialPoint];
                foreach (var node in usefulNodes) {
                    costPerTunnel.Add(node, GetShortestPaths(node, connections).Where(t => t.Key.flowRate > 0 && t.Key != node).ToDictionary(t => t.Key, t => t.Value));
                }
            }

            private static IReadOnlyDictionary<Node, int> GetShortestPaths(Node node, IReadOnlyDictionary<Node, IReadOnlyList<Node>> connections) {
                var pathWeights = new Dictionary<Node, int> {{node, 0}};
                var open = new List<Node> {node};
                var closed = new List<Node>();
                while (open.Count > 0 && pathWeights.Count < connections.Count) {
                    foreach (var connection in connections[open[0]].Where(connection => !open.Contains(connection) && !closed.Contains(connection))) {
                        pathWeights.Add(connection, pathWeights[open[0]] + 1);
                        open.Add(connection);
                    }

                    closed.Add(open[0]);
                    open.RemoveAt(0);
                }

                return pathWeights;
            }

            public IReadOnlyDictionary<Node, int> GetTunnelsFrom(Node node) => costPerTunnel[node];
        }

        private class Node {
            public int flowRate { get; }

            public Node(int flowRate) {
                this.flowRate = flowRate;
            }
        }
    }
}