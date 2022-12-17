using System;
using System.Collections.Generic;

namespace Day15 {
    public readonly struct Vector2Int {
        public static Vector2Int up { get; } = new Vector2Int(0, 1);
        public static Vector2Int down { get; } = new Vector2Int(0, -1);
        public static Vector2Int right { get; } = new Vector2Int(1, 0);
        public static Vector2Int left { get; } = new Vector2Int(-1, 0);
        public static Vector2Int downLeft { get; } = new Vector2Int(-1, -1);
        public static Vector2Int downRight { get; } = new Vector2Int(1, -1);
        public static Vector2Int upRight { get; } = new Vector2Int(1, 1);
        public static Vector2Int upLeft { get; } = new Vector2Int(-1, 1);

        public int x { get; }
        public int y { get; }

        public Vector2Int(int x, int y) {
            this.x = x;
            this.y = y;
        }

        public static implicit operator Vector2Int((int x, int y) d) => new Vector2Int(d.x, d.y);
        public static Vector2Int operator +(Vector2Int first, Vector2Int second) => new Vector2Int(first.x + second.x, first.y + second.y);
        public static Vector2Int operator -(Vector2Int first, Vector2Int second) => new Vector2Int(first.x - second.x, first.y - second.y);
        public static bool operator ==(Vector2Int first, Vector2Int second) => first.x == second.x && first.y == second.y;
        public static bool operator !=(Vector2Int first, Vector2Int second) => !(first == second);
        private bool Equals(Vector2Int other) => x == other.x && y == other.y;
        public override bool Equals(object obj) => obj is Vector2Int other && Equals(other);
        public override int GetHashCode() => (x * 397) ^ y;
        public override string ToString() => $"({x:000},{y:000})";

        public int GetManhattanDistance(Vector2Int other) => Math.Abs(other.x - x) + Math.Abs(other.y - y);


        /// <summary>Returns a position a step towards the given point, where this position is moved by at most one unit on the X axis and one unit on the Y axis</summary>
        public Vector2Int StepTowards(Vector2Int other) => new Vector2Int(x + Math.Clamp(other.x - x, -1, 1), y + Math.Clamp(other.y - y, -1, 1));

        public IEnumerable<Vector2Int> FindAllAtManhattanDistance(int distance) {
            var result = new HashSet<Vector2Int>();
            var other = new Vector2Int(x - distance, y);
            result.Add(other);
            foreach (var direction in new[] {upRight, downRight, downLeft, upLeft}) {
                while ((other + direction).GetManhattanDistance(this) == distance) {
                    other += direction;
                    result.Add(other);
                }
            }

            return result;
        }
    }
}