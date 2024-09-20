using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using HenryDev;
using Unity.VisualScripting;
using UnityEngine;

namespace HenryDev.Math
{
    public class IInt
    {
        public string Value;
        private List<(string suffix, string value)> scaledFactor = new List<(string suffix, string value)>()
        {
            ("a", "1_000"),
            ("b", "1_000_000"),
            ("c", "1_000_000_000"),
            ("d", "1_000_000_000_000"),
            ("e", "1_000_000_000_000_000"),
            ("f", "1_000_000_000_000_000_000"),
            ("g", "1_000_000_000_000_000_000_000"),
            ("h", "1_000_000_000_000_000_000_000_000"),
            ("i", "1_000_000_000_000_000_000_000_000_000"),
            ("j", "1_000_000_000_000_000_000_000_000_000_000"),
            ("k", "1_000_000_000_000_000_000_000_000_000_000_000"),
            ("l", "1_000_000_000_000_000_000_000_000_000_000_000_000"),
            ("m", "1_000_000_000_000_000_000_000_000_000_000_000_000_000"),
            ("n", "1_000_000_000_000_000_000_000_000_000_000_000_000_000_000"),
            ("o", "1_000_000_000_000_000_000_000_000_000_000_000_000_000_000_000"),
            ("p", "1_000_000_000_000_000_000_000_000_000_000_000_000_000_000_000_000"),
            ("q", "1_000_000_000_000_000_000_000_000_000_000_000_000_000_000_000_000_000"),
            ("r", "1_000_000_000_000_000_000_000_000_000_000_000_000_000_000_000_000_000_000"),
            ("s", "1_000_000_000_000_000_000_000_000_000_000_000_000_000_000_000_000_000_000_000"),
            ("t", "1_000_000_000_000_000_000_000_000_000_000_000_000_000_000_000_000_000_000_000_000"),
            ("u", "1_000_000_000_000_000_000_000_000_000_000_000_000_000_000_000_000_000_000_000_000_000"),
            ("v", "1_000_000_000_000_000_000_000_000_000_000_000_000_000_000_000_000_000_000_000_000_000_000"),
            ("w", "1_000_000_000_000_000_000_000_000_000_000_000_000_000_000_000_000_000_000_000_000_000_000_000"),
            ("x", "1_000_000_000_000_000_000_000_000_000_000_000_000_000_000_000_000_000_000_000_000_000_000_000_000"),
            ("y", "1_000_000_000_000_000_000_000_000_000_000_000_000_000_000_000_000_000_000_000_000_000_000_000_000_000"),
            ("z", "1_000_000_000_000_000_000_000_000_000_000_000_000_000_000_000_000_000_000_000_000_000_000_000_000_000_000"),
        };
        private int suffixCycle => this.scaledFactor.Count; // 26
        private int precision;

        public int Length => Value.Length;
        public static IInt Zero => new IInt("0");
        public bool IsSigned => Value.StartsWith("-");
        public static string Absolute(IInt value)
        {
            if (value.IsSigned)
            {
                return value.Value.Substring(1);
            }
            return value.Value;
        }
        public static string Absolute(string value)
        {
            if (value.StartsWith("-"))
            {
                return value.Substring(1);
            }
            return value;
        }
        /// <summary>
        /// Return a clean value of the given IInt (remove leading zeros and underscores)
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string CleanValue(IInt value)
        {
            if (value.IsSigned)
            {
                return value.Value.Replace("_", "").TrimLeft('0', skipFromIndex: 0);
            }
            return value.Value.Replace("_", "").TrimLeft('0');
        }
        /// <summary>
        /// Return a clean value of the given IInt (remove leading zeros and underscores)
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string CleanValue(string value)
        {
            if (value.StartsWith("-"))
            {
                return value.Replace("_", "").TrimLeft('0', skipFromIndex: 0);
            }
            return value.Replace("_", "").TrimLeft('0');
        }
        /// <summary>
        /// Clean current value (remove leading zeros and underscores)
        /// </summary>
        public void CleanSelf()
        {
            Value = CleanValue(this);
        }
        public IInt(string value, int precision = 3)
        {
            this.precision = Mathf.Min(this.suffixCycle, precision);
            Value = value;
        }
        public string ToScaledString()
        {
            string actualValue = CleanValue(this);
            if (actualValue.Length <= 3)
            {
                return actualValue;
            }
            List<string> segments = actualValue.Cut(segmentLength: 3, isReversed: true);
            int length = segments.Count;
            int precisionSegmentCount = Mathf.CeilToInt((float)this.precision / 3f);
            int suffixIndex = length - 2;
            if (suffixIndex < 0)
            {
                return actualValue;
            }
            int fullSuffixIndex = suffixIndex / this.suffixCycle;
            if (fullSuffixIndex > 0)
            {
                suffixIndex %= this.suffixCycle;
            }
            string suffix = this.scaledFactor[suffixIndex].suffix;
            for (int i = 0; i < fullSuffixIndex; i++)
            {
                suffix = this.scaledFactor[^1].suffix + suffix;
            }
            List<string> precisionSegments = segments.GetRange(0, precisionSegmentCount);
            string value = string.Join(",", precisionSegments) + suffix;
            return value;
        }
        public override string ToString()
        {
            return Value;
        }
        public string ToFormattedString(string separator = "_")
        {
            string clean = CleanValue(this);
            List<string> segments = clean.Cut(segmentLength: 3, isReversed: true);
            return string.Join(separator, segments);
        }
        private static int CompareAbsoluteValues(string a, string b)
        {
            if (a.Length != b.Length)
                return a.Length.CompareTo(b.Length);
            return string.Compare(a, b);
        }

        public static IInt operator +(IInt a, IInt b)
        {
            bool isANegative = a.IsSigned;
            bool isBNegative = b.IsSigned;

            string cleanA = CleanValue(a);
            string cleanB = CleanValue(b);

            string absA = Absolute(cleanA);
            string absB = Absolute(cleanB);

            if (isANegative && isBNegative)
            {
                return new IInt("-" + AddAbsoluteValues(absA, absB));
            }
            else if (!isANegative && !isBNegative)
            {
                return new IInt(AddAbsoluteValues(absA, absB));
            }
            else
            {
                if (CompareAbsoluteValues(absA, absB) >= 0)
                {
                    return new IInt((isANegative ? "-" : "") + SubtractAbsoluteValues(absA, absB));
                }
                else
                {
                    return new IInt((isBNegative ? "-" : "") + SubtractAbsoluteValues(absB, absA));
                }
            }
        }
        public static IInt operator -(IInt a, IInt b)
        {
            bool isANegative = a.IsSigned;
            bool isBNegative = b.IsSigned;

            string cleanA = CleanValue(a);
            string cleanB = CleanValue(b);

            string absA = Absolute(cleanA);
            string absB = Absolute(cleanB);

            if (isANegative && isBNegative)
            {
                if (CompareAbsoluteValues(absA, absB) >= 0)
                {
                    return new IInt(SubtractAbsoluteValues(absA, absB));
                }
                else
                {
                    return new IInt("-" + SubtractAbsoluteValues(absB, absA));
                }
            }
            else if (!isANegative && !isBNegative)
            {
                if (CompareAbsoluteValues(absA, absB) >= 0)
                {
                    return new IInt(SubtractAbsoluteValues(absA, absB));
                }
                else
                {
                    return new IInt("-" + SubtractAbsoluteValues(absB, absA));
                }
            }
            else
            {
                return new IInt(AddAbsoluteValues(absA, absB));
            }
        }
        public static IInt operator *(IInt a, IInt b)
        {
            bool isANegative = a.IsSigned;
            bool isBNegative = b.IsSigned;

            string cleanA = CleanValue(a);
            string cleanB = CleanValue(b);

            string absA = Absolute(cleanA);
            string absB = Absolute(cleanB);

            string result = MultiplyAbsoluteValues(absA, absB);

            bool isResultNegative = isANegative ^ isBNegative;
            return new IInt(isResultNegative ? "-" + result : result);
        }
        private static string AddAbsoluteValues(string a, string b)
        {
            int maxLen = Mathf.Max(a.Length, b.Length);
            string newA = a.PadLeft(maxLen, '0');
            string newB = b.PadLeft(maxLen, '0');

            int carry = 0;
            char[] result = new char[maxLen];

            for (int i = maxLen - 1; i >= 0; i--)
            {
                int digitSum = (newA[i] - '0') + (newB[i] - '0') + carry;
                carry = digitSum / 10;
                result[i] = (char)((digitSum % 10) + '0');
            }

            if (carry > 0)
            {
                char[] finalResult = new char[maxLen + 1];
                finalResult[0] = (char)(carry + '0');
                Array.Copy(result, 0, finalResult, 1, maxLen);
                return new string(finalResult);
            }
            return new string(result);
        }

        private static string SubtractAbsoluteValues(string a, string b)
        {
            int maxLen = Mathf.Max(a.Length, b.Length);
            string newA = a.PadLeft(maxLen, '0');
            string newB = b.PadLeft(maxLen, '0');

            int borrow = 0;
            char[] result = new char[maxLen];

            for (int i = maxLen - 1; i >= 0; i--)
            {
                int digitDiff = (newA[i] - '0') - (newB[i] - '0') - borrow;
                if (digitDiff < 0)
                {
                    digitDiff += 10;
                    borrow = 1;
                }
                else
                {
                    borrow = 0;
                }
                result[i] = (char)(digitDiff + '0');
            }

            return new string(result).TrimStart('0');
        }
        private static string MultiplyAbsoluteValues(string a, string b)
        {
            int lenA = a.Length;
            int lenB = b.Length;
            int[] result = new int[lenA + lenB];

            for (int i = lenA - 1; i >= 0; i--)
            {
                for (int j = lenB - 1; j >= 0; j--)
                {
                    int mul = (a[i] - '0') * (b[j] - '0');
                    int sum = mul + result[i + j + 1];

                    result[i + j + 1] = sum % 10;
                    result[i + j] += sum / 10;
                }
            }

            StringBuilder sb = new StringBuilder();
            foreach (int num in result)
            {
                if (!(sb.Length == 0 && num == 0)) // Skip leading zeros
                {
                    sb.Append(num);
                }
            }

            return sb.Length == 0 ? "0" : sb.ToString();
        }
    }
}

