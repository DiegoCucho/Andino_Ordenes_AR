using System.Collections.Generic;

public class RuleResult
{
    public string estado;                 // "RETENIDA" o "LIBERADA"
    public List<string> motivos = new List<string>();
}

/// <summary>Controles del modo defensa: monto, horario, beneficiario nuevo/mal escrito y doble aprobación.</summary>
public static class RuleEngine
{
    public const float LimiteMillones = 5f;
    public const int HoraInicio = 8, HoraFin = 18;

    public static RuleResult Evaluar(OrderData o)
    {
        var r = new RuleResult();

        if (o.montoMillones > LimiteMillones)
            r.motivos.Add("Monto sobre el límite (US$" + LimiteMillones + " M)");

        if (!HoraHabil(o.hora))
            r.motivos.Add("Fuera de horario (" + o.hora + ")");

        if (o.beneficiarioNuevo)
            r.motivos.Add("Beneficiario nunca usado");

        var db = OrderDatabase.Load();
        if (TypoDetector.EsSospechoso(o.beneficiario, db.beneficiariosValidos, out var sug))
            r.motivos.Add("Nombre sospechoso: ¿" + sug + "?");

        if (!o.dobleAprobacion)
            r.motivos.Add("Sin doble aprobación");

        r.estado = r.motivos.Count > 0 ? "RETENIDA" : "LIBERADA";
        return r;
    }

    static bool HoraHabil(string hhmm)
    {
        if (string.IsNullOrEmpty(hhmm) || hhmm.Length < 2) return false;
        if (!int.TryParse(hhmm.Substring(0, 2), out int h)) return false;
        return h >= HoraInicio && h < HoraFin;
    }
}
