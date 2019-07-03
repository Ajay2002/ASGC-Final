using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ActionManager : MonoBehaviour
{
    [Header("Action Elements")]
    public float movementCost = 0.2f;
    
    public string subState = "";
    public bool goalAccomplished = true;

    public enum ActionState {
        Eating,
        Sleeping,
        Fighting,
        Running,
        Breeding,
        Nothing
    }

    public ActionState currentState;

    [Header("Components")]
    public EntityManager entity;
    public StateManager stateManager;
    //public NavMeshAgent  agent;

    [HideInInspector]
    private bool movementProcessed;

    private void Awake() {
        entity.manager = GameObject.FindObjectOfType<MapManager>();
        //agent = GetComponent<NavMeshAgent>();
        
    }

    private void Start() {
       
    }

    ActionTemplate currentAction;

    private void Update() {
        
        MovementUpdate();

        if (currentAction != null & currentState != ActionState.Nothing) {
            currentAction.Update();
        }

    }

    public void Eat(bool begin) {

        if (begin) {
            currentState = ActionState.Eating;
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

    public void ActionCompletion() {
        //agent.ResetPath();
        movementProcessed = true;
        subState = "";
        currentState = ActionState.Nothing;
        currentAction = null;

        entity.SuccessfulAction("");

       

    }

    public void StopMovement() {
        movementProcessed = true;
        target = transform.position;
    }


    #region  Eating Management
    
    private void OnCollisionEnter (Collision col) {
        if (col.transform.tag == "Food") {
            if (entity.type == GTYPE.Creature) {
                stateManager.EatState();
                if (entity.food.Contains(col.transform))
                    entity.food.Remove(col.transform);
                GameObject.Destroy(col.transform.gameObject);

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
    
    //TODO: Soon this will use A* however for now we will use the Navigation.AI
	public void MoveTo (Transform target, float speed, string invocationTarget, float distPerc)
	{
        // if ( agent.isOnNavMesh) {
        //     agent.ResetPath();
            this.target = target.position;
            // agent.speed = speed;
            // agent.SetDestination(target.position + (transform.position - target.position) * distPerc);
            movementProcessed = false;
            invocationStatement = invocationTarget;
        //}
    }

	public void MoveTo (Vector3 target, float speed, string invocationTarget, float distPerc)
	{   
        // if ( agent.isOnNavMesh) {
        //     agent.ResetPath();
            this.target = target;
            // agent.speed = speed;
            // agent.SetDestination(target + (transform.position - target) * distPerc);
            movementProcessed = false;
            invocationStatement = invocationTarget;
        //}
    }

    private void MovementUpdate ()
	{
		if (!movementProcessed && currentAction != null)
		if (Vector3.Distance(transform.position,target) <= 0.2f )
		{
            currentAction.MovementComplete(invocationStatement);
            movementProcessed = true;
		}
		else
		{
            Vector3 d = (target-transform.position).normalized*entity.traits.speed*Time.deltaTime;
            d.y = 0;
            transform.position += d;
			stateManager.state.energy -= stateManager.EnergyMovementCalculation(entity.traits.speed) * Time.deltaTime * movementCost;
		}
	}

    #endregion
    
    #region  Sensory Handling
    public void FOVChecker (float rate, float sensoryDistance)
	{	
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

    #endregion
    
}

public abstract class ActionTemplate {

    public EntityManager manager;

    public abstract void Begin(EntityManager m);
    public abstract void Completion();
    public abstract void Update();
    public abstract void MovementComplete (string statement);

}


//Go after the random point
//If something better comes along - go towards it
//If it becomes null or it can't make it to the new one it reverts back to original
public class CreatureEatingAction : ActionTemplate {

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
        manager.controller.Eat(false);
    }

    Transform foodItem;

    public override void Update() {

        if (Vector3.Distance(manager.transform.position,randomPointOnMap) <= 0.5f) {
            Completion();
        }

        if (currentState == "movingToTarget") {

            if (manager.food.Count > 0) {
                if (manager.food[0] != null) {

                    currentState = "movingToSeperate";
                    foodItem = manager.food[0];
                    manager.controller.MoveTo(foodItem.position,manager.traits.speed,"goingToFood",0f);
                    
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

                manager.stateManagement.EatState();
                GameObject.Destroy(foodItem.gameObject);
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
        this.m = m;
        foundSafePlaceToSleep = false;
        randomPoint = m.manager.GetRandomPointAwayFrom(m.position,m.traits.sightRange);
        m.controller.MoveTo(randomPoint,m.traits.speed,"reachedRandomPoint",0f);
        currentState = "lookingForPlaceToSleep";
    }

    float sleepTimer = 0f;

    public override void Update() {

        if (Vector3.Distance(m.position,randomPoint) <= 0.5f && currentState == "lookingForPlaceToSleep") {
            randomPoint = m.manager.GetRandomPointAwayFrom(m.position,m.traits.sightRange);
            m.controller.MoveTo(randomPoint,m.traits.speed,"reachedRandomPoint",0f);
            currentState = "lookingForPlaceToSleep";
        }

        if (currentState == "lookingForPlaceToSleep") {
            
            if (m.type == GTYPE.Creature) {
                if (m.enemies.Count <= 0) {
                    
                    m.controller.StopMovement();
                    currentState = "sleeping";
                    sleepTimer = 2f;
                    //Debug.Log("A");

                }
            }
            else {
                if (m.creatures.Count <= 0) {
                    
                    m.controller.StopMovement();
                    currentState = "sleeping";
                    sleepTimer = 2f;
            
                   // Debug.Log("B");
                }
            }

            m.controller.subState = "lookingForPlaceToSleep";
        }

        if (currentState == "sleeping") {

            sleepTimer -= Time.deltaTime;

            if (sleepTimer <= 0f) {
                m.stateManagement.AquiringSleep();
                Completion();
//                Debug.Log("Reaching Sleep");
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
                if (manager.creatures[0] != null) {
                    
                    currentState = "movingToSeperate";
                    foodItem = manager.creatures[0];
                    manager.controller.MoveTo(foodItem.position,manager.traits.speed,"goingToFood",0f);
                    foodItem.stateManagement.Pursuit(manager);
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