using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestFunction : MonoBehaviour
{
    public float testKillInterval = 1.0f;
    public int totalKillstoSimulate = 5;
    private int currentKillCount = 0;

    // Start is called before the first frame update
    void Start()
    {
        StartKillSimulation();
    }

    void StartKillSimulation()
    {
        currentKillCount = 0;
        InvokeRepeating(nameof(SimulateKill), 0.0f, testKillInterval);
    }

    void SimulateKill()
    {
        if (currentKillCount >= totalKillstoSimulate)
        {
            CancelInvoke(nameof(SimulateKill));
            Debug.Log("Kill simulation complete");
            return;
        }
        ScoreManager.instance.RegisterKill();
        Debug.Log("Kill Registered");

        currentKillCount++;
    }
}