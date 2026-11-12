using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>Pantalla de creditos. Edita el texto en el Inspector (campo Creditos) y escribe los nombres del equipo.</summary>
public class CreditosUI : MonoBehaviour
{
    public GameObject panel;
    public TextMeshProUGUI txt;
    public Button btnAbrir, btnCerrar;

    [TextArea(12, 30)]
    public string creditos =
        "CRÉDITOS\n\n" +
        "ANDES-SIM · Escenario 4: Las 36 órdenes (AR)\n" +
        "Proyecto académico basado en hechos reales (Banco de Bangladesh, 2016). BANCO ANDINO, sus personas, cuentas y datos son ficticios.\n\n" +
        "EQUIPO\n" +
        "KIANU WILLIAM RETAMOZO LAVADO · Líder de proyecto y narrativa\n" +
        "DIEGO FREDY CUCHO RIVERA · Desarrollador Unity (C#)\n" +
        "ANYELO JHERAY VELITA LOZANO · Analista de ciberseguridad y QA\n\n" +
        "HERRAMIENTAS\n" +
        "- Unity 6 (Unity Technologies)\n" +
        "- Vuforia Engine, licencia Basic (PTC Inc.)\n" +
        "- TextMeshPro y fuente Liberation Sans (SIL Open Font License)\n\n" +
        "RECURSOS\n" +
        "- Marcadores y fichas: creados por el equipo con código (Python/Pillow, fuentes DejaVu y Liberation). Sin recursos de terceros.\n\n" +
        "HERRAMIENTAS DE IA USADAS\n" +
        "- Claude (Anthropic): apoyo en el código C#, los datos ficticios y el diseño de los marcadores.";

    void Start()
    {
        if (txt != null)
        {
            txt.text = creditos;
            txt.enableAutoSizing = true;
            txt.fontSizeMin = 18f;
            txt.fontSizeMax = 38f;
        }
        if (btnAbrir != null) btnAbrir.onClick.AddListener(() => { if (panel != null) panel.SetActive(true); });
        if (btnCerrar != null) btnCerrar.onClick.AddListener(() => { if (panel != null) panel.SetActive(false); });
        if (panel != null) panel.SetActive(false);
    }
}
