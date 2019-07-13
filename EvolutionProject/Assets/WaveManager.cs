using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    public static WaveManager Instance;
    public Transform foxObject;

    public FoxWaveTraits currentWaveRet {
        get {
            return foxWaves[Mathf.Clamp(currentWave,0,foxWaves.Count-1)];
        }
    }

    public bool waveIsRunning;
    public int currentWave = -1;

    [Header("Wave Management")]
    public List<FoxWaveTraits> foxWaves = new List<FoxWaveTraits>();
    private List<EntityManager> waveManagedFoxes = new List<EntityManager>();

    private void Awake() {
        if (Instance == null)
            Instance = this;
    }
    
    public Vector3 GetRandomSpawnPoint () {

        SpawnPoint[] spawnPoints = FindObjectsOfType<SpawnPoint>();
        int i = Random.Range(0,spawnPoints.Length);
        if (spawnPoints[i] != null)
            return spawnPoints[i].GetRandomPoint();
        else
            return spawnPoints[0].GetRandomPoint();
    }


    public void CreateNewWave () {

        currentWave ++;
        if (currentWave < 0 || currentWave > foxWaves.Count - 1)
        {Debug.LogError("Something is wrong with the Current Wave");    return;}

        FoxWaveTraits t = foxWaves[currentWave];

        //Spawn with the isPartOfWave = true
        for (int i = 0; i < t.numberOfFoxes; i++) {

            GeneticTraits applyTrait;
            CurrentState applyState;

            (applyTrait, applyState) = StateTraitCreation();

            EntityManager m = GameObject.Instantiate(foxObject.gameObject,GetRandomSpawnPoint(), Quaternion.identity).transform.GetComponent<EntityManager>();
            m.controller.movementCost = t.movementCost;
            m.stateManagement.state = applyState;
            m.traits = applyTrait;
            m.isPartOfWave = true;
            m.stateManagement.maxAge = t.maxAge;
            m.stateManagement.eatPower = t.eatPower;
            m.stateManagement.hungerIncrease = t.hungerIncrease;
            m.stateManagement.sleepPower = t.sleepPower;
            m.stateManagement.reproductiveIncrease = t.reproductiveIncrease;
            m.stateManagement.reproductiveCost = t.reproductiveCost;
            waveManagedFoxes.Add(m);

        }

    }

    private (GeneticTraits, CurrentState) StateTraitCreation() {
        FoxWaveTraits t = foxWaves[currentWave];
        GeneticTraits traits = new GeneticTraits();
        traits.surroundingCheckCooldown = UnityEngine.Random.Range(0.1f,2f);
        
        traits.decisionCoolDown = UnityEngine.Random.Range(0.1f,1f);
        
        traits.speed = t.averageTraits.speed+Random.Range(-t.variation,t.variation);
    
        traits.size = t.averageTraits.size+Random.Range(-t.variation,t.variation);
        
        traits.attractiveness = t.averageTraits.attractiveness+Random.Range(-t.variation,t.variation);
    
        traits.sightRange = t.averageTraits.sightRange+Random.Range(-t.variation,t.variation);
    
        traits.dangerSense = t.averageTraits.dangerSense+Random.Range(-t.variation,t.variation);
    
        traits.strength = t.averageTraits.strength+Random.Range(-t.variation,t.variation);
    
        traits.heatResistance = t.averageTraits.heatResistance+Random.Range(-t.variation,t.variation);
    
        traits.intellect =t.averageTraits.intellect+Random.Range(-t.variation,t.variation);
    
        traits.brute = t.averageTraits.brute+Random.Range(-t.variation,t.variation);
        
        traits.HI = t.averageTraits.HI+Random.Range(-t.variation,t.variation);
    
        traits.AI = t.averageTraits.AI+Random.Range(-t.variation,t.variation);

        traits.FI = t.averageTraits.FI+Random.Range(-t.variation,t.variation);
    
        traits.HUI = t.averageTraits.HUI+Random.Range(-t.variation,t.variation);

        traits.SI = t.averageTraits.SI+Random.Range(-t.variation,t.variation);
    
        traits.RI = t.averageTraits.RI+Random.Range(-t.variation,t.variation);

        traits.TI = t.averageTraits.TI+Random.Range(-t.variation,t.variation);

        return (traits,t.startingState);
    }

    public void KillWave() {

        for (int i = 0; i < waveManagedFoxes.Count; i++) {

            //This should be done in a more smooth fashion
            if (waveManagedFoxes[i] != null)
            GameObject.Destroy(waveManagedFoxes[i].gameObject);

        }
        
        waveManagedFoxes.Clear();
    }

}
