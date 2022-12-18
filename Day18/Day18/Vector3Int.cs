namespace Day18 {
    public readonly struct Vector3Int {
        public static Vector3Int zero { get; } = new Vector3Int(0, 0, 0);
        public static Vector3Int up { get; } = new Vector3Int(0, 1, 0);
        public static Vector3Int down { get; } = new Vector3Int(0, -1, 0);
        public static Vector3Int right { get; } = new Vector3Int(1, 0, 0);
        public static Vector3Int left { get; } = new Vector3Int(-1, 0, 0);
        public static Vector3Int forward { get; } = new Vector3Int(0, 0, 1);
        public static Vector3Int back { get; } = new Vector3Int(0, 0, -1);
        public static Vector3Int one { get; } = new Vector3Int(1, 1, 1);

        public int x { get; }
        public int y { get; }
        public int z { get; }

        public Vector3Int(int x, int y, int z) {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public static implicit operator Vector3Int((int x, int y, int z) d) => new Vector3Int(d.x, d.y, d.z);
        public static Vector3Int operator +(Vector3Int first, Vector3Int second) => new Vector3Int(first.x + second.x, first.y + second.y, first.z + second.z);
        public static Vector3Int operator -(Vector3Int first, Vector3Int second) => new Vector3Int(first.x - second.x, first.y - second.y, first.z - second.z);
        public static Vector3Int operator *(Vector3Int v, int times) => new Vector3Int(v.x * times, v.y * times, v.z * times);
        public static Vector3Int operator *(int times, Vector3Int v) => new Vector3Int(v.x * times, v.y * times, v.z * times);
        public static bool operator ==(Vector3Int first, Vector3Int second) => first.x == second.x && first.y == second.y && first.z == second.z;
        public static bool operator !=(Vector3Int first, Vector3Int second) => !(first == second);
        private bool Equals(Vector3Int other) => x == other.x && y == other.y && z == other.z;
        public override bool Equals(object obj) => obj is Vector3Int other && Equals(other);
        public override int GetHashCode() => (((x * 397) ^ y) * 397) ^ z;
        public override string ToString() => $"({x:000},{y:000},{z:000})";

        public Vector3Int[] Get6Neighbours() => new[] {this + up, this + down, this + left, this + right, this + forward, this + back};
    }
}