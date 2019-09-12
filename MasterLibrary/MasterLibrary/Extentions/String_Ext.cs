using System;

namespace MasterLibrary.Extentions
{
    public static class String_Ext
    {
        public static int LevenshteinDistance(this string s, string t)
        {
            int n = s.Length;
            int m = t.Length;
            int[,] d = new int[n + 1, m + 1];

            // Step 1
            if (n == 0)
            {
                return m;
            }

            if (m == 0)
            {
                return n;
            }

            // Step 2
            for (int i = 0; i <= n; d[i, 0] = i++)
            {
            }

            for (int j = 0; j <= m; d[0, j] = j++)
            {
            }

            // Step 3
            for (int i = 1; i <= n; i++)
            {
                //Step 4
                for (int j = 1; j <= m; j++)
                {
                    // Step 5
                    int cost = (t[j - 1] == s[i - 1]) ? 0 : 1;

                    // Step 6
                    d[i, j] = Math.Min(
                        Math.Min(d[i - 1, j] + 1, d[i, j - 1] + 1),
                        d[i - 1, j - 1] + cost);
                }
            }
            // Step 7
            return d[n, m];
        }

        public static bool ContainsAny(this string s, string[] filter, bool ignoreCase = false)
        {
            if (ignoreCase)
            {
                string ss = s.ToLower();
                foreach (string f in filter)
                    if (ss.Contains(f.ToLower()))
                        return true;
            }
            else
            {
                foreach (string f in filter)
                    if (s.Contains(f))
                        return true;
            }
            return false;
        }
    }
}
