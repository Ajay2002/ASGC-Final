using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GE : MonoBehaviour
{

//     #region  Variables

//     //References
//     [Header("External References")]
//     public MapManager manager;
//     public GeneticController controller;

//     [Header("Internal References")]
//     public GeneticTraits traits;
//     public CurrentState state;
//     public Brain brain;

//     //Private Bools
//     [Header("Program State")]
//     public float fitness;
//     public bool currentlyPerformingAction;
//     public float timeSinceAction;

//     #endregion

//     #region  Default Methods

//     private void Start() {
//         //Just start to raom
//         Roam();
//     }

//     private void Update() {

//     }

//     #endregion

//     #region  Actions
    
//     public void Fight(GE e) {

//     }

//     private void OverrideAction() {
//         currentlyPerformingAction = true;
//     }
    
//     private void Death() {
//         GameObject.Destroy(this.gameObject);
//     }

//     //Energy Boost, Hunger Reduction + Health
//     public void Eat(Transform food) {

//     }

//     public void Sleep() {

//     }

//     public void Nothing() {

//     }

//     public void Roam() {
//         Vector3 p = manager.GetRandomPointAwayFrom(transform.position, traits.sightRange);
//         controller.MoveTo(p,traits.speed/2,"roamCompleted",0);

//         currentlyPerformingAction = true;
//         timeSinceAction = 0;
//     }

//     public void Flight(Vector3 position) {
//         Vector3 p = manager.GetRandomPointAwayFrom(position, traits.sightRange);
//         controller.MoveTo(p,traits.speed,"flightCompleted",0);

//         currentlyPerformingAction = true;
//         timeSinceAction = 0;
//     }

//     public void CompletedAction (string actionType) {
        
//         if (actionType == "flightCompleted") {
//             currentlyPerformingAction = false;
//         }
//         else if (actionType == "roamCompleted") {

//             //TODO: If nothing is in sight range (this is important)
//             Roam();
//             currentlyPerformingAction = false;
//         }
//     }

//     #endregion

//     #region  Energy Calculations
//     public float EnergyMovementCalculation (float movementSpeed) {
//         //NOTE: This is a temporary function        
//         return movementSpeed*traits.size+state.age;
//     }

//     public float EnergyMovementCalculation (float movementSpeed, float d) {
//         //NOTE: This is a temporary function        
//         return d*(movementSpeed*traits.size-Mathf.Pow(state.age,2));
//     }
//     #endregion
}

// [System.Serializable]
// public class GeneticTraits {
//     public float surroundingCheckCooldown;
//     public float decisionCoolDown;
//     public float speed;
//     public float size;
//     public float attractiveness;
//     public float sightRange;
//     public float dangerSense;
//     public float strength;
//     public float heatResistance;
//     public float intellect;
//     public float brute;

//     public float HI;
//     public float AI;
//     public float FI;
//     public float HUI;
//     public float SI;
//     public float RI;
// }

// [System.Serializable]
// public struct CurrentState {
//     public float energy;
//     public float health;
//     public float age;
//     public float fear;
//     public float hunger;
//     public float sleepiness;
//     public float reproductiveness;
// }

