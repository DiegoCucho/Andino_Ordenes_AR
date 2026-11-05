using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Interfaz del jugador para el escenario 4:
///  - RF-02: clasificar la orden escaneada (bloqueada, pagada o retenida) con retroalimentacion.
///  - RF-03: panel que explica que validacion detuvo la orden «Fandation».
///  - Pantalla final con el resultado y resumen (EndScenario).
/// Se crea y conecta con el menu ANDES-SIM > Agregar clasificacion, final y creditos.
/// </summary>
public class OrdenesPanel : MonoBehaviour
{
    [Header("Clasificar")]
    public GameObject panelClasificar;
    public TextMeshProUGUI txtOrden;
    public TextMeshProUGUI txtFeedback;
    public Button btnBloqueada, btnPagada, btnRetenida;

    [Header("Fandation")]
    public GameObject panelFandation;
    public TextMeshProUGUI txtExplicacion;

    [Header("Final")]
    public Button btnFinalizar;
    public GameObject panelFinal;
    public TextMeshProUGUI txtFinal;
    public Button btnCerrarFinal;

    static OrdenesPanel instancia;
    // clave "Ataque:ORD-01" -> opcion elegida
    static readonly Dictionary<string, string> elegidas = new Dictionary<string, string>();
    OrderTarget actual;

    void Awake() { instancia = this; }

    void Start()
    {
        if (btnBloqueada != null) btnBloqueada.onClick.AddListener(() => Clasificar("bloqueada"));
        if (btnPagada != null) btnPagada.onClick.AddListener(() => Clasificar("pagada"));
        if (btnRetenida != null) btnRetenida.onClick.AddListener(() => Clasificar("retenida"));
        if (btnFinalizar != null) btnFinalizar.onClick.AddListener(MostrarFinal);
        if (btnCerrarFinal != null) btnCerrarFinal.onClick.AddListener(() => { if (panelFinal != null) panelFinal.SetActive(false); });

        if (panelClasificar != null) panelClasificar.SetActive(false);
        if (panelFandation != null) panelFandation.SetActive(false);
        if (panelFinal != null) panelFinal.SetActive(false);

        if (GameManager.I != null) GameManager.I.OnModeChanged += Refrescar;
    }

    void OnDestroy()
    {
        if (GameManager.I != null) GameManager.I.OnModeChanged -= Refrescar;
    }

    // ---------- llamadas desde OrderTarget ----------
    public static void MostrarOrden(OrderTarget t)
    {
        if (instancia != null) instancia.Mostrar(t);
    }

    public static void OcultarOrden(OrderTarget t)
    {
        if (instancia != null && instancia.actual == t) instancia.Ocultar();
    }

    void Mostrar(OrderTarget t)
    {
        actual = t;
        if (panelClasificar != null) panelClasificar.SetActive(true);
        Refrescar();
    }

    void Ocultar()
    {
        actual = null;
        if (panelClasificar != null) panelClasificar.SetActive(false);
        if (panelFandation != null) panelFandation.SetActive(false);
    }

    // ---------- clasificacion ----------
    static bool EnDefensa() { return GameManager.I != null && GameManager.I.mode == SimMode.Defensa; }
    static string ModoClave() { return EnDefensa() ? "Defensa" : "Ataque"; }

    /// <summary>Respuesta correcta: en ataque depende del estado real; en defensa todas quedan retenidas.</summary>
    static string Correcta(OrderData o, bool defensa)
    {
        if (defensa) return "retenida";
        switch (o.estadoReal)
        {
            case "bloqueada": return "bloqueada";
            case "pagada": return "pagada";
            default: return "retenida";   // «Fandation» detenida y orden nunca reenviada
        }
    }

    void Clasificar(string opcion)
    {
        if (actual == null || actual.Data == null) return;
        elegidas[ModoClave() + ":" + actual.Data.id] = opcion;
        if (GameManager.I != null) GameManager.I.RegisterEvidence("clas_" + actual.Data.id);
        Refrescar();
        var inv = FindFirstObjectByType<InventoryUI>();
        if (inv != null) inv.Actualizar();
    }

    void Refrescar()
    {
        if (actual == null || actual.Data == null) return;
        var o = actual.Data;
        bool defensa = EnDefensa();

        if (txtOrden != null)
            txtOrden.text = "Clasifica " + o.id + "  (" + (defensa ? "modo defensa" : "ataque 2016") + ")";

        string clave = ModoClave() + ":" + o.id;
        if (txtFeedback != null)
        {
            string elegida;
            if (elegidas.TryGetValue(clave, out elegida))
            {
                string ok = Correcta(o, defensa);
                if (elegida == ok)
                {
                    txtFeedback.text = "CORRECTO: " + Nombre(ok);
                    txtFeedback.color = new Color(0.3f, 1f, 0.6f);
                }
                else
                {
                    txtFeedback.text = "INCORRECTO. Era: " + Nombre(ok);
                    txtFeedback.color = new Color(1f, 0.4f, 0.35f);
                }
            }
            else
            {
                txtFeedback.text = "Elige una opcion";
                txtFeedback.color = Color.white;
            }
        }

        // RF-03: explicacion de «Fandation»
        bool esFandation = o.estadoReal == "detenida_fandation";
        if (panelFandation != null) panelFandation.SetActive(esFandation);
        if (esFandation && txtExplicacion != null) txtExplicacion.text = Explicar(o, defensa);
    }

    static string Nombre(string op)
    {
        switch (op)
        {
            case "bloqueada": return "BLOQUEADA";
            case "pagada": return "PAGADA";
            default: return "RETENIDA";
        }
    }

    string Explicar(OrderData o, bool defensa)
    {
        var db = OrderDatabase.Load();
        var sb = new StringBuilder();
        sb.AppendLine("VALIDACIÓN QUE DETUVO LA ORDEN");
        sb.AppendLine();

        string sug;
        if (TypoDetector.EsSospechoso(o.beneficiario, db.beneficiariosValidos, out sug))
        {
            int d = TypoDetector.Levenshtein(o.beneficiario.ToUpperInvariant(), sug.ToUpperInvariant());
            sb.AppendLine("- Nombre del beneficiario: «" + o.beneficiario + "» no coincide con «" + sug + "» (distancia de edición: " + d + ").");
        }
        sb.AppendLine("- Monto de US$" + o.montoMillones.ToString("0.#") + " M para una fundación pequeña: inusual para el destino (" + o.destino + ").");
        if (o.beneficiarioNuevo) sb.AppendLine("- Beneficiario nunca usado antes por el banco.");
        sb.AppendLine();

        if (defensa)
            sb.Append("Con la defensa, un validador automático aplica estas mismas señales a las 36 órdenes y todas quedan retenidas.");
        else
            sb.Append("En 2016 la analista del banco intermediario lo notó a mano, pidió aclaración y detuvo el pago. Los US$20 M se recuperaron completos, pero las demás órdenes no tuvieron esa revisión.");
        return sb.ToString();
    }

    // ---------- resumen de aciertos (lo usa InventoryUI) ----------
    public static string ResumenClasificacion()
    {
        int n = 0, ok = 0;
        foreach (var kv in elegidas)
        {
            string[] partes = kv.Key.Split(':');
            if (partes.Length != 2) continue;
            var o = OrderDatabase.Find(partes[1]);
            if (o == null) continue;
            n++;
            if (kv.Value == Correcta(o, partes[0] == "Defensa")) ok++;
        }
        return "Clasificación: " + ok + " aciertos de " + n;
    }

    // ---------- pantalla final ----------
    public void MostrarFinal()
    {
        if (GameManager.I != null) GameManager.I.EndScenario();
        if (panelFinal == null || txtFinal == null) return;

        var sb = new StringBuilder();
        if (EnDefensa())
        {
            sb.AppendLine("RESULTADO · MODO DEFENSA");
            sb.AppendLine();
            sb.AppendLine("Las 36 órdenes quedan retenidas por límites, horario, beneficiario y doble aprobación.");
            sb.AppendLine("Pagadas: 0 · Perdidos: US$0");
        }
        else
        {
            sb.AppendLine("RESULTADO · ATAQUE 2016");
            sb.AppendLine();
            sb.AppendLine("36 órdenes por US$951 M. Se bloquean 30; pasan 5.");
            sb.AppendLine("Pagadas: 4 a Manila (US$81 M). Detenida: «Fandation» (US$20 M).");
            sb.AppendLine("Salieron US$101 M · Recuperados US$35 M · Perdidos US$66 M");
        }
        sb.AppendLine();
        sb.AppendLine("Órdenes escaneadas: " + InventoryUI.TotalEscaneadas);
        sb.AppendLine(ResumenClasificacion());
        if (GameManager.I != null)
            sb.AppendLine("Evidencias: " + GameManager.I.EvidenceCount + " · Defensas aplicadas: " + GameManager.I.DefenseCount);

        txtFinal.text = sb.ToString();
        panelFinal.SetActive(true);
    }
}
