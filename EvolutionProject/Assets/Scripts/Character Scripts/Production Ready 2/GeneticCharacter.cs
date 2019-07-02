using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GeneticCharacter : MonoBehaviour
{
    // #region Vars
    
    // public enum GeneticType {Predator, Creature};

    // [Header("Type")]
    // public GeneticType type;
    // public bool initial;
    // public GeneticCharacter parentA,parentB;

    // [Header("External References")]
    // public MapManager manager;
    // public GeneticController controller;

    // [Header("Traits")]
    // public GeneticTraits traits;
    // [Header("State")]
    // public CurrentState state;

    // //Sensory Variables
    // [Header("Current Senses")]
    // public List<GeneticCharacter> enemies = new List<GeneticCharacter>(); //Enemy Tag
    // public List<GeneticCharacter> creatures = new List<GeneticCharacter>(); //Player Tag
    // public List<Transform> food = new List<Transform>(); //Food Tag
    // public List<GeneticCharacter> bredWith = new List<GeneticCharacter>();

    // #endregion
    
    // #region  State Management

    // public virtual void StateUpdate() {
    //     state.energy = Mathf.Clamp(state.energy,0f,100f);
        
    //     state.age += age * Time.deltaTime;

    //     if (state.age >= 100) {
    //         Death();
    //     }
        
    //     if (enemies != null)
    //     state.fear = Mathf.Clamp(enemies.Count*10 - (traits.strength*10)/2 + traits.dangerSense*10,0f,100f);
        
    //     if (state.energy <= 50) {
    //         state.sleepiness = Mathf.Clamp(state.sleepiness+0.3f*Time.deltaTime,0f,100f);
    //     }

    //     if (state.age <= 50 && state.age >= 20) {
    //         state.reproductiveness += 0.9f * Time.deltaTime;
    //     }
    //     else if (state.age >= 50 && state.age <= 100) {
    //         state.reproductiveness -= 0.9f * Time.deltaTime;
    //     }
        

    //     state.hunger = Mathf.Clamp(state.hunger + (Time.deltaTime*0.2f*traits.size),0f,100f);


    //     //Consequences
    //     if (state.health <= 0) {
    //         Death();
    //     }

    //     if (state.age >= 100) {
    //         Death();
    //     }

    //     if (state.hunger >= 100) {
    //         state.energy -= 2*Time.deltaTime;
    //         state.health -= 2*Time.deltaTime;
    //     }

    //     if (state.sleepiness >= 100) {
    //         state.energy -= 1.5f*Time.deltaTime;
    //         state.health -= 2f*Time.deltaTime;
    //     }

    //     if (!currentlyPausedState && !pausedState)
    //     StateActionConversion();
    // }

    // public virtual void StateActionConversion() {
    //     public virtual void StateActionConversion() {

    //     float highestScore = 0f;
    //     int highest = 0;
    //     for (int i = 0; i < 8; i++) {

    //         if (i == 0) {
    //             if ((100-state.healthView)*traits.HI > highestScore) {
    //                 highestScore = (100-state.health);
    //                 highest = i;
    //             }
    //         }
    //         else if (i == 1) {
    //             if (state.reproductivenessView*traits.RI > highestScore) {
    //                 highestScore = state.reproductiveness;
    //                 highest = i;
    //             }
    //         }
    //         else if (i == 2) {
    //             if (state.hungerView*traits.HUI > highestScore) {
    //                 highestScore = state.hunger;
    //                 highest = i;
    //             }
    //         }
    //         else if (i == 3) {
    //             if (state.sleepView*traits.SI > highestScore) {
    //                 highestScore = state.sleepiness;
    //                 highest = i;
    //             }
    //         }
    //         else if (i == 4) {
    //             if ((100-state.energyView) > highestScore) {
    //                 highestScore = state.energy;
    //                 highest = i;
    //             }
    //         }
    //         else if (i == 5) {
    //             if ((state.fearView)*traits.FI > highestScore) {
    //                 highestScore = state.energy;
    //                 highest = i;
    //             }
    //         }
    //     }

    //     if (highest == 0) {
    //         LowHealth();
    //         currentlyWantingTo = "Increase Health";
    //     }
    //     else if (highest == 1) {
    //         HighReproductivity();
    //         currentlyWantingTo = "Reproduce";
    //     }
    //     else if (highest == 2) {
    //         FoodRequired();
    //         currentlyWantingTo = "Eat";
    //     }
    //     else if (highest == 3) {
    //         LowSleep();
    //         currentlyWantingTo = "Sleep";
    //     }
    //     else if (highest == 4) {
    //         LowEnergy();
    //         currentlyWantingTo = "Increase Energy";
    //     }
    //     else if (highest == 5) {
    //         HighFear();
    //         currentlyWantingTo = "Fight or Flight";
    //     }
    // }
    // }
    

    // public abstract void LowSleep();
    // public abstract void LowHealth();
    // public abstract void FoodRequired();
    // public abstract void HighFear();
    // public abstract void LowEnergy();
    // public abstract void HighReproductivity();

    // #endregion

    // #region  Action Management

    // public string currentAction = "";

    // public virtual void CompletedAction (string invocationStatement) {
        
    // }
    // public virtual void Eat(){}
    // public virtual void Fight(GeneticEntity_T fighter){}
    // public virtual void Flight(){}
    // public virtual void Reproduce() {
    //     /*
    //     e.state.energy -= 70;
    //     e.state.hunger += 30;
    //     e.state.sleepiness += 20;
    //     e.state.reproductiveness -= 100;

    //     state.energy -= 70;
    //     state.hunger += 30;
    //     state.sleepiness += 20;
    //     state.reproductiveness -= 100;
    //     */

    //     //This is where the request is sent, the mating is done etc.
    // }
    // public virtual void Breed(GeneticCharacter e) {
        
    //     Debug.Log("Change The Spawner's Type");
    //     GeneticCharacter newEntity = manager.SpawnEntity(manager.GetRandomPointAwayFrom(transform.position,traits.sightRange)).GetComponent<GeneticCharacter>();
       

    //     #region  Trait Modification
    //     newEntity.traits.surroundingCheckCooldown = UnityEngine.Random.Range(0.01f,1.0f)<manager.mutationChance ? UnityEngine.Random.Range(0.1f,2f) : ((traits.surroundingCheckCooldown+e.traits.surroundingCheckCooldown)/2);
    
    //     newEntity.traits.decisionCoolDown = UnityEngine.Random.Range(0.01f,1.0f)<manager.mutationChance ? UnityEngine.Random.Range(0.1f,5f) : ((traits.decisionCoolDown+e.traits.decisionCoolDown)/2);
        
    //     newEntity.traits.speed = UnityEngine.Random.Range(0.01f,1.0f)<manager.mutationChance ? (UnityEngine.Random.Range(0.01f,1f)*5) : ((traits.speed+e.traits.speed)/2);
    
    //     newEntity.traits.size = UnityEngine.Random.Range(0.01f,1.0f)<manager.mutationChance ? UnityEngine.Random.Range(0.01f,1f)*3 : ((traits.size+e.traits.size)/2);
        
    //     newEntity.traits.attractiveness = UnityEngine.Random.Range(0.01f,1.0f)<manager.mutationChance ? UnityEngine.Random.Range(0.01f,1f) : ((traits.attractiveness+e.traits.attractiveness)/2);
    
    //     newEntity.traits.sightRange = UnityEngine.Random.Range(0.01f,1.0f)<manager.mutationChance ? UnityEngine.Random.Range(0.01f,1f)*5 : ((traits.sightRange+e.traits.sightRange)/2);
    
    //     newEntity.traits.dangerSense = UnityEngine.Random.Range(0.01f,1.0f)<manager.mutationChance ? UnityEngine.Random.Range(0.01f,1f) : ((traits.dangerSense+e.traits.dangerSense)/2);
    
    //     newEntity.traits.strength = UnityEngine.Random.Range(0.01f,1.0f)<manager.mutationChance ? UnityEngine.Random.Range(0.01f,1f) : ((traits.strength+e.traits.strength)/2);
    
    //     newEntity.traits.heatResistance = UnityEngine.Random.Range(0.01f,1.0f)<manager.mutationChance ? UnityEngine.Random.Range(0.01f,1f) : ((traits.heatResistance+e.traits.heatResistance)/2);
    
    //     newEntity.traits.intellect  =UnityEngine.Random.Range(0.01f,1.0f)<manager.mutationChance ? UnityEngine.Random.Range(0.01f,1f) : ((traits.intellect+e.traits.intellect)/2);
    
    //     newEntity.traits.brute = UnityEngine.Random.Range(0.01f,1.0f)<manager.mutationChance ? UnityEngine.Random.Range(0.01f,1f) : ((traits.brute+e.traits.brute)/2);
        
    //     newEntity.traits.HI = UnityEngine.Random.Range(0.01f,1.0f)<manager.mutationChance ? UnityEngine.Random.Range(0f,1f) : ((traits.HI+e.traits.HI)/2);
    
    //     newEntity.traits.AI = UnityEngine.Random.Range(0.01f,1.0f)<manager.mutationChance ? UnityEngine.Random.Range(0f,1f) : ((traits.AI+e.traits.AI)/2);

    //     newEntity.traits.FI = UnityEngine.Random.Range(0.01f,1.0f)<manager.mutationChance ? UnityEngine.Random.Range(0f,1f) : ((traits.FI+e.traits.FI)/2);
    
    //     newEntity.traits.HUI = UnityEngine.Random.Range(0.01f,1.0f)<manager.mutationChance ? UnityEngine.Random.Range(0f,1f) : ((traits.HUI+e.traits.HUI)/2);

    //     newEntity.traits.SI = UnityEngine.Random.Range(0.01f,1.0f)<manager.mutationChance ? UnityEngine.Random.Range(0f,1f) : ((traits.SI+e.traits.SI)/2);
    
    //     newEntity.traits.RI = UnityEngine.Random.Range(0.01f,1.0f)<manager.mutationChance ? UnityEngine.Random.Range(0f,1f) : ((traits.RI+e.traits.RI)/2);
    //     #endregion

    //     //State Resets
    //     newEntity.state.age = 0;
    //     newEntity.state.energy = 100;
    //     newEntity.state.fear = 0;
    //     newEntity.state.sleepiness = 0;
    //     newEntity.state.hunger = 0;
    //     newEntity.state.health = 100;
    //     newEntity.state.reproductiveness = 0f;

    //     newEntity.parentA = this;
    //     newEntity.parentB = e;
    // }

    // public virtual void Death(){}
    // public virtual void Sleep(){}
    // public virtual void Roam(){}

    // #endregion

    // #region  Request Management

    // public virtual void BreedRequest() {

    // }

    // public virtual void FightRequest() {

    // }

    // #endregion

    // #region  Sensory Management

    // //Receiving of sensory data & energy consumption thruogh sensory capture
    // public virtual void SensoryUpdate(List<GeneticCharacter> enemies, List<Transform> food, List<GeneticCharacter> friends) {
        
    //     this.enemies = enemies;
    //     this.creatures = friends;
    //     this.food = food;

    //     //Energy Expenditure on Sense
    //     state.energy -= 2.5f * (traits.sightRange/10)*traits.dangerSense;

    // }

    // #endregion

    // #region  Unity Methods
    
    // //Other Variables
    // [HideInInspector]
    // public float ageRate;

    // private void Start() {

    //     ageRate = Random.Range(0.2f,1.5f);
        
    //     if (initial) 
    //         Randomise();

    //     state.age = 0;
    //     state.energy = 100;
    //     state.health = 100;

    //     manager = GameObject.FindObjectOfType<MapManager>();
    //     controller = GetComponent<GeneticController>();
    //     traits.manager = manager;

    //     timerEnabled = true;
    // }

    // private bool timerEnabled;
    // private float timerSense;
    // private void Update() {
    //     if (timerEnabled == true) {
    //         if (timerSense > 0) {
    //             timerSense -= Time.deltaTime;
    //         }
    //         else if (timerSense <= 0) {
    //             controller.FOVChecker(traits.surroundingCheckCooldown,traits.sightRange);
    //             timerSense = traits.surroundingCheckCooldown;
    //         }
    //     }

    //     if (state.health > -10) {
    //         StateUpdate();
    //     }

    //     if (currentAction != "") {

    //     }
    // }

    // private void OnCollisionEnter (Collider col) {
    //     if (col.transform.tag == "Food") {
    //         if (type == GeneticType.Creature) {
    //             state.hunger -= 40*((traits.size/3));
    //             state.energy += 20*((traits.size/3));
    //             if (food.Contains(col.transform))
    //                 food.Remove(col.transform);
    //             GameObject.Destroy(col.transform.gameObject);
    //         }
    //     }
    //     else if (col.transform.tag == "Player") {

    //     }
    //     else if (col.transform.tag == "Enemy") {

    //     }
    // }

    // #endregion
    
    // #region  Energy Management Methods

    // public virtual float EnergyMovementCalculation (float movementSpeed) {
    //     return movementSpeed*traits.size+state.age*0.05f;
    // }


    // #endregion

    // #region  Helper Methods

    // public void Randomise() {

    //     traits.surroundingCheckCooldown = UnityEngine.Random.Range(0.1f,2f);
        
    //     traits.decisionCoolDown = UnityEngine.Random.Range(0.1f,5f);
        
    //     traits.speed = UnityEngine.Random.Range(0.01f,1f)*5;
    
    //     traits.size = UnityEngine.Random.Range(0.01f,1f)*3;
        
    //     traits.attractiveness = UnityEngine.Random.Range(0.01f,1f);
    
    //     traits.sightRange = UnityEngine.Random.Range(0.01f,1f)*10;
    
    //     traits.dangerSense = UnityEngine.Random.Range(0.01f,1f);
    
    //     traits.strength = UnityEngine.Random.Range(0.01f,1f);
    
    //     traits.heatResistance = UnityEngine.Random.Range(0.01f,1f);
    
    //     traits.intellect = UnityEngine.Random.Range(0.01f,1f);
    
    //     traits.brute = UnityEngine.Random.Range(0.01f,1f);
        
    //     traits.HI = UnityEngine.Random.Range(0f,1f);
    
    //     traits.AI = UnityEngine.Random.Range(0f,1f);

    //     traits.FI = UnityEngine.Random.Range(0f,1f);
    
    //     traits.HUI = UnityEngine.Random.Range(0f,1f);

    //     traits.SI = UnityEngine.Random.Range(0f,1f);
    
    //     traits.RI = UnityEngine.Random.Range(0f,1f);
    // }

    // #endregion
}
[System.Serializable]
public class GeneticTraits
{
    public MapManager manager;

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
public struct CurrentState
{   
    public float energyView;
    public float healthView;
    public float ageView;
    public float fearView;
    public float hungerView;
    public float sleepView;
    public float reproductivenessView;
    
	public float energy {
        set {
            energyView = Mathf.Clamp(value,0,100);
        }
        get {return energyView;}
    }
	public float health { 
        set {

            healthView = Mathf.Clamp(value,0,100);
        }
        get {return healthView;}
    }
	public float age { 
        set {

            ageView = Mathf.Clamp(value,0,100);
        }
        get {return ageView;}
    }
	public float fear  {
        set {

            fearView = Mathf.Clamp(value,0,100);
        }
        get {return fearView;}
    }
	public float hunger { 
        set {

            hungerView = Mathf.Clamp(value,0,100);
        }
        get {return hungerView;}
    }
	public float sleepiness { 
        set {

            sleepView = Mathf.Clamp(value,0,100);
        }
        get {return sleepView;}
    }
	public float reproductiveness { 
        set {

            reproductivenessView = Mathf.Clamp(value,0,100);
        }
        get {return reproductivenessView;}
    }
}
