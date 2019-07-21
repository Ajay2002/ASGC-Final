using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
//using Unity.Entities;

/* Extra Things 

1. Removed the move to for breeding
2. Removed something in the eating actions :/

 */
public class ActionManager : MonoBehaviour
{
    [Header("Action Elements")]
    public float movementCost = 0.2f;
    public Animator controller;

    public string subState = "";
    public bool goalAccomplished = true;

   

    public ActionState currentState;

    [Header("Components")]
    public EntityManager entity;
    public StateManager stateManager;
    public NavMeshAgent  agent;

    [HideInInspector]
    private bool movementProcessed;

    private void Awake() {
        
        
        agent = GetComponent<NavMeshAgent>();
        //agent.updatePosition=false;
    }

    private void Start() {
       entity.manager = GameObject.FindObjectOfType<MapManager>();
    }

    ActionTemplate currentAction;

    private Vector3 lastPosition;
    private void Update() {

        
        if (currentAction != null && currentState != ActionState.Nothing)
        MovementUpdate();

        if (currentAction != null && currentState != ActionState.Nothing) {
            currentAction.Update();
        }

        avgVelocity = (transform.position-lastPosition)/Time.deltaTime;
        lastPosition = transform.position;

        if (avgVelocity.magnitude <= 0.1f) {
            controller.SetBool("still",true);
        } else {controller.SetBool("still",false);}

        if (subState=="waitingForM") {
            if(m2==null)
                ActionCompletion();
        }


    }

    public Vector3 avgVelocity;

    public void Eat(bool begin) {
        currentState = ActionState.Eating;
        if (begin) {
            
            if (entity.type == GTYPE.Creature) {
                currentAction = new CreatureEatingAction();
                currentAction.Begin(entity);
            }
            else {
                currentAction = new PredatorEatingAction();
                currentAction.Begin(entity);
            }
        }
        else {
            ActionCompletion();
        }

    }

    public void Sleep (bool begin) {

        currentState = ActionState.Sleeping;

        if (begin) {
            
            currentAction = new EntitySleepingAction();
            currentAction.Begin(entity);

        }
        else {
            ActionCompletion();
        }

    }

    public void DrinkWater (bool begin) 
    {
        currentState = ActionState.Drinking;
        if (begin == true) {

            currentAction = new DrinkingAction();
            currentAction.Begin(entity);

        }
        else {
            ActionCompletion();
        }
    }

    public void Flight(bool begin) {
        currentState = ActionState.Running;
        if (begin) {
            currentAction = new FlightAction();
            currentAction.Begin(entity);
        }   
        else {
            ActionCompletion();
        }
    }

    public void Breed (bool begin) {
        currentState = ActionState.Breeding;
        if (begin == true) {

            currentAction = new BreedingAction();
            currentAction.Begin(entity);

        }
        else {
            ActionCompletion();
        }
    }

    //Breed request from 'm'
    public bool BreedRequest (EntityManager m) {
        if (m != null)
        if (m.stateManagement.fitness >= entity.manager.GetAverageFitness(m.type)) {

            if (m.traits.attractiveness >= entity.traits.attractiveness-0.4f) {
                return true;
            }

        }

        return false;

    }

    EntityManager m2;
    
    public void BreedConfirmed(EntityManager m) {
        m2=m;
        subState="waitingForM";
        StopMovement();
        transform.LookAt(m.transform.position);
        subState = "";
        currentState = ActionState.Breeding;
        
    }

    
    // /target.position + (transform.position - target.position) * distPerc
    public void ActionCompletion() {
        if (agent.isOnNavMesh)
        agent.ResetPath();
    
        movementProcessed = true;
        subState = "";
        currentState = ActionState.Nothing;
        currentAction = null;

        entity.SuccessfulAction("");
 
    }

    public void StopMovement() {
        movementProcessed=true;
        agent.ResetPath();
    }

    public void ForceBreed (EntityManager e) {
        entity.decision.currentlyPerformingAction = true;
        ActionCompletion();
        currentState = ActionState.Breeding;
        BreedingAction a = new BreedingAction();
        currentAction = a;
        a.BeginForce(this.entity,e);
    }

    #region  Eating Management
    
    private void OnCollisionEnter (Collision col) {
        if (col.transform.tag == "Food") {
            if (entity.type == GTYPE.Creature) {
                if (col.transform.GetComponent<Food>() != null) {
                    
                    col.transform.GetComponent<Food>().Eat();
                    stateManager.EatState(col.transform.GetComponent<Food>());

                }
                else
                    stateManager.EatState();
                if (entity.food.Contains(col.transform))
                    entity.food.Remove(col.transform);
                
                //GameObject.Destroy(col.transform.gameObject);

                if (currentState == ActionState.Eating && currentAction != null)
                    currentAction.Completion();
            }

        }

        else if (col.transform.tag == "Player") {
            
        }
        else if (col.transform.tag == "Enemy") {

        }
    }
    
    #endregion
    
    #region  Movement

    string invocationStatement = "";
    Vector3 target;
    
    List<Vector3> currentPath = new List<Vector3>();
    //TODO: Soon this will use A* however for now we will use the Navigation.AI
	public void MoveTo (Transform target, float speed, string invocationTarget, float distPerc)
	{       
            this.target = target.position;
            agent.speed = speed;
            
            movementProcessed = false;
            invocationStatement = invocationTarget;
            StopAndReset();
    }

    bool foundPath = false;
    public void FoundPath (List<PathNode> nodes) {
        for (int i = 0; i < nodes.Count; i++) {
            currentPath.Add(nodes[i].transform.position);
        }

        currentPath.Add(target);
        foundPath = true;
    }

	public void MoveTo (Vector3 target, float speed, string invocationTarget, float distPerc)
	{   
            this.target = target ;
            agent.speed = speed;
            
            movementProcessed = false;
            invocationStatement = invocationTarget;
            StopAndReset();
    }

    Vector3 look;
    int currentPathPoint = 0;
    float timeWithoutPath = 0f;

    protected float _distanceStearingTarget;
    public float failsafeSpeed=0.5f;
    private void MovementUpdate ()
	{
		if (!movementProcessed && currentAction != null) {
            if (Vector3.Distance(transform.position,target) <= 0.9f || (agent.pathPending == false && agent.remainingDistance <= 0.6f))
            {
                controller.speed=1f;
                currentAction.MovementComplete(invocationStatement);
                movementProcessed = true;
            }
            else
            {
            
                controller.speed=entity.traits.speed/2;
                stateManager.state.energy -= stateManager.EnergyMovementCalculation(entity.traits.speed) * movementCost * Time.deltaTime;
                
            }

	    }
    }

    public void GotPath (NavMeshPath queryReturn) {
        if (agent.isOnNavMesh)
        agent.ResetPath();
        //agent.isStopped = false;

        if (queryReturn.status == NavMeshPathStatus.PathInvalid)
        {ActionCompletion(); return;}    
        
        
        agent.SetPath(queryReturn);

    }

    public void StopAndReset () {
        //agent.isStopped = true;
        NavMeshManager.Instance.RequestPath(new NavMeshQuery(transform,GotPath,agent, target,entity.traits.speed));
    }

    #endregion
    
    #region  Sensory Handling
    public void FOVChecker (float rate, float sensoryDistance)
	{	
        if (currentState != ActionState.Nothing && currentAction != null) {
            Collider[] distanceCheck = Physics.OverlapSphere(transform.position, sensoryDistance);

            List<EntityManager> enemies = new List<EntityManager>();
            List<EntityManager> player  = new List<EntityManager>();
            List<Transform>     food    = new List<Transform>();

            for (int i = 0; i < distanceCheck.Length; i++)
            {
                if (distanceCheck[i].transform == transform || distanceCheck[i].transform.root == transform)
                    continue;

                //TODO: Remember to add and make sure it is in the FOV
                string tag = distanceCheck[i].gameObject.tag;

                if (tag.Equals("Enemy"))
                {
                    enemies.Add(distanceCheck[i].transform.root.GetComponent<EntityManager>());
                }
                else if (tag.Equals("Player"))
                {
                    player.Add(distanceCheck[i].transform.root.GetComponent<EntityManager>());
                }
                else if (tag.Equals("Food"))
                {
                    food.Add(distanceCheck[i].transform);
                }
            }

            entity.SensoryUpdate(enemies, food, player);
        }
        else {
            entity.SensoryUpdate(entity.enemies, entity.food, entity.creatures);
        }

		
	}

    #endregion
    
}

 public enum ActionState {
        Eating,
        Sleeping,
        Fighting,
        Running,
        Breeding,
        Drinking,
        Nothing
    }
public abstract class ActionTemplate {

    public EntityManager manager;

    public abstract void Begin(EntityManager m);
    public abstract void Completion();
    public abstract void Update();
    public abstract void MovementComplete (string statement);

}

public class DrinkingAction : ActionTemplate {
    bool reachedDestination = false;
    Vector3 randomPointOnMap = new Vector3();
    bool movingToWater = true;

    public override void Begin(EntityManager m) {
        manager = m;
       movingToWater = true;
       randomPointOnMap = m.manager.NearestPointOnMapWater(m.manager.GetRandomPoint());
        reachedDestination = false;
        if (movingToWater)
        Debug.DrawLine(randomPointOnMap,randomPointOnMap+Vector3.up*100,Color.magenta,20);
        else
        Debug.DrawLine(randomPointOnMap,randomPointOnMap+Vector3.up*100,Color.red,20);

        m.controller.MoveTo(randomPointOnMap,m.traits.speed,"goingToTarget",0f);
        currentState = "movingToTarget";
    }

    public override void Completion() {
        manager.controller.DrinkWater(false);
    }

    public override void Update() {
        if (currentState == "movingToTarget") {
            if (manager.controller.avgVelocity.magnitude <= 0.05f) {
                manager.controller.MoveTo(randomPointOnMap,manager.traits.speed,"goingToTarget",0f);
            }
        }
        if (Vector3.Distance(manager.transform.position,randomPointOnMap) <= 0.6f) {
            Completion();
        }

    }

    string currentState = "";

    public override void MovementComplete (string statement) {
        if (statement == "goingToTarget") {
            Completion();
        }
        else if (statement == "goingToFood") {
            
                if (movingToWater) {
                    manager.stateManagement.DrankWaterState();
                }
                Completion();

            }
            else {

                manager.controller.MoveTo(randomPointOnMap,manager.traits.speed,"goingToTarget",0f);
                currentState = "movingToTarget";

            }

        }
}



//Go after the random point
//If something better comes along - go towards it
//If it becomes null or it can't make it to the new one it reverts back to original
public class CreatureEatingAction : ActionTemplate {

    bool reachedDestination = false;
    Vector3 randomPointOnMap = new Vector3();

    public override void Begin(EntityManager m) {
        manager = m;
        randomPointOnMap = m.manager.GetRandomPoint();
        reachedDestination = false;
        m.controller.MoveTo(randomPointOnMap,m.traits.speed,"goingToTarget",0f);
        currentState = "movingToTarget";
    }

    public override void Completion() {
        manager.controller.Eat(false);
    }

    Transform foodItem;

    public override void Update() {
        if (currentState == "movingToTarget") {
            if (manager.controller.avgVelocity.magnitude <= 0.05f) {
                manager.controller.MoveTo(randomPointOnMap,manager.traits.speed,"goingToTarget",0f);
            }
        }
        if (Vector3.Distance(manager.transform.position,randomPointOnMap) <= 0.6f) {
            Completion();
        }

        if (currentState == "movingToTarget") {

            if (manager.food.Count > 0) {
                for (int i = 0; i <manager.food.Count; i++) {
                if (manager.food[i] != null  && manager.food[i].GetComponent<Food>().lockedOn == null) {
                    manager.food[i].GetComponent<Food>().lockedOn = manager;
                    currentState = "movingToSeperate";
                    foodItem = manager.food[i];
                    manager.controller.MoveTo(foodItem.position,manager.traits.speed,"goingToFood",0f);
                    
                }
                }
            }



        }

        if (currentState == "movingToSeperate") {
            if (foodItem == null) {
                manager.controller.MoveTo(randomPointOnMap,manager.traits.speed,"goingToTarget",0f);
                currentState = "movingToTarget";
            }
        }
    }

    string currentState = "";

    public override void MovementComplete (string statement) {
        if (statement == "goingToTarget") {
            Completion();
        }
        else if (statement == "goingToFood") {
            
            if (foodItem != null) {

                //TODO: Change to use the Food component attached to the gameobject
                //TODO: Use the food to determine how much energy to gain and hunger to lose
                //TODO: as well as use it to destroy the gameobject.
                if (foodItem.GetComponent<Food>() != null) {
                    manager.stateManagement.EatState(foodItem.GetComponent<Food>());
                    foodItem.GetComponent<Food>().Eat();
                }
                else {
                    manager.stateManagement.EatState();
                    GameObject.Destroy(foodItem.gameObject);
                }
                

                Completion();

            }
            else {

                manager.controller.MoveTo(randomPointOnMap,manager.traits.speed,"goingToTarget",0f);
                currentState = "movingToTarget";

            }

        }
    }

}

public class EntitySleepingAction : ActionTemplate {
    
    bool foundSafePlaceToSleep = false;
    Vector3 randomPoint;

    EntityManager m;
    string currentState = "";

    public override void Begin(EntityManager m) {
        if (m == null)
        Completion();
        this.m = m;
        foundSafePlaceToSleep = false;
        randomPoint = m.manager.GetRandomPointAwayFrom(m.position,m.traits.sightRange);
        m.controller.MoveTo(randomPoint,m.traits.speed,"reachedRandomPoint",0f);
        currentState = "lookingForPlaceToSleep";
    }

    float sleepTimer = 0f;

    public override void Update() {

        if (currentState == "lookingForPlaceToSleep") {
            if (m.controller.avgVelocity.magnitude <= 0.05f) {
                m.controller.MoveTo(randomPoint,m.traits.speed,"reachedRandomPoint",0f);
            }
        }

        if (Vector3.Distance(m.position,randomPoint) <= 0.7f && currentState == "lookingForPlaceToSleep") {
            randomPoint = m.manager.GetRandomPointAwayFrom(m.position,m.traits.sightRange);
            m.controller.MoveTo(randomPoint,m.traits.speed,"reachedRandomPoint",0f);
            currentState = "lookingForPlaceToSleep";
        }

        if (currentState == "lookingForPlaceToSleep") {
            
            if (m.type == GTYPE.Creature) {
                if (m.enemies.Count <= 0) {
                    
                    m.controller.StopMovement();
                    currentState = "sleeping";
                    sleepTimer = 5f;
                }
            }
            else {
                if (m.creatures.Count <= 0) {
                    
                    m.controller.StopMovement();
                    currentState = "sleeping";
                    sleepTimer = 5f;
                }
            }

            m.controller.subState = "lookingForPlaceToSleep";
        }

        if (currentState == "sleeping") {

            sleepTimer -= Time.deltaTime;

            if (sleepTimer <= 0f) {
                m.stateManagement.AquiringSleep();
                Completion();
            }
            
            m.controller.subState = "sleeping";
        }


    }

    public override void MovementComplete (string statement) {
        if (statement == "reachedRandomPoint") {
            //Didn't find a place to sleep.. go again
            randomPoint = m.manager.GetRandomPointAwayFrom(m.position,m.traits.sightRange);
            m.controller.MoveTo(randomPoint,m.traits.speed,"reachedRandomPoint",0f);
            currentState = "lookingForPlaceToSleep";
        }
    }

    public override void Completion() {
        m.controller.Sleep(false);
    }

}
// No pursuit just increase fear beyond measure

public class PredatorEatingAction : ActionTemplate {

    string currentState = "";
    EntityManager chasing;
    EntityManager manager;
    Vector3 randomPointOnMap;


    public override void Begin(EntityManager m) {
        manager = m;
        randomPointOnMap = m.manager.GetRandomPointAwayFrom(m.transform.position,m.traits.sightRange);
        m.controller.MoveTo(randomPointOnMap,m.traits.speed,"goingToTarget",0f);
        currentState = "movingToTarget";
        m.controller.subState = "randomPointMovement";
    }

    EntityManager foodItem;
    public override void Update() {
        if (Vector3.Distance(manager.transform.position,randomPointOnMap) <= 0.5f) {
            Completion();
            manager.controller.subState = "reachedAtDistance";
            return;
        }

        if (foodItem != null)
        if (Vector3.Distance(manager.position,foodItem.position) < 0.6f) {
            manager.stateManagement.EatState();
            GameObject.Destroy(foodItem.gameObject);
            Completion();
            manager.controller.subState = "ateATDistance";
            return;
        }

        if (currentState == "movingToTarget") {

            if (manager.creatures.Count > 0) {
                for (int i = 0; i < manager.creatures.Count; i++) {
                    if (manager.creatures[i] != null && manager.creatures[i].claimedBy == null) {
                        manager.creatures[i].claimedBy = this.manager;
                        currentState = "movingToSeperate";
                        foodItem = manager.creatures[0];
                        manager.controller.MoveTo(foodItem.position,manager.traits.speed,"goingToFood",0f);
                        foodItem.stateManagement.Pursuit(manager);
                    }
                }
            }

             manager.controller.subState = "randomMovementTarge";

        }
        else if (currentState == "movingToSeperate") {
            if (foodItem == null) {
                manager.controller.MoveTo(randomPointOnMap,manager.traits.speed,"goingToTarget",0f);
                currentState = "movingToTarget";
                manager.controller.subState = "updateRandomPointMovement";
            }
            else {
                
                if (Vector3.Distance(manager.position,foodItem.position) < manager.traits.sightRange) {
                    manager.controller.MoveTo(foodItem.transform.position,manager.traits.speed,"reachedFood",0.5f);
                    foodItem.stateManagement.Pursuit(manager);
                    manager.controller.subState = "inPursuit";
                }
                else {
                    //FIXME: Not sure if this is stable yet :/
                    foodItem.claimedBy = null;
                    manager.controller.MoveTo(randomPointOnMap,manager.traits.speed,"goingToTarget",0f);
                    currentState = "movingToTarget";
                    manager.controller.subState = "backToRandomPointMovement";
                }

            }
        }
        else {
            manager.controller.MoveTo(randomPointOnMap,manager.traits.speed,"goingToTarget",0f);
            currentState = "movingToTarget";
            manager.controller.subState = "backBackToRandomPointMovement";
        }
    }

    public override void Completion() {
        manager.controller.Eat(false);
    }

    public override void MovementComplete(string statement) {
        if (statement == "goingToTarget") {
            Completion();
            manager.controller.subState = "finishedGoingToRandomPoint";
        }
        else if (statement == "goingToFood") {
            
            if (foodItem != null) {
                manager.controller.subState = "ateAtFinish";
                manager.stateManagement.EatState();
                GameObject.Destroy(foodItem.gameObject);
                Completion();

            }
            else {

                manager.controller.MoveTo(randomPointOnMap,manager.traits.speed,"goingToTarget",0f);
                currentState = "movingToTarget";
                manager.controller.subState = "goAgain";

            }

        }
    }


}

public class BreedingAction : ActionTemplate {

    public string state;
    EntityManager manager;
    EntityManager mate;
    Vector3 randomPoint;
    //Request from higher fitness
    //Evaluates based on attractiveness
    public override void Begin(EntityManager m) {

        manager = m;
        //manager.stateManagement.state.age = 30;
        state = "lookingForMate";
        randomPoint = m.manager.GetRandomPointAwayFrom(m.position,m.traits.sightRange);
        m.controller.MoveTo(randomPoint,m.traits.speed,"reachedRandomPoint",0f);

    }

    public void BeginForce (EntityManager m, EntityManager other) {
        manager = m;
        mate = other;



         if (manager == null || mate == null)
            {Debug.Log("Illegal Mating Conditions, Send Error Warning -- doesn't exist");return;}

        if (manager.bredWith.Contains(mate))
            {Debug.Log("Illegal Mating Conditions, Send Error Warning -- mated before");return;}
        
        if (mate.stateManagement.state.age < 20 || manager.stateManagement.state.age < 20)
            {Debug.Log("Illegal Mating Conditions, Send Error Warning -- age");return;}

        if (manager.parentA != null && manager.parentB != null) {
            if (mate == manager.parentA) {
                Debug.Log("Illegal Mating Conditions, Send Error Warning -- similar parents");return;}

            if (mate == manager.parentB){
                Debug.Log("Illegal Mating Conditions, Send Error Warning -- similar parents 2");return;}
    
            if (mate.parentA != null && mate.parentB != null) {
                if (mate.parentA == manager.parentA || mate.parentA == manager.parentB) {
                    Debug.Log("Illegal Mating Conditions, Send Error Warning -- similar parents 3");
                    return;
                    
                }
                if (mate.parentB == manager.parentB || mate.parentB == manager.parentB) {
                    Debug.Log("Illegal Mating Conditions, Send Error Warning -- 4");
                    return;

                }

                
                

            }

        }

        if (manager.parentA != null && manager.parentB != null) {
            if (mate.parentA == this.manager) {
                Debug.Log("Illegal Mating Conditions, Send Error Warning -- similar parents -- 5");
                return;
            }

            if (mate.parentB == this.manager) {
                Debug.Log("Illegal Mating Conditions, Send Error Warning -- similar parents -- 6");
                return;

            }
        }
        //Debug.Log("Began Mate");
        state = "mating";
        BeginForceMating();
    }

    public override void Completion() {
        manager.controller.Breed(false);
        mate=null;
        state="";
    }

    float timeSinceBreedReq = 0f;

    public override void Update() {
        
        if (mate == null) {
            state = "lookingForMate";
            manager.controller.MoveTo(randomPoint,manager.traits.speed,"reachedRandomPoint",0f);
        }

        if (Vector3.Distance(manager.position,randomPoint) <= 0.6f && state == "lookingForMate") {
            Completion();
        }

        if (state == "lookingForMate") {
            if (manager.controller.avgVelocity.magnitude <= 0.05f) {
                manager.controller.MoveTo(randomPoint,manager.traits.speed,"reachedRandomPoint",0f);
            }
        }

        if (state == "lookingForMate") {
            if (manager.type == GTYPE.Creature) {
                if (manager.creatures.Count > 0) {
                    bool foundBreeding = false;
                    EntityManager found = new EntityManager();
                    for (int i = 0; i < manager.creatures.Count; i++) {
                                  if (manager.creatures[i] == null)
                            {continue;}

                        if (manager.bredWith.Contains(manager.creatures[i]))
                            {continue;}
                        
                        if (manager.creatures[i].stateManagement.state.age < 10 || manager.stateManagement.state.age < 10)
                            {continue;}

                        
                        if (manager.parentA != null && manager.creatures[i] == manager.parentA)
                            continue;

                        if (manager.parentB != null && manager.creatures[i] == manager.parentB)
                            continue;

                        
                        if ((manager.creatures[i].parentA != null && manager.parentA != null && manager.creatures[i].parentA == manager.parentA) || (manager.creatures[i].parentA != null && manager.parentB != null && manager.creatures[i].parentA == manager.parentB))
                            continue;
                        if ((manager.creatures[i].parentB != null && manager.parentB != null && manager.creatures[i].parentB == manager.parentB) || (manager.creatures[i].parentB != null && manager.parentB != null && manager.creatures[i].parentB == manager.parentB))
                            continue;
                        
                        if (manager.creatures[i].parentA != null && manager.creatures[i].parentA == this.manager)
                            continue;

                        if (manager.creatures[i].parentB != null && manager.creatures[i].parentB == this.manager)
                        continue;

                        if (manager.stateManagement.fitness > manager.creatures[i].stateManagement.fitness)
                        continue;

                        foundBreeding = true;
                        found = manager.creatures[i];
                        break;
                    }

                    if (foundBreeding) {
                        //Send a request
                        if (found != null) {
                            //Debug.Log("Request Send");
                            // if (found.controller.BreedRequest(manager)) {
                            //     state = "mating";
                            //     mate = found;
                            //     BeginMating();
                            // }

                            mate=found;
                            if (manager.stateManagement.fitness >= mate.manager.GetAverageFitness(manager.type)) {

                                    if (manager.traits.attractiveness >= mate.traits.attractiveness-0.4f) {
                                        
                                        state = "mating";
                                        BeginMating();
                                    }

                            }
                            
                        }

                    }
                }
            }
            else if (manager.type == GTYPE.Predator) {
                 if (manager.enemies.Count > 0) {
                    bool foundBreeding = false;
                    EntityManager found = new EntityManager();
                    for (int i = 0; i < manager.enemies.Count; i++) {
                        if (manager.enemies[i] == null)
                            {continue;}

                        if (manager.bredWith.Contains(manager.enemies[i]))
                            {continue;}
                        
                        if (manager.enemies[i].stateManagement.state.age < 10 || manager.stateManagement.state.age < 10)
                            {continue;}

                        
                        if (manager.parentA != null && manager.enemies[i] == manager.parentA)
                            continue;

                        if (manager.parentB != null && manager.enemies[i] == manager.parentB)
                            continue;

                        
                        if ((manager.enemies[i].parentA != null && manager.parentA != null && manager.enemies[i].parentA == manager.parentA) || (manager.enemies[i].parentA != null && manager.parentB != null && manager.enemies[i].parentA == manager.parentB))
                            continue;
                        if ((manager.enemies[i].parentB != null && manager.parentB != null && manager.enemies[i].parentB == manager.parentB) || (manager.enemies[i].parentB != null && manager.parentB != null && manager.enemies[i].parentB == manager.parentB))
                            continue;
                        
                        if (manager.enemies[i].parentA != null && manager.enemies[i].parentA == this.manager)
                            continue;

                        if (manager.enemies[i].parentB != null && manager.enemies[i].parentB == this.manager)
                        continue;

                        if (manager.stateManagement.fitness > manager.enemies[i].stateManagement.fitness)
                        continue;

                        foundBreeding = true;
                        found = manager.enemies[i];
                        break;
                    }

                    if (foundBreeding) {
                        //Send a request
                        if (found != null) {
                            if (found.controller.BreedRequest(manager)) {
                                state = "mating";
                                mate = found;
                                BeginMating();
                            }
                        }

                    }
                }
            }

        }

        if (state == "mating") {
            timeSinceBreedReq += Time.deltaTime;
            if (timeSinceBreedReq >= 5) {
                mate.controller.ActionCompletion();
                Completion();
            }
        }

    }

    private void BeginMating() {

        //Change the state of the other to a breed wait
        //Move to other entity
        //Wait 1 second
        //Breed
        //mate.controller.BreedConfirmed(manager);
        manager.controller.MoveTo(mate.position,manager.traits.speed,"reachedMate",0.7f);
        state = "mating";
        Breed(mate);

    }

     private void BeginForceMating() {

        //Change the state of the other to a breed wait
        //Move to other entity
        //Wait 1 second
        //Breed
        
        mate.controller.BreedConfirmed(manager);
        //manager.controller.MoveTo(mate.position,manager.traits.speed,"reachedMate",0.7f);
        state = "mating";
        Breed(mate);

    }
    

    private void Breed (EntityManager e) {

        
        e.bredWith.Add(manager);
        manager.bredWith.Add(e);
        e.stateManagement.ReproductionState();
        manager.stateManagement.ReproductionState();

        Vector3 pos = manager.manager.GetRandomPointAwayFrom(manager.position,manager.traits.sightRange);
        //Vector3 pos = e.position + e.transform.forward*0.5f;
        EntityManager newEntity;
        if (GTYPE.Creature == manager.type)
        newEntity = manager.manager.SpawnEntity(pos, e).GetComponent<EntityManager>();
        else
        newEntity = manager.manager.SpawnEntityEnemy(pos, e).GetComponent<EntityManager>();
        

        if (GTYPE.Predator == manager.type && (manager.isPartOfWave || mate.isPartOfWave))
            newEntity.isPartOfWave = true;

        if (manager.isNeuralNet && e.isNeuralNet) {
        NNetwork A = e.network;
        NNetwork B = manager.network;
        
        NNetwork Child1 = new NNetwork();
        Child1.Initialise(manager.manager.hiddenLayer,manager.manager.hiddenNeuron,10,5);

        for (int w = 0; w < Child1.weights.Count; w++)
        {
            if (UnityEngine.Random.Range(0.01f,1.0f) < manager.manager.mutationChance) {
                for (int x = 0; x < Child1.weights[w].RowCount; x++)
                {
                    for (int y = 0; y < Child1.weights[w].ColumnCount; y++)
                    {
                        Child1.weights[w][x, y] = UnityEngine.Random.Range(-1f, 1f);
                    }
                }
            }
            else {
                if (UnityEngine.Random.Range(0.0f, 1.0f) < 0.5f)
                {//(Matrix<float> A, Matrix<float> B) = CrossOver(population[AIndex].weights[w], population[BIndex].weights[w]);
                    Child1.weights[w]=(A.weights[w]);
                }
                else {
                    Child1.weights[w]=(B.weights[w]);
                }
            }
        }

        for (int b = 0; b < Child1.biases.Count; b++)
        {
            if (UnityEngine.Random.Range(0.01f,1.0f) < manager.manager.mutationChance) {
                Child1.biases[b] = UnityEngine.Random.Range(-1f,1f);
            }
            else {
                if (UnityEngine.Random.Range(0.0f, 1.0f) < 0.5f)
                {
                    Child1.biases[b]=(A.biases[b]);
                }
                else
                {
                    Child1.biases[b] = (B.biases[b]);
                }
            }

        }

        newEntity.network = Child1;
        newEntity.isNeuralNet = manager.isNeuralNet;
        }

        
        //newEntity.traits.surroundingCheckCooldown = UnityEngine.Random.Range(0.01f,1.0f)<manager.manager.mutationChance ? UnityEngine.Random.Range(0.1f,2f)  : (UnityEngine.Random.Range(0.01f,1.0f)<0.5f ? manager.traits.surroundingCheckCooldown : e.traits.surroundingCheckCooldown);
        
        #region  Trait Modification
        newEntity.traits.surroundingCheckCooldown = UnityEngine.Random.Range(0.01f,1.0f)<manager.manager.mutationChance ? UnityEngine.Random.Range(0.1f,2f)  : (UnityEngine.Random.Range(0.01f,1.0f)<0.5f ? manager.traits.surroundingCheckCooldown : e.traits.surroundingCheckCooldown);
    
        newEntity.traits.decisionCoolDown = UnityEngine.Random.Range(0.01f,1.0f)<manager.manager.mutationChance ? UnityEngine.Random.Range(0.1f,1f) : (UnityEngine.Random.Range(0.01f,1.0f)<0.5f ? manager.traits.decisionCoolDown : e.traits.decisionCoolDown);
        
        newEntity.traits.speed = UnityEngine.Random.Range(0.01f,1.0f)<manager.manager.mutationChance ? (UnityEngine.Random.Range(0.01f,1f)*5) : (UnityEngine.Random.Range(0.01f,1.0f)<0.5f ? manager.traits.speed : e.traits.speed);
    
        newEntity.traits.size = UnityEngine.Random.Range(0.01f,1.0f)<manager.manager.mutationChance ? UnityEngine.Random.Range(0.01f,1f)*3 : (UnityEngine.Random.Range(0.01f,1.0f)<0.5f ? manager.traits.size : e.traits.size);
        
        newEntity.traits.attractiveness = UnityEngine.Random.Range(0.01f,1.0f)<manager.manager.mutationChance ? UnityEngine.Random.Range(0.01f,1f) : (UnityEngine.Random.Range(0.01f,1.0f)<0.5f ? manager.traits.attractiveness : e.traits.attractiveness);
    
        newEntity.traits.sightRange = UnityEngine.Random.Range(0.01f,1.0f)<manager.manager.mutationChance ? UnityEngine.Random.Range(0.01f,1f)*10 : (UnityEngine.Random.Range(0.01f,1.0f)<0.5f ? manager.traits.sightRange : e.traits.sightRange);
    
        newEntity.traits.dangerSense = UnityEngine.Random.Range(0.01f,1.0f)<manager.manager.mutationChance ? UnityEngine.Random.Range(0.01f,1f) : (UnityEngine.Random.Range(0.01f,1.0f)<0.5f ? manager.traits.dangerSense : e.traits.dangerSense);
    
        newEntity.traits.strength = UnityEngine.Random.Range(0.01f,1.0f)<manager.manager.mutationChance ? UnityEngine.Random.Range(0.01f,1f) : (UnityEngine.Random.Range(0.01f,1.0f)<0.5f ? manager.traits.strength : e.traits.strength);
    
        newEntity.traits.heatResistance = UnityEngine.Random.Range(0.01f,1.0f)<manager.manager.mutationChance ? UnityEngine.Random.Range(0.01f,1f) : (UnityEngine.Random.Range(0.01f,1.0f)<0.5f ? manager.traits.heatResistance : e.traits.heatResistance);
    
        newEntity.traits.intellect  =UnityEngine.Random.Range(0.01f,1.0f)<manager.manager.mutationChance ? UnityEngine.Random.Range(0.01f,1f) : (UnityEngine.Random.Range(0.01f,1.0f)<0.5f ? manager.traits.intellect : e.traits.intellect);
    
        newEntity.traits.brute = UnityEngine.Random.Range(0.01f,1.0f)<manager.manager.mutationChance ? UnityEngine.Random.Range(0.01f,1f) : (UnityEngine.Random.Range(0.01f,1.0f)<0.5f ? manager.traits.brute : e.traits.brute);
        
        newEntity.traits.HI = UnityEngine.Random.Range(0.01f,1.0f)<manager.manager.mutationChance ? UnityEngine.Random.Range(0f,1f) : (UnityEngine.Random.Range(0.01f,1.0f)<0.5f ? manager.traits.HI : e.traits.HI);
    
        newEntity.traits.AI = UnityEngine.Random.Range(0.01f,1.0f)<manager.manager.mutationChance ? UnityEngine.Random.Range(0f,1f) : (UnityEngine.Random.Range(0.01f,1.0f)<0.5f ? manager.traits.AI : e.traits.AI);

        newEntity.traits.FI = UnityEngine.Random.Range(0.01f,1.0f)<manager.manager.mutationChance ? UnityEngine.Random.Range(0f,1f) : (UnityEngine.Random.Range(0.01f,1.0f)<0.5f ? manager.traits.FI : e.traits.FI);
    
        newEntity.traits.HUI = UnityEngine.Random.Range(0.01f,1.0f)<manager.manager.mutationChance ? UnityEngine.Random.Range(0f,1f) : (UnityEngine.Random.Range(0.01f,1.0f)<0.5f ? manager.traits.HUI : e.traits.HUI);

        newEntity.traits.SI = UnityEngine.Random.Range(0.01f,1.0f)<manager.manager.mutationChance ? UnityEngine.Random.Range(0f,1f) : (UnityEngine.Random.Range(0.01f,1.0f)<0.5f ? manager.traits.SI : e.traits.SI);
    
        newEntity.traits.RI = UnityEngine.Random.Range(0.01f,1.0f)<manager.manager.mutationChance ? UnityEngine.Random.Range(0f,1f) : (UnityEngine.Random.Range(0.01f,1.0f)<0.5f ? manager.traits.RI : e.traits.RI);
        
        newEntity.traits.TI = UnityEngine.Random.Range(0.01f,1.0f)<manager.manager.mutationChance ? UnityEngine.Random.Range(0f,1f) : (UnityEngine.Random.Range(0.01f,1.0f)<0.5f ? manager.traits.TI : e.traits.TI);

        #endregion

        //State Resets
        newEntity.stateManagement.state.age = 0;
        newEntity.stateManagement.state.energy = 100;
        newEntity.stateManagement.state.fear = 0;
        newEntity.stateManagement.state.sleepiness = 0;
        newEntity.stateManagement.state.hunger = 0;
        newEntity.stateManagement.state.health = 100;
        newEntity.stateManagement.state.reproductiveness = 0f;

        

        newEntity.parentA = this.manager;
        newEntity.parentB = e;

         //State changing with regards to breeding
        e.stateManagement.ReproductionState();
        manager.stateManagement.ReproductionState();
        Completion();
        //e.controller.ActionCompletion();
    }

    public override void MovementComplete(string statement) {
        if (statement == "reachedRandomPoint") {

            Completion();
            if (mate!=null)
            mate.controller.ActionCompletion();
        }
        else if (statement == "reachedMate") {
            
            

        }
    }

}

public class FlightAction : ActionTemplate {

    bool reachedDestination = false;
    Vector3 randomPointOnMap = new Vector3();

    public override void Begin(EntityManager m) {
        manager = m;
        randomPointOnMap = m.manager.GetRandomPointAwayFrom(m.transform.position,m.traits.sightRange);
        reachedDestination = false;
        m.controller.MoveTo(randomPointOnMap,m.traits.speed,"goingToTarget",0f);
        currentState = "movingToTarget";
    }

    public override void Completion() {
        manager.controller.Flight(false);
    }

    Transform foodItem;

    public override void Update() {

        if (Vector3.Distance(manager.transform.position,randomPointOnMap) <= 0.5f) {
            Completion();
        }

    }

    string currentState = "";

    public override void MovementComplete (string statement) {
        if (statement == "goingToTarget") {
            Completion();
        }
    }

}