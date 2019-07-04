﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateManager : MonoBehaviour
{
    public float fitness;
    public EntityManager entity;
    public CurrentState state;

    //public CurrentState tempState;

    private void Start() {
        entity = GetComponent<EntityManager>();
    }

    private void Update() {
        StateUpdate();
    }

    private void StateUpdate() {
        state.energy = Mathf.Clamp(state.energy,0f,100f);
        
        state.age += 0.6f * Time.deltaTime;

        if (state.age >= 100) {
            GameObject.Destroy(this.gameObject);
        }
        
        if (entity.enemies != null)
        state.fear = Mathf.Clamp(entity.enemies.Count*10 - entity.traits.strength/2 + entity.traits.dangerSense/3,0f,100f);
        
        if (state.energy <= 50) {
            state.sleepiness = Mathf.Clamp(state.sleepiness+2*Time.deltaTime,0f,100f);
        }

        if (state.age <= 50 && state.age >= 17) {
            state.reproductiveness += 1 * Time.deltaTime * Random.Range(1f,1.5f);
        }
        else if (state.age >= 50 && state.age <= 75) {
            state.reproductiveness -= 1 * Time.deltaTime * Random.Range(1,1.5f);
        }
        

        state.hunger = Mathf.Clamp(state.hunger + (Time.deltaTime*1.5f*entity.traits.size),0f,100f);

        if (state.hunger >= 95) {
            state.health -= Time.deltaTime*20;
            state.energy -= Time.deltaTime;
        }

        if (state.sleepiness >= 95) {
            state.health -= Time.deltaTime*3;
            state.energy -= Time.deltaTime;
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
        state.energy += 20;
        state.health += 20;
        state.sleepiness -= 40;
    }

    public virtual float EnergyMovementCalculation (float movementSpeed) {
        return movementSpeed*entity.traits.size+state.age*0.05f;
    }

    public void Pursuit (EntityManager e) {
        if (e != null && entity != null)
        state.fear += entity.traits.strength*10+e.traits.size*2+e.traits.speed*10;
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
