using System.Numerics;
using System.Text.RegularExpressions;

namespace Day25 {
    public struct Snafu {
        public static Snafu zero { get; } = (Snafu) 0;

        private BigInteger value { get; }


        private Snafu(BigInteger l) => value = l;

        public static implicit operator Snafu(int l) => new Snafu(l);
        public static implicit operator Snafu(long l) => new Snafu(l);
        public static implicit operator Snafu(BigInteger l) => new Snafu(l);

        public static bool TryParse(string s, out Snafu snafu) {
            snafu = default;
            if (!Regex.IsMatch(s, "[012\\-=]*")) return false;
            var value = (BigInteger) 0;
            var coefficient = (BigInteger) 1;
            for (var i = s.Length - 1; i >= 0; --i) {
                value += s[i] switch {'=' => -2, '-' => -1, _ => s[i] - '0'} * coefficient;
                coefficient *= 5;
            }

            snafu = new Snafu(value);
            return true;
        }

        public static Snafu Parse(string s) => TryParse(s, out var snafu) ? snafu : default;


        public override string ToString() {
            if (value == 0) return "0";
            if (value < 0) return "?? (<0)";
            var tmp = value;
            var result = "";
            while (tmp > 0) {
                if ((tmp % 5) == 3) result = $"={result}";
                else if (tmp % 5 == 4) result = $"-{result}";
                else result = $"{tmp % 5}{result}";
                if (tmp % 5 >= 3) tmp += 5;
                tmp /= 5;
            }

            return result;
        }

        public long ToLong() => (long) value;

        public static Snafu operator +(Snafu s, Snafu t) => new Snafu(s.value + t.value);
    }
}