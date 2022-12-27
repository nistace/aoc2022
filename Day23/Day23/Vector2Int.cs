using System.Collections.Generic;

namespace Day22 {
    public readonly struct Vector2Int {
        public static Vector2Int zero { get; } = new Vector2Int(0, 0);
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
        public static Vector2Int operator *(Vector2Int v, int times) => new Vector2Int(v.x * times, v.y * times);
        public static Vector2Int operator *(int times, Vector2Int v) => new Vector2Int(v.x * times, v.y * times);
        public static Vector2Int operator -(Vector2Int v) => -1 * v;
        public static bool operator ==(Vector2Int first, Vector2Int second) => first.x == second.x && first.y == second.y;
        public static bool operator !=(Vector2Int first, Vector2Int second) => !(first == second);
        private bool Equals(Vector2Int other) => x == other.x && y == other.y;
        public override bool Equals(object obj) => obj is Vector2Int other && Equals(other);
        public override int GetHashCode() => (x * 397) ^ y;
        public override string ToString() => $"({x:000},{y:000})";

        public IEnumerable<Vector2Int> Get8Neighbours() => new[] {
            this + left, this + down, this + up, this + right, this + downLeft, this + downRight, this + upLeft, this + upRight
        };
    }
}