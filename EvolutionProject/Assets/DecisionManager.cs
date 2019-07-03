using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DecisionManager : MonoBehaviour
{
    public EntityManager entity;
    public CurrentState state {
        get {
            return entity.stateManagement.state;
        }
    }

    public GeneticTraits traits {
        get {
            return entity.traits;
        }
    }

    float stationaryT = 0f;
    private void Update() {
        StationaryChecking();
    }

    ActionManager.ActionState currentActionState = ActionManager.ActionState.Nothing;

    private void StationaryChecking() {

        if (currentActionState == entity.controller.currentState) {
            stationaryT += Time.deltaTime;
        }
        else {
            stationaryT = 0f;
        }

        if (stationaryT >= 10) {
            currentlyPerformingAction = false;
            entity.controller.ActionCompletion();
            stationaryT = 0f;
        }

        currentActionState = entity.controller.currentState;

    }
    
    public string currentlyWantingTo;
    public bool currentlyPerformingAction = false;

    public void StateActionConversion() {
        if (!currentlyPerformingAction) {
            stationaryT = 0f;
            float highestScore = 0f;
            int highest = 0;
            for (int i = 0; i < 8; i++) {

                if (i == 0) {
                    if ((100-state.healthView)*traits.HI > highestScore) {
                        highestScore = (100-state.healthView)*traits.HI;
                        highest = i;
                    }
                }
                else if (i == 1) {
                    if (state.reproductivenessView*traits.RI > highestScore) {
                        highestScore = state.reproductivenessView*traits.RI;
                        highest = i;
                    }
                }
                else if (i == 2) {
                    if (state.hungerView*traits.HUI > highestScore) {
                        highestScore = state.hungerView*traits.HUI;
                        highest = i;
                    }
                }
                else if (i == 3) {
                    if (state.sleepView*traits.SI > highestScore) {
                        highestScore = state.sleepView*traits.SI;
                        highest = i;
                    }
                }
                else if (i == 4) {
                    if ((100-state.energyView)*0.1f > highestScore) {
                        highestScore = (100-state.energyView)*0.1f;
                        highest = i;
                    }
                }
                else if (i == 5) {
                    if ((state.fearView)*traits.FI > highestScore) {
                        highestScore = (state.fearView)*traits.FI;
                        highest = i;
                    }
                }
            }

            if (highest == 0) {
                LowHealth();
                currentlyWantingTo = "Increase Health";
                currentlyPerformingAction = true;
            }
            else if (highest == 1) {
                HighReproductivity();
                currentlyWantingTo = "Reproduce";
                currentlyPerformingAction = true;
            }
            else if (highest == 2) {
                FoodRequired();
                currentlyWantingTo = "Eat";
                currentlyPerformingAction = true;
            }
            else if (highest == 3) {
                LowSleep();
                currentlyWantingTo = "Sleep";
                currentlyPerformingAction = true;
            }
            else if (highest == 4) {
                LowEnergy();
                currentlyWantingTo = "Increase Energy";
                currentlyPerformingAction = true;
            }
            else if (highest == 5) {
                HighFear();
                currentlyWantingTo = "Fight or Flight";
                currentlyPerformingAction = true;
            }
        }
    }

    private void LowHealth() {

        if (state.sleepView*traits.SI < state.hungerView*traits.HUI) {
            FoodRequired();
            return;
        }
        else {
            LowSleep();
            return;
        }
        
    }

    private void HighReproductivity() {

        entity.controller.Breed(true);

    }

    private void FoodRequired() {
        entity.controller.Eat(true);
    }

    private void LowSleep() {
        entity.controller.Sleep(true);
    }

    private void LowEnergy() {

        if (state.sleepView*traits.SI < state.hungerView*traits.HUI) {
            FoodRequired();
            return;
        }
        else {
            LowSleep();
            return;
        }

    }

    //Fighting not an option right now
    private void HighFear() {
        entity.controller.Flight(true);
    }
}
