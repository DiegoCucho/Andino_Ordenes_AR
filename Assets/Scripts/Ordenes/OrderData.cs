using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class OrderData
{
    public string id;
    public string beneficiario;
    public string destino;
    public float montoMillones;
    public string estadoReal;      // bloqueada | pagada | detenida_fandation | no_reenviada
    public string hora;            // HH:mm
    public bool beneficiarioNuevo;
    public bool dobleAprobacion;
}

[Serializable]
public class OrderDatabaseDto
{
    public List<OrderData> ordenes;
    public List<string> beneficiariosValidos;
}

/// <summary>Carga Resources/ordenes.json (datos ficticios).</summary>
public static class OrderDatabase
{
    static OrderDatabaseDto cache;

    public static OrderDatabaseDto Load()
    {
        if (cache != null) return cache;
        var txt = Resources.Load<TextAsset>("ordenes");
        cache = txt != null ? JsonUtility.FromJson<OrderDatabaseDto>(txt.text)
                            : new OrderDatabaseDto { ordenes = new List<OrderData>(), beneficiariosValidos = new List<string>() };
        return cache;
    }

    public static OrderData Find(string id)
    {
        foreach (var o in Load().ordenes) if (o.id == id) return o;
        return null;
    }
}
