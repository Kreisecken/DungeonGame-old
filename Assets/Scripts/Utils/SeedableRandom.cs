using System;
using System.Collections;
using UnityEngine;

namespace DungeonGame.Utils
{
    public class SeedableRandom
    {
        private Seed _seed;

        public Seed Seed
        {
            get => _seed;
            set
            {
                _seed = value;

                state = Seed.CreateState(4);
            }
        }

        private long[] state;

        public SeedableRandom()          => Seed = Environment.TickCount;
        public SeedableRandom(Seed seed) => Seed = seed;

        public double Double()
        {
            return (long)((ulong)Next() >> 11) * 2.9103830456733704e-11; // 0x1.0p-53
        }

        public int Int32(int bound)
        {
            return (int) Int64(bound);
        }

        public int Int32(int min, int max)
        {
            return Int32(max - min + 1) + min;
        }

        public long Int64(long bound)
        {
            return (Next() & long.MaxValue) % bound;
        }

        public long Int64(int min, int max)
        {
            return Int64(max - min + 1) + min;
        }

        public double Double(double min, double max)
        {
            return Double() * (max - min) + min;
        }

        public bool Bool(double probability)
        {
            return Double() < probability;
        }

        public string String(int length, char[] charSet)
        {
            char[] chars = new char[length];

            for (int i = 0; i < length; i++)
                chars[i] = charSet[Int32(0, charSet.Length - 1)];

            return new string(chars);
        }

        //xoshiro256ss
        private long Next()
        {
            long result = Rol64(state[1] * 5, 7) * 9;

            long t = state[1] << 17;

            state[2] ^= state[0];
            state[3] ^= state[1];
            state[1] ^= state[2];
            state[0] ^= state[3];

            state[2] ^= t;

            state[3] = Rol64(state[3], 45);

            return result;
        }

        private static long Rol64(long x, int k)
        {
            return (x << k) | ((long) ((ulong) x >> (64 - k)));
        }

        public static implicit operator Seed  (SeedableRandom random) => random.Seed;
        public static implicit operator SeedableRandom(Seed   seed  ) => new(seed);
    }

    public struct Seed
    {
        private readonly long value;

        public Seed(long   seed) => value = seed;
        public Seed(string seed) => value = seed.MD5HashCode();

        public long[] CreateState(int length)
        {
            long[] result = new long[length];

            long state = value;

            // splitmix64
            for (int i = 0; i < length; i++)
            {
                state += -7046029254386353131L;

                long random = state;

                random = (random ^ (random >> 30)) * -4658895280553007687L;
                random = (random ^ (random >> 27)) * -7723592293110705685L;

                result[i] = random ^ (random >> 31);
            }

            return result;
        }

        public static implicit operator long(Seed   seed) => seed.value;
        public static implicit operator Seed(long   seed) => new(seed);
        public static implicit operator Seed(string seed) => new(seed);
    }
}