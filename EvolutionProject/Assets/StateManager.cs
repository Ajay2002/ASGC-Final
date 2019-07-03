using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateManager : MonoBehaviour
{
    public float fitness;
    public EntityManager entity;
    public CurrentState state;

    private void Start() {
        entity = GetComponent<EntityManager>();
    }

    private void Update() {
        fitness = EvaluateFitness();
    }

    public void SensoryUpdate() {
        state.energy -= 0.2f * (entity.traits.sightRange/10)*entity.traits.dangerSense;
    }

    public void EatState() {
        state.hunger -= 40*((entity.traits.size/3));
        state.energy += 20*((entity.traits.size/3));
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
        Debug.LogError("Nothing here");
    }

    public virtual float EnergyMovementCalculation (float movementSpeed) {
        return movementSpeed*entity.traits.size+state.age*0.05f;
    }

    public void Pursuit (EntityManager e) {
        state.fear = 100-entity.traits.strength/2-e.traits.size/2;
    }

    public void ReproductionState() {
        state.energy -= 70;
        state.hunger += 30;
        state.sleepiness += 20;
        state.reproductiveness -= 100;
    }
    
    public float EvaluateFitness() {
        return ((100-state.ageView)+state.energyView+(100-state.hungerView)+(100-state.sleepView)+(100-state.fearView)+state.health*2)/10;
    }

}
