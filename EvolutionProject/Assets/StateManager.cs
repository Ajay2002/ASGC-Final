using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateManager : MonoBehaviour
{
    public EntityManager entity;
    public CurrentState state;

    private void Start() {
        entity = GetComponent<EntityManager>();
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

}
