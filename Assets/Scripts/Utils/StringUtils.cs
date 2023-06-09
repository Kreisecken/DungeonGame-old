using System;
using System.Collections;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

namespace DungeonGame.Utils
{
    public static class StringUtils
    {
        public static char[] Alphabet = "abcdefghijklmnopqrstuvwxyz".ToCharArray();
        public static char[] Digits = "0123456789".ToCharArray();
        public static char[] AlphaNumeric = "abcdefghijklmnopqrstuvwxyz0123456789".ToCharArray();

        private static MD5 MD5_Instance { get; } = MD5.Create();

        // string.GetHashCode() returns different results
        // for different devices, but this Method is (should be) consistent
        public static long MD5HashCode(this string str)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(str);

            byte[] hash = MD5_Instance.ComputeHash(bytes);

            long result = BitConverter.ToInt64(hash, 0);

            return result;
        }
    }
}