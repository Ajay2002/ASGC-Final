using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateManager : MonoBehaviour
{
    public float fitness;
    public EntityManager entity;
    public CurrentState state;

    [Header("Wave Based Properties")]
    public int maxAge;
    public float eatPower=1f;
    public float hungerIncrease=1.2f;
    public float sleepPower=1f;
    public float reproductiveIncrease=2;
    public float reproductiveCost=1f;


    //public CurrentState tempState;

    private void Start() {
        entity = GetComponent<EntityManager>();
    }

    private void Update() {
        if (entity.type == GTYPE.Creature)
        StateUpdate();
        else
        FoxStateUpdate();
    }

    private void FoxStateUpdate() {
        state.energy = Mathf.Clamp(state.energy,0f,100f);
        
        state.age += 0.3f * Time.deltaTime;

        state.thirst += 0.7f*Time.deltaTime;

        if (state.age >= maxAge) {
            GameObject.Destroy(this.gameObject);
        }
        
        if (entity.enemies != null)
        state.fear = Mathf.Clamp(entity.enemies.Count*10 - entity.traits.strength/2 + entity.traits.dangerSense/3,0f,100f);
        
        if (state.energy <= 50) {
            state.sleepiness = Mathf.Clamp(state.sleepiness+2*Time.deltaTime,0f,100f);
        }

        if (state.age <= 50 && state.age >= 20) {
            state.reproductiveness += reproductiveIncrease * Time.deltaTime * Random.Range(1f,1.5f);
        }
        else if (state.age >= 50 && state.age <= 75) {
            state.reproductiveness -= 0.2f * Time.deltaTime;
        }
        

        state.hunger = Mathf.Clamp(state.hunger + (Time.deltaTime*hungerIncrease*entity.traits.size),0f,100f);

        if (state.hunger >= 95) {
            state.health -= Time.deltaTime*20;
            state.energy -= Time.deltaTime*3;
        }

        if (state.sleepiness >= 95) {
            state.health -= Time.deltaTime*20;
            state.energy -= Time.deltaTime*3;
        }

        if (state.energy <= 5) {
            state.hunger += Time.deltaTime*1.5f;
            state.sleepiness += Time.deltaTime;
        }

        if (state.health <= 0) {
            GameObject.Destroy(this.gameObject);
        }

        if (state.age >= maxAge) {
            GameObject.Destroy(this.gameObject);
        }
    }

    float temperatureCalculation = 0f;
    private void StateUpdate() {
        if (temperatureCalculation <= 0f) {
            //Temp : Grass (0.5), Desert (1), Snow (0.01), Forest (0.6)

            float f = 0.5f;

            BiomeType t;
            if (MapManager.Instance.GetBiomeTypeFromPosition(transform.position, out t)) {

                if (t == BiomeType.Grass)
                    f = 0.5f;
                else if (t == BiomeType.Desert)
                    f = 1f;
                else if (t == BiomeType.Snow)
                    f = 0.01f;
                else if (t == BiomeType.Forest) 
                    f = 0.75f;

            }

            float distanceFrom = Mathf.Abs(entity.traits.heatResistance - f);

            state.energy -= distanceFrom*Time.deltaTime*2;

            temperatureCalculation = Random.Range(1f, 3f);
        }
        else temperatureCalculation-=Time.deltaTime;
        state.energy = Mathf.Clamp(state.energy,0f,100f);
        
        state.age += 0.3f * Time.deltaTime;

        state.thirst += 0.7f*Time.deltaTime;

        if (state.age >= 100) {
            GameObject.Destroy(this.gameObject);
        }
        
        if (entity.enemies != null)
        state.fear = Mathf.Clamp(entity.enemies.Count*10 - entity.traits.strength/2 + entity.traits.dangerSense/3,0f,100f);
        
        if (state.energy <= 50) {
            state.sleepiness = Mathf.Clamp(state.sleepiness+2*Time.deltaTime,0f,100f);
        }

        if (state.age <= 50 && state.age >= 20) {
            state.reproductiveness += 2 * Time.deltaTime * Random.Range(1f,1.5f);
        }
        else if (state.age >= 50 && state.age <= 75) {
            state.reproductiveness -= 0.2f * Time.deltaTime;
        }
        

        state.hunger = Mathf.Clamp(state.hunger + (Time.deltaTime*2f*entity.traits.size),0f,100f);

        if (state.hunger >= 95) {
            state.health -= Time.deltaTime*20;
            state.energy -= Time.deltaTime*3;
        }

        if (state.sleepiness >= 95) {
            state.health -= Time.deltaTime*20;
            state.energy -= Time.deltaTime*3;
        }

        if (state.energy <= 5) {
            state.hunger += Time.deltaTime;
            state.sleepiness += Time.deltaTime;
        }

        if (state.health <= 0) {
            GameObject.Destroy(this.gameObject);
        }

        if (state.age >= 100) {
            GameObject.Destroy(this.gameObject);
        }

    }

    public void SensoryUpdate() {
        state.energy -= 0.2f * (entity.traits.sightRange/10)*entity.traits.dangerSense;
    }

    public void EatState() {
        state.hunger -= 40*((entity.traits.size/3));
        state.energy += 5;
    }

    public void EatState (Food food) {
        //No value attached to food
        state.hunger -= 40*((entity.traits.size/3))*food.value;
        state.energy += 5;
    }

    public void ResetState () {
        state.age = 0;
        state.energy = 100;
        state.health = 100;
        state.fear = 0;
        state.hunger = 0;
        state.sleepiness = 0;
    }


    public void AquiringSleep() {
//        Debug.LogError("Nothing here");

        state.energy += 20*sleepPower;
        state.health += 20*sleepPower;
        state.sleepiness -= 40*sleepPower;
    }

    public virtual float EnergyMovementCalculation (float movementSpeed) {
        Biome b = MapManager.Instance.GetBiomeFromPosition(transform.position);
        if (b!= null && b.type == entity.creatureBiomeType)
            return movementSpeed*entity.traits.size+state.age*0.05f;
        else
            return movementSpeed*entity.traits.size+state.age*0.1f;
    }

    public void Pursuit (EntityManager e) {
        if (e != null && entity != null) {
            state.fear += entity.traits.strength*15+e.traits.size*2+e.traits.speed*10;
            fitnessAddition+=2f;
        }
    }

    public void ReproductionState() {
        state.energy -= 30*reproductiveCost;
        state.hunger += 20*reproductiveCost;
        state.sleepiness += 20*reproductiveCost;
        state.reproductiveness -= 60*reproductiveCost;
        fitnessAddition+=30*reproductiveCost;
    }

    private float fitnessAddition = 0f;
    
    public float EvaluateFitness() {
        return (((100-state.ageView)+state.energyView+(100-state.hungerView)+(100-state.sleepView)+(100-state.fearView)+state.health*2)/10)+fitnessAddition;
    }

    public void DrankWaterState() {
        state.thirst -= 40;
        state.reproductiveness+=10;
        state.energy += 20;
        state.health+=10;
    }
}
