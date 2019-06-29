using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Reflection;

public class GeneticEntity : MonoBehaviour
{
   #region  Variables

    //References
    [Header("External References")]
    public MapManager manager;
    public GeneticController controller;

    [Header("Internal References")]
    public GeneticTraits traits;
    public CurrentState state;
    public Brain brain;

    //Private Bools
    [Header("Program State")]
    public float fitness;
    public bool currentlyPerformingAction;
    public float timeSinceAction;

    //Sensory Variables
    public List<GeneticEntity> enemies = new List<GeneticEntity>();
    public List<GeneticEntity> friends = new List<GeneticEntity>();
    public List<Transform> food = new List<Transform>();
    #endregion

    #region  Default Methods

    private void Start() {
        //Just start to raom
        Roam();
        controller.FOVChecker(traits.surroundingCheckCooldown,traits.sightRange);
        timerSense = traits.surroundingCheckCooldown;
        timerEnabled = true;
    }

    float timerSense = 0f;
    bool timerEnabled = false;
    private void Update() {
        if (timerEnabled == true) {
            if (timerSense > 0) {
                timerSense -= Time.deltaTime;
            }
            else if (timerSense <= 0) {
                controller.FOVChecker(traits.surroundingCheckCooldown,traits.sightRange);
                timerSense = traits.surroundingCheckCooldown;
            }
        }

        if (state.health > 0) {
            StateUpdate();
        }

    }

    #endregion

    #region  Actions
    
    public void Fight(GE e) {

    }

    private void OverrideAction() {
        currentlyPerformingAction = true;
    }
    
    private void Death() {
        print("death");
    }

    //Energy Boost, Hunger Reduction + Health
    public void Eat(Transform food) {

    }

    public void Sleep() {

    }

    public void Nothing() {

    }

    public void Roam() {
        Vector3 p = manager.GetRandomPointAwayFrom(transform.position, traits.sightRange);
        controller.MoveTo(p,traits.speed/2,"roamCompleted",0);

        currentlyPerformingAction = true;
        timeSinceAction = 0;
    }

    public void Flight(Vector3 position) {
        Vector3 p = manager.GetRandomPointAwayFrom(position, traits.sightRange);
        controller.MoveTo(p,traits.speed,"flightCompleted",0);

        currentlyPerformingAction = true;
        timeSinceAction = 0;
    }

    public void CompletedAction (string actionType) {
        
        if (actionType == "flightCompleted") {
            currentlyPerformingAction = false;
        }
        else if (actionType == "roamCompleted") {

            //TODO: If nothing is in sight range (this is important)
            Roam();
            currentlyPerformingAction = false;
        }
    }

    #endregion

    #region  Energy Calculations
    public float EnergyMovementCalculation (float movementSpeed) {
        //NOTE: This is a temporary function        
        return movementSpeed*traits.size+state.age;
    }

    public float EnergyMovementCalculation (float movementSpeed, float d) {
        //NOTE: This is a temporary function        
        return d*(movementSpeed*traits.size-Mathf.Pow(state.age,2));
    }
    #endregion

    #region  State Management

    private void StateUpdate() {
        //TODO: These are really simple at the moment and more complex behaviours will have to emerge
        
        state.energy = Mathf.Clamp(state.energy,0f,100f);
        
        state.age += 0.2f * Time.deltaTime;

        if (state.age >= 100) {
            Death();
        }
        
        if (enemies != null)
        state.fear = Mathf.Clamp(enemies.Count*10 - traits.strength/2 + traits.dangerSense/3,0f,100f);
        
        if (state.energy <= 50) {
            state.sleepiness = Mathf.Clamp(state.sleepiness+2*Time.deltaTime,0f,100f);
        }

        if (state.age <= 50 && state.age >= 25) {
            state.reproductiveness += 3 * Time.deltaTime;
        }
        else if (state.age >= 50 && state.age <= 75) {
            state.reproductiveness -= 3 * Time.deltaTime;
        }
        

        state.hunger = Mathf.Clamp(state.hunger + (Time.deltaTime*0.2f*traits.size),0f,100f);

        StateActionConversion();
    }

    private void StateActionConversion() {

        if (state.hunger >= 90) {
            //Force eat (until it finds food reduce health and energy)
            state.health -= 2 * Time.deltaTime;
            state.energy -= 2 * Time.deltaTime;
        }
        
        if (state.sleepiness >= 90) {
            state.sleepiness = 100;
            //Force sleep (until it finds a place to sleep & settles in reduce health)
            state.health -= 2 * Time.deltaTime;
            state.energy -= 2 * Time.deltaTime;
        }

        if (state.reproductiveness >= 100) {
            //Force reproduction
        }

        if (state.energy <= 5) {
            state.health -= 2 * Time.deltaTime;
        }

        if (state.health <= 0) {
            Death();
        }

        // help.Plot(t,state.age,"Age");
        // help.Plot(t,state.energy,"Energy");
        // help.Plot(t, state.fear,"Fear");
        // help.Plot(t, state.sleepiness,"Sleepiness");
        // help.Plot(t, state.hunger,"Hunger");
        // help.Plot(t, state.reproductiveness,"Reproductive Urge");

    }
    
    #endregion

    #region  Sensory Control


    public void SensoryUpdate(List<GeneticEntity> enemies, List<Transform> food, List<GeneticEntity> friends) {
        if (currentlyPerformingAction) {
    
            // this.enemies = enemies;
            // this.food = food;
            // this.friends = friends;
            // //return;
        }
        //Do the network code here
        // print ("Detected : " + enemies.Count + " enemies, " + friends.Count + " friends, " + food.Count + " items!");
        this.enemies = enemies;
        this.food = food;
        this.friends = friends;
    }

    #endregion

    #region  Other

    public void Randomise() {
        for (int i = 0; i < brain.tags.Count; i++) {
            SenseNetwork net = brain.correspondingNetwork[i];
            net.network.Initialise(2,10,6,net.possibleActions.Count);
        }

        traits.surroundingCheckCooldown = UnityEngine.Random.Range(0.3f,2f);
        
        traits.decisionCoolDown = UnityEngine.Random.Range(0.5f,5f);
        
        traits.speed = UnityEngine.Random.Range(0.01f,10f);
    
        traits.size = UnityEngine.Random.Range(0.01f,5f);
        
        traits.attractiveness = UnityEngine.Random.Range(0.01f,100f);
    
        traits.sightRange = UnityEngine.Random.Range(0.01f,5f);
    
        traits.dangerSense = UnityEngine.Random.Range(0.01f,10f);
    
        traits.strength = UnityEngine.Random.Range(0.01f,10f);
    
        traits.heatResistance = UnityEngine.Random.Range(0.01f,10f);
    
        traits.intellect = UnityEngine.Random.Range(0.01f,10f);
    
        traits.brute = UnityEngine.Random.Range(0.01f,10f);
        
        traits.HI = UnityEngine.Random.Range(0f,1f);
    
        traits.AI = UnityEngine.Random.Range(0f,1f);

        traits.FI = UnityEngine.Random.Range(0f,1f);
    
        traits.HUI = UnityEngine.Random.Range(0f,1f);

        traits.SI = UnityEngine.Random.Range(0f,1f);
    
        traits.RI = UnityEngine.Random.Range(0f,1f);
    }

    #endregion

}

[System.Serializable]
public class GeneticTraits {
    public float surroundingCheckCooldown;
    public float decisionCoolDown;
    public float speed;
    public float size;
    public float attractiveness;
    public float sightRange;
    public float dangerSense;
    public float strength;
    public float heatResistance;
    public float intellect;
    public float brute;

    public float HI;
    public float AI;
    public float FI;
    public float HUI;
    public float SI;
    public float RI;
}

[System.Serializable]
public struct CurrentState {
    public float energy;
    public float health;
    public float age;
    public float fear;
    public float hunger;
    public float sleepiness;
    public float reproductiveness;
}


[System.Serializable]
public class SenseNetwork {
    
    public List<string> possibleActions = new List<string>();
    public List<float> inputValues = new List<float>();
    public List<float> outputValues = new List<float>();
    public NNetwork network;

}

[System.Serializable]
public class Brain {
    
    [Header("Vision Sense")]
    public float fov;
    public float range;
    public List<string> tags = new List<string>();
    public List<SenseNetwork> correspondingNetwork = new List<SenseNetwork>();
    
}
