using System;
using System.Linq;

class Program
{
    static void Main()
    {
        Console.Write("Ingrese el puente: ");
        string input = Console.ReadLine() ?? "";
        bool valido = IsValidBridge(input.Trim());
        Console.WriteLine(valido ? "VALIDO" : "INVALIDO");
    }

    static bool IsValidBridge(string s)
    {
        if (string.IsNullOrEmpty(s)) return false;

        if (s[0] != '*' || s[s.Length - 1] != '*') return false;

        if (s == "**") return true;

        if (s.Length < 2) return false;

        string core = s.Substring(1, s.Length - 2);

        if (core.Any(c => c != '=' && c != '+')) return false;

        if (!core.SequenceEqual(core.Reverse())) return false;

        int n = core.Length;

        for (int i = 0; i < n;)
        {
            if (core[i] == '=')
            {
                int j = i;
                while (j < n && core[j] == '=') j++;
                int len = j - i;

                if (len > 3) return false;

                if (len == 3)
                {
                    if (n % 2 == 0) return false;
                    int startCenter = (n - 3) / 2;
                    if (i != startCenter) return false;
                }

                bool leftAdj = (i - 1 >= 0 && core[i - 1] == '+');
                bool rightAdj = (j < n && core[j] == '+');
                if (!leftAdj && !rightAdj) return false;

                i = j;
            }
            else
            {
                i++;
            }
        }

        return true;
    }
}