using DungeonGame.Utils.Graph;
using System;
using System.Collections.Generic;
using Unity.Mathematics;
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

        public SeedableRandom()                  => Seed = Environment.TickCount;
        public SeedableRandom(Seed seed=default) => Seed = seed;

        public float Float32()
        {
            return (float) Int32() / int.MaxValue;
        }

        public float Float32(float min, float max)
        {
            return Float32() * (max - min) + min;
        }

        public double Float64()
        {
            return (double) UInt64() / ulong.MaxValue;
        }

        public double Float64(double min, double max)
        { 
            return Float64() * (max - min + 1) + min;
        }

        public int Int32()
        {
            return (int) (Int64() & int.MaxValue);
        }

        public int Int32(int bound)
        {
            return (int)(Int32() * ((double) bound / int.MaxValue));
        }

        public int Int32(int min, int max)
        {
            return Int32(max - min + 1) + min;
        }

        public long Int64()
        {
            return Next();
        }

        public long Int64(long bound)
        {
            return (long)(Int64() * ((double) bound / long.MaxValue));
        }

        public long Int64(int min, int max)
        {
            return Int64(max - min + 1) + min;
        }

        public ulong UInt64()
        {
            return (ulong) Int64();
        }

        public bool Bool(double probability)
        {
            return Float64() < probability; // TODO: 32 or 64? should work fine for now
        }

        public string String(int length, char[] charSet)
        {
            char[] chars = new char[length];

            for (int i = 0; i < length; i++)
                chars[i] = charSet[Int32(0, charSet.Length - 1)];

            return new string(chars);
        }

        public Vector2 PointInsideUnitCircle()
        {
            return PointInsideCircle(Circle.UNIT_CIRCLE);
        }

        public Vector2 PointInsideCircle(Circle circle)
        {
            return PointInsideCircle(circle.center, circle.radius);
        }

        public Vector2 PointInsideCircle(Vector2 center, float radius)
        {
            float theta = 2 * Mathf.PI * Float32();
            float r = radius * Mathf.Sqrt(Float32());

            return center + new Vector2(Mathf.Cos(theta), Mathf.Sin(theta)) * r;
        }

        // Fisher-Yates shuffle
        public void Shuffle<T>(IList<T> list)
        {
            for (int i = 0; i < list.Count - 2; i++)
            {
                int randomIndex = Int32(i, list.Count - 1);

                (list[randomIndex], list[i]) = (list[i], list[randomIndex]);
            }
        }

        public long Next()
        {
            return PRNGAlgorithms.Xoshiro256ss(ref state);
        }

        public static implicit operator Seed(SeedableRandom random) => random.Seed;
        public static implicit operator SeedableRandom(Seed   seed) => new(seed);
        public static implicit operator SeedableRandom(string seed) => new((Seed)seed);
        public static implicit operator SeedableRandom(long   seed) => new((Seed)seed);
    }

    public readonly struct Seed
    {
        private readonly long value;

        public Seed(long   seed) => value = seed;
        public Seed(string seed) => value = seed.MD5HashCode();

        public long[] CreateState(int length)
        {
            long[] result = new long[length];

            long state = value;

            for (int i = 0; i < length; i++)
                result[i] = PRNGAlgorithms.SplitMix64(ref state);

            return result;
        }

        public static implicit operator long(Seed   seed) => seed.value;
        public static implicit operator Seed(long   seed) => new(seed);
        public static implicit operator Seed(string seed) => new(seed);
    }

    public static class PRNGAlgorithms
    {
        public static long SplitMix64(ref long state)
        {
            state += -7046029254386353131L;

            long random = state;

            random = (random ^ (random >> 30)) * -4658895280553007687L;
            random = (random ^ (random >> 27)) * -7723592293110705685L;

            return random ^ (random >> 31);
        }

        public static long Xoshiro256ss(ref long[] state)
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
    }
}