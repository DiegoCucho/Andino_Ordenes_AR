using System;
using System.Collections.Generic;
using UnityEngine;

public enum SimMode { Ataque, Defensa }

/// <summary>Contrato común a todos los escenarios de ANDES-SIM.</summary>
public class GameManager : MonoBehaviour
{
    public static GameManager I;

    public ScenarioData data;
    public SimMode mode = SimMode.Ataque;

    public event Action OnModeChanged;
    public event Action OnStateChanged;

    readonly HashSet<string> evidencias = new HashSet<string>();
    readonly HashSet<string> defensas = new HashSet<string>();

    public int EvidenceCount => evidencias.Count;
    public int DefenseCount => defensas.Count;

    void Awake()
    {
        if (I != null && I != this) { Destroy(gameObject); return; }
        I = this;
    }

    public void SetMode(SimMode m)
    {
        mode = m;
        OnModeChanged?.Invoke();
        OnStateChanged?.Invoke();
    }

    public void ToggleMode() => SetMode(mode == SimMode.Ataque ? SimMode.Defensa : SimMode.Ataque);

    public void RegisterEvidence(string id)
    {
        if (evidencias.Add(id)) OnStateChanged?.Invoke();
    }

    public void ApplyDefense(string id)
    {
        if (defensas.Add(id)) OnStateChanged?.Invoke();
    }

    public void EndScenario()
    {
        string resultado = mode == SimMode.Defensa
            ? "DEFENSA: las 36 órdenes quedan retenidas. US$0 perdidos."
            : "ATAQUE 2016: US$101 millones pagados.";
        Debug.Log(resultado);
        OnStateChanged?.Invoke();
    }
}
