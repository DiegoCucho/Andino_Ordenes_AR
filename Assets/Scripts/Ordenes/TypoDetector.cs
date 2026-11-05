using System;
using System.Collections.Generic;

/// <summary>Detecta beneficiarios mal escritos (caso «Fandation») con distancia de Levenshtein.</summary>
public static class TypoDetector
{
    public static int Levenshtein(string a, string b)
    {
        var d = new int[a.Length + 1, b.Length + 1];
        for (int i = 0; i <= a.Length; i++) d[i, 0] = i;
        for (int j = 0; j <= b.Length; j++) d[0, j] = j;
        for (int i = 1; i <= a.Length; i++)
            for (int j = 1; j <= b.Length; j++)
            {
                int c = a[i - 1] == b[j - 1] ? 0 : 1;
                d[i, j] = Math.Min(Math.Min(d[i - 1, j] + 1, d[i, j - 1] + 1), d[i - 1, j - 1] + c);
            }
        return d[a.Length, b.Length];
    }

    /// <summary>True si el nombre no es válido pero se parece a uno válido (1 a 3 ediciones).</summary>
    public static bool EsSospechoso(string beneficiario, IEnumerable<string> validos, out string sugerido)
    {
        sugerido = null;
        string b = beneficiario.ToUpperInvariant();
        foreach (var v in validos)
        {
            int dist = Levenshtein(b, v.ToUpperInvariant());
            if (dist == 0) return false;
            if (dist <= 3) { sugerido = v; return true; }
        }
        return false;
    }
}
