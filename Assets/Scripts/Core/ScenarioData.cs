using UnityEngine;

/// <summary>Textos y cifras del escenario. Crear con: Assets > Create > ANDES-SIM > ScenarioData.</summary>
[CreateAssetMenu(menuName = "ANDES-SIM/ScenarioData", fileName = "Escenario04_Ordenes")]
public class ScenarioData : ScriptableObject
{
    public int numero = 4;
    public string titulo = "Las 36 órdenes";
    public string fecha = "4 y 5 feb 2016";
    [TextArea(2, 6)] public string historia =
        "Las 36 órdenes viajan al Banco Corresponsal Internacional. 35 son rechazadas por formato y reenviadas; al final se bloquean 30 y pasan 5.";
    public int totalOrdenes = 36;
    public float totalMillones = 951f;
}
