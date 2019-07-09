using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityManager : MonoBehaviour
{
    [Header("State and Traits")]
    public bool initial;
    public GeneticTraits traits;
    public DecisionManager decision;

    public Vector3 position {
        get {
            return transform.position;
        }
    }

    //Action Management
    public void SuccessfulAction(string action) {
        decision.currentlyPerformingAction = false;
        decision.StateActionConversion();
    }

    public void FailedAction(string action) {
        decision.currentlyPerformingAction = false;
        decision.StateActionConversion();
    }
    
    //Sensory management & entity setup
    [Header("Sensory Elements")]
    public MeshRenderer renderer;
    public GTYPE type;
    public NNetwork network;
    public EntityManager parentA, parentB;
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

    public BiomeType creatureBiomeType;

    private void OnDrawGizmos() {
        Gizmos.DrawWireSphere(transform.position,traits.sightRange);
    }


    public bool isNeuralNet = false;

    private void Start() {
        
        manager = GameObject.FindObjectOfType<MapManager>();
        controller = GetComponent<ActionManager>();
        traits.manager = manager;

        MapManager.Instance.GetBiomeTypeFromPosition(transform.position, out creatureBiomeType);

        if (creatureBiomeType==BiomeType.Grass){
            renderer.sharedMaterials[1]=MapManager.Instance.biomeFurMaterials[0];
        }
        else if (creatureBiomeType==BiomeType.Snow) {
            renderer.sharedMaterials[1]=MapManager.Instance.biomeFurMaterials[1];
        }
        else if (creatureBiomeType==BiomeType.Desert) {
            renderer.sharedMaterials[1]=MapManager.Instance.biomeFurMaterials[2];
        }
        else if (creatureBiomeType==BiomeType.Forest) {
            renderer.sharedMaterials[1]=MapManager.Instance.biomeFurMaterials[3];
        }

        if (initial) {
            Randomise();
            if (manager.tryNetwork) {
                network = new NNetwork();
                network.Initialise(manager.hiddenLayer,manager.hiddenNeuron,10,5);
                int a = Random.Range(0,2);
                if (a == 0)
                    isNeuralNet = true;
            }
        }

        stateManagement.ResetState();


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
                decision.StateActionConversion();
            }
        }
    }

    public void Randomise() {
        network.Initialise(5,10,7,5);
        traits.surroundingCheckCooldown = UnityEngine.Random.Range(0.1f,2f);
        
        traits.decisionCoolDown = UnityEngine.Random.Range(0.1f,1f);
        
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