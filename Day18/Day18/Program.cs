using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Day18 {
    internal static class Program {
        private static void Main() {
            Console.WriteLine($"Part 1: {Part1()}");
            Console.WriteLine("\n-----------------\n");
            Console.WriteLine($"Part 2: {Part2()}");
        }


        private static string Part1() {
            var droplets = ReadInput().ToArray();
            var dropletFaces = droplets.Length * 6;
            var adjacentFaces = droplets.Sum(t => t.Get6Neighbours().Count(u => droplets.Contains(u)));

            return $"{dropletFaces - adjacentFaces}"
                   + $"{Environment.NewLine}dropletFaces: {dropletFaces}"
                   + $"{Environment.NewLine}adjacentFaces: {adjacentFaces}";
        }


        private static string Part2() {
            var droplets = ReadInput().ToHashSet();
            var box = (
                min: (Vector3Int) (droplets.Min(t => t.x), droplets.Min(t => t.y), droplets.Min(t => t.z)),
                max: (Vector3Int) (droplets.Max(t => t.x), droplets.Max(t => t.y), droplets.Max(t => t.z))
            );
            var boxingCells = new HashSet<Vector3Int>();
            var dropletExternalFaces = 0;
            var boxingExternalFaces = 0;
            for (var x = box.min.x; x <= box.max.x; ++x)
            for (var y = box.min.y; y <= box.max.y; ++y)
            for (var z = box.min.z; z <= box.max.z; ++z) {
                if (x != box.min.x && y != box.min.y && z != box.min.z && x != box.max.x && y != box.max.y && z != box.max.z) continue;
                Vector3Int cell = (x, y, z);
                PropagateBoxingCells(box, cell, droplets, boxingCells);
                if (boxingCells.Contains(cell)) boxingExternalFaces += CountExternalFaces(cell, box);
                else dropletExternalFaces += CountExternalFaces(cell, box);
            }


            var boxingCellsFaces = boxingCells.Count * 6;
            var boxingAdjacentFaces = boxingCells.Sum(t => t.Get6Neighbours().Count(u => boxingCells.Contains(u)));

            return $"{boxingCellsFaces + dropletExternalFaces - boxingAdjacentFaces - boxingExternalFaces}"
                   + $"{Environment.NewLine}  boxingCellsFaces: {boxingCellsFaces}"
                   + $"{Environment.NewLine}+ dropletExternalFaces: {dropletExternalFaces}"
                   + $"{Environment.NewLine}- boxingAdjacentFaces: {boxingAdjacentFaces}"
                   + $"{Environment.NewLine}- externalFaces: {boxingExternalFaces}";
        }

        private static int CountExternalFaces(Vector3Int cell, (Vector3Int min, Vector3Int max) box) {
            var count = 0;
            var (min, max) = box;
            if (cell.x == min.x) count++;
            if (cell.y == min.y) count++;
            if (cell.z == min.z) count++;
            if (cell.x == max.x) count++;
            if (cell.y == max.y) count++;
            if (cell.z == max.z) count++;
            return count;
        }

        private static void PropagateBoxingCells((Vector3Int min, Vector3Int max) box, Vector3Int cell, IReadOnlySet<Vector3Int> droplets, ISet<Vector3Int> boxingCells) {
            if (droplets.Contains(cell)) return;
            if (boxingCells.Contains(cell)) return;
            if (cell.x < box.min.x || cell.y < box.min.y || cell.z < box.min.z) return;
            if (cell.x > box.max.x || cell.y > box.max.y || cell.z > box.max.z) return;
            boxingCells.Add(cell);
            foreach (var neighbour in cell.Get6Neighbours()) {
                PropagateBoxingCells(box, neighbour, droplets, boxingCells);
            }
        }


        private static IEnumerable<Vector3Int> ReadInput() =>
            File.ReadAllLines("input.txt").Select(t => Regex.Match(t, "(\\d+),(\\d+),(\\d+)")).Where(t => t.Success)
                .Select(t => (Vector3Int) (int.Parse(t.Groups[1].Value), int.Parse(t.Groups[2].Value), int.Parse(t.Groups[3].Value)));
    }
}