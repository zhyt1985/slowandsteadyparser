using System;
using System.Collections.Generic;
using System.Text;

namespace SlowAndSteadyParser
{
    public static class StringDifference
    {
        /// <summary>
        /// Compute Levenshtein distance
        /// </summary>
        /// <param name="s">String 1</param>
        /// <param name="t">String 2</param>
        /// <returns>Distance between the two strings.
        /// </returns>
        public static int ComputeLevenshtein(string s1, string s2)
        {

            int n = s1.Length; //length of s
            int m = s2.Length; //length of t
            int[,] d = new int[n + 1, m + 1]; // matrix
            int cost; // cost

            // Step 1
            if (n == 0) return m;
            if (m == 0) return n;

            // Step 2
            for (int i = 0; i <= n; d[i, 0] = i++);
            for (int j = 0; j <= m; d[0, j] = j++);

            // Step 3
            for (int i = 1; i <= n; i++)
            {
                //Step 4
                for (int j = 1; j <= m; j++)
                {
                    // Step 5
                    cost = (s2.Substring(j - 1, 1) == s1.Substring(i - 1, 1) ? 0 : 1);

                    // Step 6
                    d[i, j] = System.Math.Min(System.Math.Min(d[i - 1, j] + 1, d[i, j - 1] + 1), d[i - 1, j - 1] + cost);
                }
            }

            // Step 7
            return d[n, m];
        }


        /// <summary>
        /// Compute Levenshtein-Distance Index
        /// </summary>
        /// <param name="s">String 1</param>
        /// <param name="t">String 2</param>
        /// <returns>Distance Index between the two strings.
        /// </returns>
        public static double ComputeLevenshteinIndex(string s1, string s2)
        {
            return (double)(2 * ComputeLevenshtein(s1, s2)) / (double)(s1.Length + s2.Length);
        }

        /// <summary>
        /// Compute LC String
        /// </summary>
        /// <param name="s">String 1</param>
        /// <param name="t">String 2</param>
        /// <returns>Distance String between the two strings.
        /// </returns>
        public static string ComputeLCString(string str1, string str2)
        {
            if (str1.Length < str2.Length)
            {
                string strTemp = str1;
                str1 = str2;
                str2 = strTemp;
            }

            int[] sign = new int[str1.Length];
            int length = 0;
            int end = 0;
            for (int i = 0; i < str2.Length; i++)
            {
                for (int j = str1.Length - 1; j >= 0; j--)
                {

                    if (str2[i] == str1[j])
                    {
                        if (i == 0 || j == 0)
                            sign[j] = 1;
                        else
                            sign[j] = sign[j - 1] + 1;
                    }
                    else
                        sign[j] = 0;

                    if (sign[j] > length)
                    {
                        length = sign[j];
                        end = j;
                    }
                }
            }
            return str1.Substring(end - length + 1, length);
        }

        public static double ComputeLCSIndex(string str1, string str2)
        {
            return 1 - 2 * (double)ComputeLCString(str1, str2).Length / (double)(str1.Length + str2.Length);
        }
    }


}
