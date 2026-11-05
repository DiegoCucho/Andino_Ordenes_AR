using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>Botón UI que alterna entre modo Ataque y modo Defensa. Asignar su OnClick a ToggleMode().</summary>
public class ModeToggleButton : MonoBehaviour
{
    public TextMeshProUGUI label;

    void Start()
    {
        var btn = GetComponent<Button>();
        if (btn != null) btn.onClick.AddListener(ToggleMode);
        if (GameManager.I != null) GameManager.I.OnModeChanged += Pintar;
        Pintar();
    }

    void OnDestroy()
    {
        if (GameManager.I != null) GameManager.I.OnModeChanged -= Pintar;
    }

    public void ToggleMode()
    {
        if (GameManager.I != null) GameManager.I.ToggleMode();
    }

    void Pintar()
    {
        if (label == null || GameManager.I == null) return;
        label.text = GameManager.I.mode == SimMode.Ataque ? "Modo: ATAQUE 2016" : "Modo: DEFENSA";
    }
}
