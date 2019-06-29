using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Reflection;

public class Trash : MonoBehaviour
{
//     [Header("Testing")]
//     public bool isRunningNetwork = false;
//     public bool showGizmos = true;
   

//     public GeneticTraits traits;
    
//     public CurrentState state;
//     public Brain brain;

//     [Header("Other References")]
//     public MapManager manager;
//     public GeneticController controller;
//     public GraphHelp help;
//     //Duration of survival?
//     //TODO: Don't forget to take into consideration the fitness & genetic mutations. That's what kills or grows at the end of the day!! 
    
//     [Header("Program State")]
//     public float fitness;
//     public bool currentlyPerformingAction;
//     public float timeSinceAction;

//     //TODO: When taking in consideration to fight something the TRAITS of the other entity have to be taken into consideration as well.
//     //FIXME: If nothing is actually being detected just roam around the place until it sees something + this costs energy or it can sleep (based on nothing network)
//     private void Start() {

       
//         // //Adding graphs
//         // help.AddGraph("Age",Color.red);
//         // help.AddGraph("Energy",Color.blue);
//         // help.AddGraph("Fear",Color.green);
//         // help.AddGraph("Sleepiness",Color.yellow);
//         // help.AddGraph("Hunger",Color.cyan);
//         // help.AddGraph("Reproductive Urge",Color.black);

//         if (isRunningNetwork) {
//             for (int i = 0; i < brain.tags.Count; i++) {
//                 GameObject[] gameObjects = GameObject.FindGameObjectsWithTag(brain.tags[i]);
//                 SenseNetwork net = brain.correspondingNetwork[i];
//                 net.network.Initialise(2,10,6,net.possibleActions.Count);

//                 float maxDist = 0;

//                 for (int t = 0; t < gameObjects.Length; t++) {
//                     if (Vector3.Distance(gameObjects[t].transform.position,transform.position) > maxDist) {
//                         maxDist = Vector3.Distance(gameObjects[t].transform.position,transform.position);
//                     }

//                 }
                

//                 for (int t = 0; t < gameObjects.Length; t++) {         
//                     List<float> inputs = new List<float>();
//                     float dist = Vector3.Distance(transform.position,gameObjects[t].transform.position)/maxDist;
//                     inputs.Add(state.age);
//                     inputs.Add(state.fear);
//                     inputs.Add(state.health);
//                     inputs.Add(state.hunger);
//                     inputs.Add(dist);
//                     inputs.Add(state.sleepiness);
                    
//                     net.inputValues = inputs;
                    
//                     List<float> outputs = net.network.RunNetwork(inputs);
//                     net.outputValues.Clear();
//                     for (int r = 0; r < outputs.Count; r++) {
//                         net.outputValues.Add(outputs[r]);
//                     }
//                     int best = 0;
//                     for (int d = 0; d < outputs.Count; d++) {
//                         //  print (net.possibleActions[d] + " : " + outputs[d]);
//                         if (outputs[best] < outputs[d]) {
//                             best = d;
//                         }
//                     }

//                 }   

//             }   
//         }
//         else {
//             Roam();
            
//             //FIXME: This is so bad, please fix this to be a non-enumerator
//             //controller.FOVChecker(traits.surroundingCheckCooldown,traits.sightRange);
//         }

//     }

//     //TODO: Fix this, this is the worst possible way to do this.. :(
        
//     public void Randomise() {
//         for (int i = 0; i < brain.tags.Count; i++) {
//             SenseNetwork net = brain.correspondingNetwork[i];
//             net.network.Initialise(2,10,6,net.possibleActions.Count);
//         }

//         traits.surroundingCheckCooldown = UnityEngine.Random.Range(0.1f,2f);
        
//        traits.decisionCoolDown = UnityEngine.Random.Range(0.5f,5f);
        
//         traits.speed = UnityEngine.Random.Range(0.01f,100f);
      
//         traits.size = UnityEngine.Random.Range(0.01f,100f);
        
//         traits.attractiveness = UnityEngine.Random.Range(0.01f,100f);
      
//         traits.sightRange = UnityEngine.Random.Range(0.01f,100f);
    
//         traits.hearingRange = UnityEngine.Random.Range(0.01f,100f);
    
//         traits.dangerSense = UnityEngine.Random.Range(0.01f,100f);
    
//         traits.strength = UnityEngine.Random.Range(0.01f,100f);
      
//         traits.heatResistance = UnityEngine.Random.Range(0.01f,100f);
      
//         traits.intellect = UnityEngine.Random.Range(0.01f,100f);
     
//         traits.brute = UnityEngine.Random.Range(0.01f,100f);
        
//         traits.lifespan = UnityEngine.Random.Range(0.01f,100f);

//         traits.healthImportance = UnityEngine.Random.Range(0f,1f);
      
//         traits.ageImportance = UnityEngine.Random.Range(0f,1f);

//         traits.fearImportance = UnityEngine.Random.Range(0f,1f);
       
//         traits.hungerImportance = UnityEngine.Random.Range(0f,1f);

//         traits.sleepinessImportance = UnityEngine.Random.Range(0f,1f);
      
//         traits.reproductiveImportance = UnityEngine.Random.Range(0f,1f);

//     }
    
//     private void OnDrawGizmos() {
//         if (showGizmos) {
//             Gizmos.color = Color.blue;
//             Gizmos.DrawWireSphere(transform.position,traits.sightRange);
//         }
//     }

//     #region  Decisions
//     /* Decision Complex */

    
//     public void Fight(GeneticEntity e) {

//     }


//     //Needs to flee away from a position to a point with minimal danger
//     public void Flight(Vector3 position) {
//         Action<string> completionReturn = new Action<string>(CompletedAction);
//         Vector3 p = GameObject.FindObjectOfType<MapManager>().GetRandomPointAwayFrom(position, traits.sightRange);
//         controller.MoveTo(p,traits.speed,completionReturn,"flightCompleted",0);

//         currentlyPerformingAction = true;
//         timeSinceAction = 0;
//     }

    
//     public float EnergyMovementCalculation (float movementSpeed) {
//         //NOTE: This is a temporary function        
//         return movementSpeed*traits.size+state.age;
//     }

//     public float EnergyMovementCalculation (float movementSpeed, float d) {
//         //NOTE: This is a temporary function        
//         return d*(movementSpeed*traits.size-Mathf.Pow(state.age,2));
//     }

//     float t = 0f;
//     private void StateUpdate() {
//         t+=Time.deltaTime;
//         //TODO: These are really simple at the moment and more complex behaviours will have to emerge
        
//         state.energy = Mathf.Clamp(state.energy,0f,100f);
        
//         state.age += 0.2f * Time.deltaTime;

//         if (state.age >= 100) {
//             Death();
//         }
        
//         if (enemies != null)
//         state.fear = Mathf.Clamp(enemies.Count*10 - traits.strength/2 + traits.dangerSense/3,0f,100f);
        
//         if (state.energy <= 50) {
//             state.sleepiness = Mathf.Clamp(state.sleepiness+2*Time.deltaTime,0f,100f);
//         }

//         if (state.age <= 50 && state.age >= 25) {
//             state.reproductiveness += 3 * Time.deltaTime;
//         }
//         else if (state.age >= 50 && state.age <= 75) {
//             state.reproductiveness -= 3 * Time.deltaTime;
//         }
      

//         state.hunger = Mathf.Clamp(state.hunger + (Time.deltaTime*0.2f*traits.size),0f,100f);

//         StateActionConversion();
//     }

//     private void StateActionConversion() {

//         if (state.hunger >= 90) {
//             //Force eat (until it finds food reduce health and energy)
//             state.health -= 2 * Time.deltaTime;
//             state.energy -= 2 * Time.deltaTime;
//         }
        
//         if (state.sleepiness >= 90) {
//             state.sleepiness = 100;
//             //Force sleep (until it finds a place to sleep & settles in reduce health)
//             state.health -= 2 * Time.deltaTime;
//             state.energy -= 2 * Time.deltaTime;
//         }

//         if (state.reproductiveness >= 100) {
//             //Force reproduction
//         }

//         if (state.energy <= 5) {
//             state.health -= 2 * Time.deltaTime;
//         }

//         if (state.health <= 0) {
//             Death();
//         }

//         // help.Plot(t,state.age,"Age");
//         // help.Plot(t,state.energy,"Energy");
//         // help.Plot(t, state.fear,"Fear");
//         // help.Plot(t, state.sleepiness,"Sleepiness");
//         // help.Plot(t, state.hunger,"Hunger");
//         // help.Plot(t, state.reproductiveness,"Reproductive Urge");

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

//     //Squishes Slightly and Particle of Zzz coming out of it -- Needs to go to a 'resting spot' that's considered safe 
//     //How does it find a safe spot? -- checks it's current sight range and if it is free of danger (keeps roaming) then it will go to sleep. 
//     //It's current status should be shown above [Looking for sleep]
//     public void Sleep() {

//     }

//     public void Nothing() {

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


//     public GeneticEntity Generate(GeneticEntity b) {

//         return null;
//     }

//     //Roam just roams around randomly
//     public void Roam() {
//         Action<string> completionReturn = new Action<string>(CompletedAction);
//         Vector3 p = GameObject.FindObjectOfType<MapManager>().GetRandomPointAwayFrom(transform.position, traits.sightRange);
//         controller.MoveTo(p,traits.speed/2,completionReturn,"roamCompleted",0);

//         currentlyPerformingAction = true;
//         timeSinceAction = 0;
//     }

//     //Current Visual Cortex Information
//     List<GeneticEntity> enemies;
//     List<Transform> food;
//     List<GeneticEntity> friends;
//     //NOTE: This is only a test, soon you will need a more diverse range of outputs (struct)
//     public void SensoryUpdate(List<GeneticEntity> enemies, List<Transform> food, List<GeneticEntity> friends) {
//         if (currentlyPerformingAction) {
//             //Nothing happens when it's performing an action...
// //            print("Unable to recieve sensory data, it's currently performing an action");
//             //FIXME: This is only here for testing purposes, should be removed later on
//             this.enemies = enemies;
//             this.food = food;
//             this.friends = friends;
//             return;
//         }
//         //Do the network code here
//         //print ("Detected : " + enemies.Count + " enemies, " + friends.Count + " friends, " + food.Count + " items!");
//         this.enemies = enemies;
//         this.food = food;
//         this.friends = friends;
//     }


//     #endregion

//     bool doneOnce = false;
//     private void Update() {
//         if (isRunningNetwork) {
//             for (int i = 0; i < brain.tags.Count; i++) {
//                 GameObject[] gameObjects = GameObject.FindGameObjectsWithTag(brain.tags[i]);
//                 SenseNetwork net = brain.correspondingNetwork[i];
//                 // net.network.Initialise(2,4,5,net.possibleActions.Count);

//                 float maxDist = 0;

//                 for (int t = 0; t < gameObjects.Length; t++) {
//                     if (Vector3.Distance(gameObjects[t].transform.position,transform.position) > maxDist) {
//                         maxDist = Vector3.Distance(gameObjects[t].transform.position,transform.position);
//                     }

//                 }
                

//                 for (int t = 0; t < gameObjects.Length; t++) {    
//                     print (net.outputValues.Count);     
//                     List<float> inputs = new List<float>();
//                     float dist = Vector3.Distance(transform.position,gameObjects[t].transform.position)/10;
//                     inputs.Add(state.age);
//                     inputs.Add(state.fear);
//                     inputs.Add(state.health);
//                     inputs.Add(state.hunger);
//                     inputs.Add(dist);
//                     inputs.Add(state.sleepiness);
                    
//                     net.inputValues = inputs;
//                     List<float> outputs = net.network.RunNetwork(inputs);
//                     net.outputValues.Clear();
//                     for (int r = 0; r < outputs.Count; r++) {
//                         net.outputValues.Add(outputs[r]);
//                     }
//                     net.outputValues = outputs;
//                     int best = 0;
//                     for (int d = 0; d < outputs.Count; d++) {
//                         // print (net.possibleActions[d] + " : " + outputs[d]);
//                         if (outputs[best] < outputs[d]) {
//                             best = d;
//                         }
//                     }

//                     //print ("For the tag : " + brain.tags[i] + " and the GameObject : " + gameObjects[t].name + " we chose to : " + net.possibleActions[best]);
//                 }   

//             }
//         }
//         else {
            
//             StateUpdate();

//         }
//     }
    
}

//The core traits that make up a genetic character
// [System.Serializable]
// public class GeneticTraits
// {   
//     [Range(0.1f,2f)]
//     public float surroundingCheckCooldown;
//     [Range(0.5f, 5f)]
//     public float decisionCoolDown;
//     [Range(0.01f,100)]
//     public float speed;
//      [Range(0.01f,100)]
//     public float size;
//      [Range(0.01f,100)]
//     public float attractiveness;
//      [Range(0.01f,100)]
//     public float sightRange;
//      [Range(0.01f,100)]
//     public float hearingRange;
//      [Range(0.01f,100)]
//     public float dangerSense;
//      [Range(0.01f,100)]
//     public float strength;
//      [Range(0.01f,100)]
//     public float heatResistance;
//      [Range(0.01f,100)]
//     public float intellect;
//      [Range(0.01f,100)]
//     public float brute;
//      [Range(0.01f,100)]
//     public float lifespan;
//     [Range(1f, 100f)]
//     public float healthImportance;
//     [Range(1f, 100f)]
//     public float ageImportance;
//     [Range(1f, 100f)]
//     public float fearImportance;
//     [Range(1f, 100f)]
//     public float hungerImportance;
//     [Range(1f, 100f)]
//     public float sleepinessImportance;
//     [Range(1f, 100f)]
//     public float reproductiveImportance;

// }

// [System.Serializable]
// public struct CurrentState {
//     [Range(0f, 100f)]
//     public float energy;
//     [Range(0f, 100f)]
//     public float health;
    
//     [Range(0f, 100f)]
//     public float age;
    
//     [Range(0f, 100f)]
//     public float fear;
    
//     [Range(0f, 100f)]
//     public float hunger;
    
//     [Range(0f, 100f)]
//     public float sleepiness;
    
//     [Range(0f, 100f)]
//     public float reproductiveness;
    
// }