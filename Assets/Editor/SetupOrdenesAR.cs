#if UNITY_EDITOR
using TMPro;
using UnityEditor;
using UnityEditor.Events;
using UnityEngine;
using UnityEngine.UI;
using Vuforia;

/// <summary>
/// Menu: ANDES-SIM > Configurar escenario 4.
/// Crea la ficha 3D en cada ImageTarget_Orden_XX, agrega OrderTarget, conecta los eventos de Vuforia
/// y crea el Canvas con el boton de modo y el panel de inventario.
/// Antes: Window > TextMeshPro > Import TMP Essential Resources.
/// Se puede ejecutar mas de una vez: omite los targets que ya tienen ficha.
/// </summary>
public static class SetupOrdenesAR
{
    [MenuItem("ANDES-SIM/Configurar escenario 4")]
    public static void Configurar()
    {
        int n = 0;
        foreach (var go in Object.FindObjectsByType<GameObject>(FindObjectsSortMode.None))
        {
            if (!go.name.StartsWith("ImageTarget_Orden_")) continue;
            if (go.transform.Find("Ficha") != null) continue;
            string num = go.name.Substring("ImageTarget_Orden_".Length);
            ConfigurarTarget(go, "ORD-" + num);
            n++;
        }

        CrearCanvas();
        EditorUtility.SetDirty(Object.FindFirstObjectByType<GameManager>());
        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene());
        Debug.Log("ANDES-SIM: " + n + " ImageTargets configurados. Guarda la escena (Ctrl+S).");
    }

    static void ConfigurarTarget(GameObject target, string orderId)
    {
        // Ficha: un cubo plano, hijo del target (1 unidad = ancho del marcador)
        var ficha = GameObject.CreatePrimitive(PrimitiveType.Cube);
        ficha.name = "Ficha";
        Object.DestroyImmediate(ficha.GetComponent<Collider>());
        ficha.transform.SetParent(target.transform, false);
        ficha.transform.localPosition = new Vector3(0f, 0.04f, 0f);
        ficha.transform.localScale = new Vector3(0.9f, 0.06f, 0.65f);

        var ot = target.AddComponent<OrderTarget>();
        ot.orderId = orderId;
        ot.ficha = ficha;
        ot.fichaRenderer = ficha.GetComponent<Renderer>();

        ot.txtId = Texto(target.transform, "txtId", orderId, new Vector3(0f, 0.075f, 0.26f), 0.14f, FontStyles.Bold);
        ot.txtMonto = Texto(target.transform, "txtMonto", "", new Vector3(0f, 0.075f, 0.15f), 0.12f, FontStyles.Bold);
        ot.txtDestino = Texto(target.transform, "txtDestino", "", new Vector3(0f, 0.075f, 0.04f), 0.09f, FontStyles.Normal);
        ot.txtBeneficiario = Texto(target.transform, "txtBeneficiario", "", new Vector3(0f, 0.075f, -0.07f), 0.08f, FontStyles.Normal);
        ot.txtEstado = Texto(target.transform, "txtEstado", "", new Vector3(0f, 0.075f, -0.18f), 0.11f, FontStyles.Bold);
        ot.txtMotivos = Texto(target.transform, "txtMotivos", "", new Vector3(0f, 0.075f, -0.30f), 0.06f, FontStyles.Normal);
        EditorUtility.SetDirty(ot);

        // Eventos de Vuforia -> OrderTarget
        var h = target.GetComponent<DefaultObserverEventHandler>();
        if (h == null) h = target.AddComponent<DefaultObserverEventHandler>();
        UnityEventTools.AddPersistentListener(h.OnTargetFound, ot.OnFound);
        UnityEventTools.AddPersistentListener(h.OnTargetLost, ot.OnLost);
        EditorUtility.SetDirty(h);
    }

    static TextMeshPro Texto(Transform parent, string nombre, string texto, Vector3 pos, float size, FontStyles style)
    {
        var go = new GameObject(nombre, typeof(TextMeshPro));
        go.transform.SetParent(parent, false);
        go.transform.localPosition = pos;                       // sobre la cara superior de la ficha
        go.transform.localRotation = Quaternion.Euler(90f, 0f, 0f);
        var t = go.GetComponent<TextMeshPro>();
        t.text = texto;
        t.fontSize = size * 10f;
        t.fontStyle = style;
        t.alignment = TextAlignmentOptions.Center;
        t.color = Color.black;
        t.textWrappingMode = TextWrappingModes.NoWrap;
        t.rectTransform.sizeDelta = new Vector2(1f, 0.2f);
        return t;
    }

    static void CrearCanvas()
    {
        if (Object.FindFirstObjectByType<ModeToggleButton>() != null) return;

        var canvasGo = new GameObject("Canvas_UI", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
        var canvas = canvasGo.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        var scaler = canvasGo.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1080, 1920);

        // Boton de modo (arriba)
        var btnGo = new GameObject("Boton_Modo", typeof(Image), typeof(Button), typeof(ModeToggleButton));
        btnGo.transform.SetParent(canvasGo.transform, false);
        var brt = (RectTransform)btnGo.transform;
        brt.anchorMin = brt.anchorMax = new Vector2(0.5f, 1f);
        brt.pivot = new Vector2(0.5f, 1f);
        brt.anchoredPosition = new Vector2(0f, -40f);
        brt.sizeDelta = new Vector2(700f, 130f);
        btnGo.GetComponent<Image>().color = new Color(0.1f, 0.2f, 0.35f, 0.9f);

        var lblGo = new GameObject("Label", typeof(TextMeshProUGUI));
        lblGo.transform.SetParent(btnGo.transform, false);
        var lrt = (RectTransform)lblGo.transform;
        lrt.anchorMin = Vector2.zero; lrt.anchorMax = Vector2.one;
        lrt.offsetMin = lrt.offsetMax = Vector2.zero;
        var lbl = lblGo.GetComponent<TextMeshProUGUI>();
        lbl.text = "Modo: ATAQUE 2016";
        lbl.fontSize = 48; lbl.alignment = TextAlignmentOptions.Center; lbl.color = Color.white;
        btnGo.GetComponent<ModeToggleButton>().label = lbl;

        // Panel de inventario (abajo)
        var panel = new GameObject("Panel_Inventario", typeof(Image), typeof(InventoryUI));
        panel.transform.SetParent(canvasGo.transform, false);
        var prt = (RectTransform)panel.transform;
        prt.anchorMin = prt.anchorMax = new Vector2(0.5f, 0f);
        prt.pivot = new Vector2(0.5f, 0f);
        prt.anchoredPosition = new Vector2(0f, 40f);
        prt.sizeDelta = new Vector2(980f, 360f);
        panel.GetComponent<Image>().color = new Color(0f, 0f, 0f, 0.65f);

        var txtGo = new GameObject("Texto_Inventario", typeof(TextMeshProUGUI));
        txtGo.transform.SetParent(panel.transform, false);
        var trt = (RectTransform)txtGo.transform;
        trt.anchorMin = Vector2.zero; trt.anchorMax = Vector2.one;
        trt.offsetMin = new Vector2(24f, 16f); trt.offsetMax = new Vector2(-24f, -16f);
        var txt = txtGo.GetComponent<TextMeshProUGUI>();
        txt.fontSize = 36; txt.color = new Color(0.3f, 1f, 0.6f); txt.alignment = TextAlignmentOptions.TopLeft;
        panel.GetComponent<InventoryUI>().txtInventario = txt;

        // EventSystem (Input System nuevo)
        if (Object.FindFirstObjectByType<UnityEngine.EventSystems.EventSystem>() == null)
        {
            new GameObject("EventSystem", typeof(UnityEngine.EventSystems.EventSystem),
                typeof(UnityEngine.InputSystem.UI.InputSystemUIInputModule));
        }
    }
}
#endif
