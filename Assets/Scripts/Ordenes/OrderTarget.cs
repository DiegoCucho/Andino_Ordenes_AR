using UnityEngine;
using TMPro;

/// <summary>
/// Va en cada ImageTarget_Orden_XX. Conecta en el DefaultObserverEventHandler de Vuforia:
///   On Target Found  -> OrderTarget.OnFound
///   On Target Lost   -> OrderTarget.OnLost
/// (así este script no depende de Vuforia y compila antes de instalarlo).
/// </summary>
public class OrderTarget : MonoBehaviour
{
    [Tooltip("Ej: ORD-01 ... ORD-36 (debe existir en ordenes.json)")]
    public string orderId = "ORD-01";

    public GameObject ficha;
    public TextMeshPro txtId, txtMonto, txtDestino, txtBeneficiario, txtEstado, txtMotivos;
    public Renderer fichaRenderer;

    public OrderData Data { get; private set; }

    void Start()
    {
        Data = OrderDatabase.Find(orderId);
        if (ficha != null) ficha.SetActive(false);
        if (GameManager.I != null) GameManager.I.OnStateChanged += Refresh;
    }

    void OnDestroy()
    {
        if (GameManager.I != null) GameManager.I.OnStateChanged -= Refresh;
    }

    public void OnFound()
    {
        if (ficha != null) ficha.SetActive(true);
        Refresh();
        InventoryUI.Registrar(orderId);
        if (GameManager.I != null && Data != null && Data.estadoReal == "detenida_fandation")
            GameManager.I.RegisterEvidence("fandation");
    }

    public void OnLost()
    {
        if (ficha != null) ficha.SetActive(false);
    }

    void Refresh()
    {
        if (Data == null) return;
        bool defensa = GameManager.I != null && GameManager.I.mode == SimMode.Defensa;

        string estado = Data.estadoReal;
        string motivos = "";
        if (defensa)
        {
            var r = RuleEngine.Evaluar(Data);
            estado = r.estado;
            motivos = string.Join("\n", r.motivos);
            GameManager.I.ApplyDefense("reglas");
        }

        Set(txtId, Data.id);
        Set(txtMonto, "US$" + Data.montoMillones.ToString("0.##") + " M");
        Set(txtDestino, Data.destino);
        Set(txtBeneficiario, Data.beneficiario);
        Set(txtEstado, estado.ToUpper().Replace('_', ' '));
        Set(txtMotivos, motivos);

        if (fichaRenderer != null)
            fichaRenderer.material.color = ColorEstado(estado);
    }

    static void Set(TextMeshPro t, string s) { if (t != null) t.text = s; }

    public static Color ColorEstado(string e)
    {
        switch (e)
        {
            case "bloqueada": return new Color(0.2f, 0.9f, 0.5f);
            case "pagada": return new Color(1f, 0.35f, 0.3f);
            case "detenida_fandation": return new Color(1f, 0.76f, 0.29f);
            case "RETENIDA": return new Color(0.3f, 0.7f, 1f);
            default: return Color.gray;
        }
    }
}
