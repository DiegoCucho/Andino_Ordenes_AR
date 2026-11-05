#if UNITY_EDITOR
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Menu: ANDES-SIM > Agregar clasificacion, final y creditos.
/// Crea dentro de Canvas_UI: panel de clasificacion, panel de «Fandation», botones Finalizar / Creditos /
/// Inventario completo, y las pantallas final y de creditos. Conecta OrdenesPanel, CreditosUI e InventoryUI.
/// Ejecutar DESPUES de "Configurar escenario 4". Se puede ejecutar mas de una vez.
/// </summary>
public static class SetupExtrasAR
{
    static readonly Color Oscuro = new Color(0.02f, 0.06f, 0.04f, 0.88f);
    static readonly Color Azul = new Color(0.1f, 0.25f, 0.45f, 0.95f);
    static readonly Color Verde = new Color(0.12f, 0.5f, 0.3f, 0.95f);
    static readonly Color Rojo = new Color(0.6f, 0.18f, 0.15f, 0.95f);
    static readonly Color Celeste = new Color(0.15f, 0.4f, 0.65f, 0.95f);

    [MenuItem("ANDES-SIM/Agregar clasificación, final y créditos")]
    public static void Agregar()
    {
        var canvasGo = GameObject.Find("Canvas_UI");
        if (canvasGo == null)
        {
            Debug.LogError("ANDES-SIM: no existe Canvas_UI. Ejecuta primero ANDES-SIM > Configurar escenario 4.");
            return;
        }
        if (canvasGo.transform.Find("Panel_Clasificar") != null)
        {
            Debug.Log("ANDES-SIM: los extras ya estaban creados.");
            return;
        }
        Transform c = canvasGo.transform;
        var bottom = new Vector2(0.5f, 0f);
        var top = new Vector2(0.5f, 1f);

        // --- Panel de clasificacion (encima del inventario) ---
        var pc = Panel(c, "Panel_Clasificar", bottom, bottom, bottom, new Vector2(0f, 480f), new Vector2(980f, 300f), Oscuro);
        var txtOrden = Texto(pc, "Txt_Orden", "Clasifica la orden", 38, TextAlignmentOptions.Center, Color.white,
            new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(20f, -70f), new Vector2(-20f, -10f));
        var bBloq = Boton(pc, "Btn_Bloqueada", "Bloqueada", bottom, new Vector2(-320f, 100f), new Vector2(290f, 100f), Verde);
        var bPag = Boton(pc, "Btn_Pagada", "Pagada", bottom, new Vector2(0f, 100f), new Vector2(290f, 100f), Rojo);
        var bRet = Boton(pc, "Btn_Retenida", "Retenida", bottom, new Vector2(320f, 100f), new Vector2(290f, 100f), Celeste);
        // los botones quedan dentro del panel: ajustamos a su anclaje inferior
        var txtFeedback = Texto(pc, "Txt_Feedback", "Elige una opción", 36, TextAlignmentOptions.Center, Color.white,
            new Vector2(0f, 0f), new Vector2(1f, 0f), new Vector2(20f, 10f), new Vector2(-20f, 90f));

        // --- Panel de «Fandation» (debajo de los botones superiores) ---
        var pf = Panel(c, "Panel_Fandation", top, top, top, new Vector2(0f, -300f), new Vector2(980f, 560f), new Color(0.25f, 0.17f, 0f, 0.92f));
        var txtExp = Texto(pf, "Txt_Explicacion", "", 32, TextAlignmentOptions.TopLeft, Color.white,
            Vector2.zero, Vector2.one, new Vector2(24f, 20f), new Vector2(-24f, -20f));
        txtExp.enableAutoSizing = true; txtExp.fontSizeMin = 18f; txtExp.fontSizeMax = 32f;

        // --- Botones superiores: Finalizar y Creditos ---
        var bFin = Boton(c, "Btn_Finalizar", "Finalizar", top, new Vector2(-250f, -190f), new Vector2(440f, 90f), Azul);
        var bCred = Boton(c, "Btn_Creditos", "Créditos", top, new Vector2(250f, -190f), new Vector2(440f, 90f), Azul);

        // --- Boton de inventario completo (justo encima del panel de inventario) ---
        var bInv = Boton(c, "Btn_InventarioCompleto", "Inventario completo (36)", bottom, new Vector2(280f, 410f), new Vector2(420f, 70f), Azul);

        // --- Pantalla final (a todo el alto) ---
        var pFinal = Panel(c, "Panel_Final", Vector2.zero, Vector2.one, new Vector2(0.5f, 0.5f), Vector2.zero, Vector2.zero, new Color(0.01f, 0.04f, 0.03f, 0.96f));
        pFinal.offsetMin = new Vector2(40f, 120f); pFinal.offsetMax = new Vector2(-40f, -120f);
        var txtFinal = Texto(pFinal, "Txt_Final", "", 40, TextAlignmentOptions.TopLeft, Color.white,
            Vector2.zero, Vector2.one, new Vector2(40f, 150f), new Vector2(-40f, -40f));
        txtFinal.enableAutoSizing = true; txtFinal.fontSizeMin = 20f; txtFinal.fontSizeMax = 40f;
        var bCerrarFinal = Boton(pFinal, "Btn_CerrarFinal", "Cerrar", bottom, new Vector2(0f, 50f), new Vector2(420f, 90f), Azul);

        // --- Pantalla de creditos ---
        var pCred = Panel(c, "Panel_Creditos", Vector2.zero, Vector2.one, new Vector2(0.5f, 0.5f), Vector2.zero, Vector2.zero, new Color(0.01f, 0.04f, 0.03f, 0.97f));
        pCred.offsetMin = new Vector2(40f, 120f); pCred.offsetMax = new Vector2(-40f, -120f);
        var txtCred = Texto(pCred, "Txt_Creditos", "", 34, TextAlignmentOptions.TopLeft, Color.white,
            Vector2.zero, Vector2.one, new Vector2(40f, 150f), new Vector2(-40f, -40f));
        var bCerrarCred = Boton(pCred, "Btn_CerrarCreditos", "Cerrar", bottom, new Vector2(0f, 50f), new Vector2(420f, 90f), Azul);

        // --- Conectar componentes ---
        var op = canvasGo.GetComponent<OrdenesPanel>();
        if (op == null) op = canvasGo.AddComponent<OrdenesPanel>();
        op.panelClasificar = pc.gameObject;
        op.txtOrden = txtOrden;
        op.txtFeedback = txtFeedback;
        op.btnBloqueada = bBloq; op.btnPagada = bPag; op.btnRetenida = bRet;
        op.panelFandation = pf.gameObject; op.txtExplicacion = txtExp;
        op.btnFinalizar = bFin;
        op.panelFinal = pFinal.gameObject; op.txtFinal = txtFinal; op.btnCerrarFinal = bCerrarFinal;
        EditorUtility.SetDirty(op);

        var cr = canvasGo.GetComponent<CreditosUI>();
        if (cr == null) cr = canvasGo.AddComponent<CreditosUI>();
        cr.panel = pCred.gameObject; cr.txt = txtCred; cr.btnAbrir = bCred; cr.btnCerrar = bCerrarCred;
        EditorUtility.SetDirty(cr);

        var inv = Object.FindFirstObjectByType<InventoryUI>();
        if (inv != null) { inv.btnCompleto = bInv; EditorUtility.SetDirty(inv); }
        else Debug.LogWarning("ANDES-SIM: no se encontró InventoryUI en la escena.");

        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene());
        Debug.Log("ANDES-SIM: extras creados. Guarda la escena (Ctrl+S). Los paneles se ocultan solos al pulsar Play.");
    }

    // ---------- ayudas de UI ----------
    static RectTransform Panel(Transform parent, string nombre, Vector2 aMin, Vector2 aMax, Vector2 pivot,
                               Vector2 pos, Vector2 size, Color color)
    {
        var go = new GameObject(nombre, typeof(RectTransform), typeof(Image));
        go.transform.SetParent(parent, false);
        var rt = (RectTransform)go.transform;
        rt.anchorMin = aMin; rt.anchorMax = aMax; rt.pivot = pivot;
        rt.anchoredPosition = pos; rt.sizeDelta = size;
        go.GetComponent<Image>().color = color;
        return rt;
    }

    static TextMeshProUGUI Texto(Transform parent, string nombre, string texto, int size, TextAlignmentOptions al, Color color,
                                 Vector2 aMin, Vector2 aMax, Vector2 offMin, Vector2 offMax)
    {
        var go = new GameObject(nombre, typeof(RectTransform), typeof(TextMeshProUGUI));
        go.transform.SetParent(parent, false);
        var rt = (RectTransform)go.transform;
        rt.anchorMin = aMin; rt.anchorMax = aMax;
        rt.offsetMin = offMin; rt.offsetMax = offMax;
        var t = go.GetComponent<TextMeshProUGUI>();
        t.text = texto; t.fontSize = size; t.alignment = al; t.color = color;
        t.raycastTarget = false;
        return t;
    }

    static Button Boton(Transform parent, string nombre, string etiqueta, Vector2 anchor, Vector2 pos, Vector2 size, Color color)
    {
        var go = new GameObject(nombre, typeof(RectTransform), typeof(Image), typeof(Button));
        go.transform.SetParent(parent, false);
        var rt = (RectTransform)go.transform;
        rt.anchorMin = rt.anchorMax = anchor; rt.pivot = new Vector2(0.5f, anchor.y >= 0.5f ? 1f : 0f);
        rt.anchoredPosition = pos; rt.sizeDelta = size;
        go.GetComponent<Image>().color = color;
        var lbl = Texto(go.transform, "Label", etiqueta, 32, TextAlignmentOptions.Center, Color.white,
            Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
        lbl.enableAutoSizing = true; lbl.fontSizeMin = 16f; lbl.fontSizeMax = 32f;
        return go.GetComponent<Button>();
    }
}
#endif
