using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    public static WaveManager Instance;

    public bool waveIsRunning;
    public int currentWave = -1;

    private void Awake() {
        if (Instance == null)
            Instance = this;
    }




}
