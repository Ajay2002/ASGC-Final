using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityManager : MonoBehaviour
{
    [Header("State and Traits")]
    public bool initial;
    public GeneticTraits traits;

    public Vector3 position {
        get {
            return transform.position;
        }
    }

    //Action Management
    public void SuccessfulAction(string action) {

    }

    public void FailedAction(string action) {

    }
    
    //Sensory management & entity setup
    [Header("Sensory Elements")]
    public GTYPE type;

    public List<EntityManager> enemies = new List<EntityManager>(); //Enemy Tag
    public List<EntityManager> creatures = new List<EntityManager>(); //Player Tag
    public List<Transform> food = new List<Transform>(); //Food Tag
    public List<EntityManager> bredWith = new List<EntityManager>();

    public void SensoryUpdate (List<EntityManager> enemies, List<Transform> food, List<EntityManager> creatures) {
        this.enemies = enemies;
        this.food = food;
        this.creatures = creatures;

        //State should not be modified anywhere except the real management
        stateManagement.SensoryUpdate();

    }

    [Header("External References & Components")]
    public MapManager manager;
    public ActionManager controller;
    public StateManager stateManagement;

    private void OnDrawGizmos() {
        Gizmos.DrawWireSphere(transform.position,traits.sightRange);
    }

    private void Start() {
        if (initial) 
            Randomise();

        stateManagement.ResetState();

        manager = GameObject.FindObjectOfType<MapManager>();
        controller = GetComponent<ActionManager>();
        traits.manager = manager;

        timerEnabled = true;
    }

    private bool timerEnabled;
    private float timerSense;
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
    }

    public void Randomise() {
        traits.surroundingCheckCooldown = UnityEngine.Random.Range(0.1f,2f);
        
        traits.decisionCoolDown = UnityEngine.Random.Range(0.1f,5f);
        
        traits.speed = UnityEngine.Random.Range(0.01f,1f)*5;
    
        traits.size = UnityEngine.Random.Range(0.01f,1f)*3;
        
        traits.attractiveness = UnityEngine.Random.Range(0.01f,1f);
    
        traits.sightRange = UnityEngine.Random.Range(0.5f,1f)*5;
    
        traits.dangerSense = UnityEngine.Random.Range(0.01f,1f);
    
        traits.strength = UnityEngine.Random.Range(0.01f,1f);
    
        traits.heatResistance = UnityEngine.Random.Range(0.01f,1f);
    
        traits.intellect = UnityEngine.Random.Range(0.01f,1f);
    
        traits.brute = UnityEngine.Random.Range(0.01f,1f);
        
        traits.HI = UnityEngine.Random.Range(0f,1f);
    
        traits.AI = UnityEngine.Random.Range(0f,1f);

        traits.FI = UnityEngine.Random.Range(0f,1f);
    
        traits.HUI = UnityEngine.Random.Range(0f,1f);

        traits.SI = UnityEngine.Random.Range(0f,1f);
    
        traits.RI = UnityEngine.Random.Range(0f,1f);
    }

}

public enum GTYPE {Creature, Predator};