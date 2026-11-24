using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Audio y fases dentro de la app (se crea solo al iniciar, no hay que ponerlo en la escena).
///  - Musica de fondo en bucle (Resources/Audio/musica) que baja de volumen mientras habla el narrador.
///  - Narrador por fase: ataque, analisis («Fandation») y defensa (Resources/Audio/ataque, analisis, defensa).
///  - Cartel que indica la fase en pantalla: FASE 1 ATAQUE, FASE 2 ANALISIS, FASE 3 DEFENSA.
/// Los clips son opcionales: si falta alguno, la app funciona igual y solo muestra el cartel.
/// </summary>
public class NarradorFases : MonoBehaviour
{
    const float VolMusica = 0.35f;
    const float VolMusicaHablando = 0.12f;
    const float SegundosCartel = 4f;
    const float EsperaRepetir = 25f;

    static NarradorFases inst;
    AudioSource musica, voz;
    GameObject cartel;
    TextMeshProUGUI txtCartel;
    Coroutine ocultar;
    readonly Dictionary<string, float> ultimaVez = new Dictionary<string, float>();

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void Crear()
    {
        if (inst == null) new GameObject("NarradorFases").AddComponent<NarradorFases>();
    }

    void Awake() { inst = this; }

    void Start()
    {
        musica = gameObject.AddComponent<AudioSource>();
        musica.loop = true;
        musica.playOnAwake = false;
        musica.volume = VolMusica;
        var m = Resources.Load<AudioClip>("Audio/musica");
        if (m != null) { musica.clip = m; musica.Play(); }

        voz = gameObject.AddComponent<AudioSource>();
        voz.playOnAwake = false;
        voz.volume = 1f;

        if (GameManager.I != null) GameManager.I.OnModeChanged += CambioDeModo;
        MostrarFase(false);   // fase 1 al abrir la app
    }

    void OnDestroy()
    {
        if (GameManager.I != null) GameManager.I.OnModeChanged -= CambioDeModo;
    }

    void Update()
    {
        if (musica == null || voz == null) return;
        float objetivo = voz.isPlaying ? VolMusicaHablando : VolMusica;
        musica.volume = Mathf.MoveTowards(musica.volume, objetivo, Time.deltaTime * 0.6f);
    }

    // ---------- eventos ----------
    void CambioDeModo()
    {
        MostrarFase(GameManager.I != null && GameManager.I.mode == SimMode.Defensa);
    }

    /// <summary>Lo llama OrderTarget al detectar un marcador.</summary>
    public static void OrdenDetectada(OrderData o)
    {
        if (inst == null || o == null) return;
        if (o.estadoReal == "detenida_fandation")
        {
            inst.Hablar("analisis", "FASE 2 · ANÁLISIS\nValidación que detuvo «Fandation»", new Color(1f, 0.75f, 0.2f));
        }
        else
        {
            // audio opcional por orden: Resources/Audio/ORD-01, ORD-02...
            inst.Hablar(o.id, null, Color.white);
        }
    }

    void MostrarFase(bool defensa)
    {
        if (defensa)
            Hablar("defensa", "FASE 3 · DEFENSA\nLas 36 órdenes quedan retenidas", new Color(0.35f, 0.7f, 1f));
        else
            Hablar("ataque", "FASE 1 · ATAQUE 2016\nLas 36 órdenes del robo", new Color(1f, 0.45f, 0.4f));
    }

    // ---------- narrador + cartel ----------
    void Hablar(string clip, string textoCartel, Color color)
    {
        float t;
        if (ultimaVez.TryGetValue(clip, out t) && Time.time - t < EsperaRepetir) return;
        ultimaVez[clip] = Time.time;

        if (textoCartel != null) Cartel(textoCartel, color);

        var c = Resources.Load<AudioClip>("Audio/" + clip);
        if (c != null && voz != null)
        {
            voz.Stop();
            voz.clip = c;
            voz.Play();
        }
    }

    void Cartel(string texto, Color color)
    {
        if (cartel == null && !CrearCartel()) return;
        txtCartel.text = texto;
        txtCartel.color = color;
        cartel.SetActive(true);
        if (ocultar != null) StopCoroutine(ocultar);
        ocultar = StartCoroutine(OcultarLuego());
    }

    IEnumerator OcultarLuego()
    {
        yield return new WaitForSeconds(SegundosCartel);
        if (cartel != null) cartel.SetActive(false);
    }

    bool CrearCartel()
    {
        var canvas = FindFirstObjectByType<Canvas>();
        if (canvas == null) return false;

        cartel = new GameObject("Cartel_Fase", typeof(RectTransform), typeof(Image));
        cartel.transform.SetParent(canvas.transform, false);
        var rt = (RectTransform)cartel.transform;
        rt.anchorMin = rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.anchoredPosition = new Vector2(0f, 330f);
        rt.sizeDelta = new Vector2(960f, 210f);
        var img = cartel.GetComponent<Image>();
        img.color = new Color(0.01f, 0.04f, 0.03f, 0.88f);
        img.raycastTarget = false;

        var go = new GameObject("Txt_Fase", typeof(RectTransform), typeof(TextMeshProUGUI));
        go.transform.SetParent(cartel.transform, false);
        var trt = (RectTransform)go.transform;
        trt.anchorMin = Vector2.zero; trt.anchorMax = Vector2.one;
        trt.offsetMin = new Vector2(20f, 10f); trt.offsetMax = new Vector2(-20f, -10f);
        txtCartel = go.GetComponent<TextMeshProUGUI>();
        txtCartel.alignment = TextAlignmentOptions.Center;
        txtCartel.fontStyle = FontStyles.Bold;
        txtCartel.enableAutoSizing = true;
        txtCartel.fontSizeMin = 24f;
        txtCartel.fontSizeMax = 56f;
        txtCartel.raycastTarget = false;
        return true;
    }
}
