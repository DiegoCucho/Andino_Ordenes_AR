using System.Collections.Generic;
using UnityEngine;
using TMPro;

/// <summary>Inventario final: totales por estado de las órdenes escaneadas (o de las 36 con "mostrar todas").</summary>
public class InventoryUI : MonoBehaviour
{
    public TextMeshProUGUI txtInventario;
    public bool contarTodas = false;      // true = muestra el total del caso completo

    static readonly HashSet<string> escaneadas = new HashSet<string>();
    static InventoryUI instancia;

    void Awake() { instancia = this; }

    void Start()
    {
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

    public void Actualizar()
    {
        if (txtInventario == null) return;
        var db = OrderDatabase.Load();
        bool defensa = GameManager.I != null && GameManager.I.mode == SimMode.Defensa;

        int bloq = 0, pag = 0, ret = 0, fand = 0, noRe = 0, total = 0;
        float mBloq = 0, mPag = 0, mRet = 0;

        foreach (var o in db.ordenes)
        {
            if (!contarTodas && !escaneadas.Contains(o.id)) continue;
            total++;
            if (defensa) { ret++; mRet += o.montoMillones; continue; }
            switch (o.estadoReal)
            {
                case "bloqueada": bloq++; mBloq += o.montoMillones; break;
                case "pagada": pag++; mPag += o.montoMillones; break;
                case "detenida_fandation": fand++; mRet += o.montoMillones; break;
                default: noRe++; break;
            }
        }

        txtInventario.text = defensa
            ? "INVENTARIO · MODO DEFENSA\nÓrdenes revisadas: " + total + "\nRetenidas: " + ret + " (US$" + mRet.ToString("0.#") + " M)\nPagadas: 0 · US$0 perdidos"
            : "INVENTARIO · ATAQUE 2016\nÓrdenes revisadas: " + total +
              "\nBloqueadas: " + bloq + " (US$" + mBloq.ToString("0.#") + " M)" +
              "\nPagadas: " + pag + " (US$" + mPag.ToString("0.#") + " M)" +
              "\nDetenida «Fandation»: " + fand +
              "\nNo reenviada: " + noRe;
    }
}
