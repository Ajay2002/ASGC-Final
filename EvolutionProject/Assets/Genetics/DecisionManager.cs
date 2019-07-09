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

    ActionState currentActionState = ActionState.Nothing;

    private void StationaryChecking() {

        if ( entity.controller.avgVelocity.magnitude <= 0.05f) {
            stationaryT += Time.deltaTime;
        }
        else {
            stationaryT = 0f;
        }

        if (entity.controller.currentState == ActionState.Sleeping || entity.controller.currentState == ActionState.Breeding)
        if (stationaryT >= 8) {
            currentlyPerformingAction = false;
            entity.controller.ActionCompletion();
            stationaryT = 0f;
        }

        if (entity.controller.currentState == ActionState.Eating || entity.controller.currentState == ActionState.Running)
        if (stationaryT >= 2) {
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

            // if (state.age >= 10)
            // RunNonLinearTest();
            // else 
            if (entity.isNeuralNet)
            RunNonLinearTest();
            else
            RunLinearTest();

        }
    }

    private void RunNonLinearTest() {

        List<float> inputs = new List<float>();
        for (int i = 0; i < 8; i++) {
            if (i == 0) {
                inputs.Add((100-state.healthView)*traits.HI);
            }
            else if (i == 1) {
                inputs.Add(state.reproductivenessView*traits.RI);
            }
            else if (i == 2) {
                inputs.Add(state.hungerView*traits.HI);
            }
            else if (i == 3) {
                inputs.Add(state.sleepView*traits.SI);
            }
            else if (i == 4) {
                inputs.Add((100-state.energyView)*0.1f);
            }
            else if (i == 5) {
                inputs.Add((state.fearView)*traits.FI);
            }
            else if (i == 6) {
                inputs.Add(state.ageView*traits.AI);
            }

        }

        inputs.Add(entity.food.Count);
        inputs.Add(entity.enemies.Count);
        inputs.Add(entity.creatures.Count);


        List<float> outputs = new List<float>();

        outputs = entity.network.RunNetwork(inputs);
        float highestScore = 0f;
        int highest = 0;

        for (int i = 0; i < outputs.Count; i++) {
            if (outputs[i]>outputs[highest]) {
                highest = i;
            }
        }

        if (highest == 0) {
                LowHealth();
                currentlyWantingTo = "Increase Health";
                currentlyPerformingAction = true;
            }
            else if (highest == 1) {
                if (state.ageView >= 10) {
                HighReproductivity();
                currentlyWantingTo = "Reproduce";
                currentlyPerformingAction = true;
                }
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

    private void RunLinearTest() {
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

    private void LowHealth() {

        if (state.sleepView*traits.SI < state.hungerView*traits.HUI) {
            FoodRequired();
            return;
        }
        else {
            FoodRequired();
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
            FoodRequired();
            return;
        }

    }

    //Fighting not an option right now
    private void HighFear() {
        entity.controller.Flight(true);
    }
}
