using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Inventario final: totales por estado.
/// Por defecto cuenta las ordenes escaneadas; con "Inventario completo" muestra las 36 del caso real.
/// </summary>
public class InventoryUI : MonoBehaviour
{
    public TextMeshProUGUI txtInventario;
    public bool contarTodas = false;      // true = totales del caso completo (36 ordenes)
    public Button btnCompleto;            // boton opcional para alternar

    static readonly HashSet<string> escaneadas = new HashSet<string>();
    static InventoryUI instancia;

    public static int TotalEscaneadas { get { return escaneadas.Count; } }

    void Awake() { instancia = this; }

    void Start()
    {
        if (txtInventario != null)
        {
            txtInventario.enableAutoSizing = true;
            txtInventario.fontSizeMin = 18f;
            txtInventario.fontSizeMax = 36f;
        }
        if (btnCompleto != null) btnCompleto.onClick.AddListener(ToggleCompleto);
        if (GameManager.I != null) GameManager.I.OnStateChanged += Actualizar;
        Actualizar();
    }

    void OnDestroy()
    {
        if (GameManager.I != null) GameManager.I.OnStateChanged -= Actualizar;
    }

    public static void Registrar(string id)
    {
        escaneadas.Add(id);
        if (instancia != null) instancia.Actualizar();
    }

    public void ToggleCompleto()
    {
        contarTodas = !contarTodas;
        Actualizar();
    }

    public void Actualizar()
    {
        if (txtInventario == null) return;
        var db = OrderDatabase.Load();
        bool defensa = GameManager.I != null && GameManager.I.mode == SimMode.Defensa;

        int bloq = 0, pag = 0, ret = 0, fand = 0, noRe = 0, total = 0;
        float mBloq = 0, mPag = 0, mRet = 0, mFand = 0, mNoRe = 0, mTotal = 0;

        foreach (var o in db.ordenes)
        {
            if (!contarTodas && !escaneadas.Contains(o.id)) continue;
            total++;
            mTotal += o.montoMillones;
            if (defensa) { ret++; mRet += o.montoMillones; continue; }
            switch (o.estadoReal)
            {
                case "bloqueada": bloq++; mBloq += o.montoMillones; break;
                case "pagada": pag++; mPag += o.montoMillones; break;
                case "detenida_fandation": fand++; mFand += o.montoMillones; break;
                default: noRe++; mNoRe += o.montoMillones; break;
            }
        }

        string alcance = contarTodas ? "CASO COMPLETO" : "ESCANEADAS";
        string cuerpo;
        if (defensa)
        {
            cuerpo = "INVENTARIO DEFENSA · " + alcance +
                     "\nOrdenes: " + total + " (US$" + mTotal.ToString("0.#") + " M)" +
                     "\nRetenidas: " + ret + " (US$" + mRet.ToString("0.#") + " M)" +
                     "\nPagadas: 0 · US$0 perdidos";
        }
        else
        {
            cuerpo = "INVENTARIO ATAQUE 2016 · " + alcance +
                     "\nOrdenes: " + total + " (US$" + mTotal.ToString("0.#") + " M)" +
                     "\nBloqueadas: " + bloq + " (US$" + mBloq.ToString("0.#") + " M)" +
                     "\nPagadas: " + pag + " (US$" + mPag.ToString("0.#") + " M)" +
                     "\nDetenida «Fandation»: " + fand + " (US$" + mFand.ToString("0.#") + " M)" +
                     "\nNo reenviada: " + noRe + " (US$" + mNoRe.ToString("0.#") + " M)";
        }
        txtInventario.text = cuerpo + "\n" + OrdenesPanel.ResumenClasificacion();

        if (btnCompleto != null)
        {
            var lbl = btnCompleto.GetComponentInChildren<TextMeshProUGUI>();
            if (lbl != null) lbl.text = contarTodas ? "Ver solo escaneadas" : "Inventario completo (36)";
        }
    }
}
