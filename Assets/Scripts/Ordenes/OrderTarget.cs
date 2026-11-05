using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/// <summary>
/// Va en cada ImageTarget_Orden_XX. Conecta en el DefaultObserverEventHandler de Vuforia:
///   On Target Found  -> OrderTarget.OnFound
///   On Target Lost   -> OrderTarget.OnLost
/// Al detectar el marcador: muestra la ficha con una animacion de entrada (C#),
/// actualiza el inventario y abre el panel de clasificacion.
/// </summary>
public class OrderTarget : MonoBehaviour
{
    [Tooltip("Ej: ORD-01 ... ORD-36 (debe existir en ordenes.json)")]
    public string orderId = "ORD-01";

    public GameObject ficha;
    public TextMeshPro txtId, txtMonto, txtDestino, txtBeneficiario, txtEstado, txtMotivos;
    public Renderer fichaRenderer;

    public OrderData Data { get; private set; }

    // animacion de entrada
    readonly List<Transform> animados = new List<Transform>();
    readonly List<Vector3> escalas = new List<Vector3>();
    Coroutine anim;

    void Start()
    {
        Data = OrderDatabase.Find(orderId);
        if (ficha != null) ficha.SetActive(false);

        Registrar(ficha != null ? ficha.transform : null);
        Registrar(txtId); Registrar(txtMonto); Registrar(txtDestino);
        Registrar(txtBeneficiario); Registrar(txtEstado); Registrar(txtMotivos);

        if (GameManager.I != null) GameManager.I.OnStateChanged += Refresh;
    }

    void OnDestroy()
    {
        if (GameManager.I != null) GameManager.I.OnStateChanged -= Refresh;
    }

    void Registrar(Component c) { if (c != null) Registrar(c.transform); }
    void Registrar(Transform t)
    {
        if (t == null) return;
        animados.Add(t);
        escalas.Add(t.localScale);
    }

    public void OnFound()
    {
        if (ficha != null) ficha.SetActive(true);
        Refresh();
        InventoryUI.Registrar(orderId);

        if (GameManager.I != null && Data != null && Data.estadoReal == "detenida_fandation")
            GameManager.I.RegisterEvidence("fandation");

        OrdenesPanel.MostrarOrden(this);

        if (anim != null) StopCoroutine(anim);
        anim = StartCoroutine(PopIn());
    }

    public void OnLost()
    {
        if (anim != null) { StopCoroutine(anim); anim = null; }
        Restaurar();
        if (ficha != null) ficha.SetActive(false);
        OrdenesPanel.OcultarOrden(this);
    }

    // Animacion: cada pieza de la ficha "salta" desde escala 0 con un pequeno rebote, una tras otra.
    IEnumerator PopIn()
    {
        for (int i = 0; i < animados.Count; i++)
            if (animados[i] != null) animados[i].localScale = Vector3.zero;

        const float dur = 0.45f, delay = 0.07f;
        float total = dur + delay * Mathf.Max(0, animados.Count - 1);
        float t = 0f;
        while (t < total)
        {
            t += Time.deltaTime;
            for (int i = 0; i < animados.Count; i++)
            {
                if (animados[i] == null) continue;
                float k = Mathf.Clamp01((t - delay * i) / dur);
                animados[i].localScale = escalas[i] * EaseOutBack(k);
            }
            yield return null;
        }
        Restaurar();
        anim = null;
    }

    void Restaurar()
    {
        for (int i = 0; i < animados.Count; i++)
            if (animados[i] != null) animados[i].localScale = escalas[i];
    }

    static float EaseOutBack(float x)
    {
        const float c1 = 1.70158f, c3 = c1 + 1f;
        return 1f + c3 * Mathf.Pow(x - 1f, 3f) + c1 * Mathf.Pow(x - 1f, 2f);
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
