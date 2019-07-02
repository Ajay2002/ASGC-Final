using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ActionManager : MonoBehaviour
{
    [Header("Action Elements")]
    public float movementCost = 0.2f;

    [Header("Components")]
    public EntityManager entity;
    public StateManager stateManager;
    public NavMeshAgent  agent;

    [HideInInspector]
    public bool currentlyInAction = false;
    public string currentAction = "";
    
    private void Awake() {
        entity.manager = GameObject.FindObjectOfType<MapManager>();
    }

    private void Start() {
        
        agent  = GetComponent<NavMeshAgent>();
        
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.W)) {
            EatFood();
            //   EatFood();
        }

        if (currentlyInAction && currentAction != "") {    
            if (currentAction == "searchingForFood" ) {

                if (entity.food.Count > 0) {
                    if (entity.food[0] != null) {
                        print ("Finding?");
                        MoveTo(entity.food[0],entity.traits.speed,"movingToFood",0);
                        currentAction = "foodPursuit";
                       
                    }
                }
            }

            if (currentAction == "searchingForCreature" && entity.type == GTYPE.Predator) {
                if (entity.creatures.Count > 0) {
                    if (entity.creatures[0] != null)
                    {
                        Chase(entity.creatures[0]);
                        
                    }
                }

            }

        }

        if (currentlyInAction)
        MovementHandling();
    }

    public void EatFood () {
        currentlyInAction = true;
        currentAction = "";

        if (entity.type == GTYPE.Creature) {

            if (entity.food.Count > 0) {
                
                if (entity.food[0] != null)
                    MoveTo(entity.food[0],entity.traits.speed,"movingToFood",0);
                else {
                    currentAction = "searchingForFood";
                    return;
                }

            }
            else {
                currentAction = "searchingForFood";
                Vector3 pointOnMap = entity.manager.GetRandomPointAwayFrom(transform.position,entity.traits.sightRange);
                Debug.DrawLine(pointOnMap, pointOnMap+Vector3.up*10,Color.red,10);
                MoveTo(pointOnMap,entity.traits.speed,"movingToFood",0f);
                return;
            }

        }
        else if (entity.type == GTYPE.Predator) 
        {
            if (entity.creatures.Count > 0) {
              if (entity.creatures[0] != null)
                    Chase (entity.creatures[0]);
                else {
                    currentAction = "searchingForCreature";
                    return;
                }

            }
            else {
                currentAction = "searchingForCreature";
                Vector3 pointOnMap = entity.manager.GetRandomPointAwayFrom(transform.position,entity.traits.sightRange);
                Debug.DrawLine(pointOnMap, pointOnMap+Vector3.up*10,Color.red,10);
                MoveTo(pointOnMap,entity.traits.speed,"movingToCreature",0f);
                return;
            }
            //Pursuit Request if Existing
            //If not look for food then send request
        }

    }

    private void Chase (EntityManager e) {
        agent.isStopped = true;
        CancelAction();
        currentlyInAction = true;
        currentAction = "creaturePursuit";
        print ("Found creature and about to give chase!");
    }

    //Collisions result in instantaneous stuff
    private void OnCollisionEnter (Collision col) {
        if (col.transform.tag == "Food") {
            if (entity.type == GTYPE.Creature) {
                stateManager.EatState();
                if (entity.food.Contains(col.transform))
                    entity.food.Remove(col.transform);
                GameObject.Destroy(col.transform.gameObject);
            }

            if (currentlyInAction && (currentAction == "searchingForFood" || currentAction == "movingToFood" || currentAction == "foodPursuit")) {
                OnSuccess("findingFood");
                agent.isStopped = true;
            }
        }
        else if (col.transform.tag == "Player") {

            if (entity.type == GTYPE.Predator) {

                if (currentlyInAction && (currentAction == "searchingForCreature" || currentAction == "movingToCreature" || currentAction == "foodPursuit")) {
                    OnSuccess("findingFood");
                }

            }

        }
        else if (col.transform.tag == "Enemy") {

        }
    }

    #region  Core Action Management

    private void CompletedAction (string invocationStatement) {
        if (invocationStatement == "movingToFood") {
            OnFailure("findingFood");
        }
        else if (invocationStatement == "movingToCreature") {
            OnFailure("findingFood");
        }
    }

    public void CancelAction() {
        currentAction = "";
        currentlyInAction = false;
       // agent.isStopped = true;
    }

    private void OnSuccess(string a) {
        entity.SuccessfulAction(a);
        print ("Success of " + a);
        CancelAction();
    }

    private void OnFailure(string a) {
        entity.FailedAction(a);
        print ("Failure of " + a);
        CancelAction();
    }

    public void Request (string request, EntityManager requestSender) {

        if (request == "pursuit") {
            CancelAction();

        }

    }
    

    #endregion

    #region  Movement Handling

    string invocationStatement;

    public void MoveTo (Transform target, float speed, string invocationTarget, float distPerc)
	{
        agent.isStopped = false;
		if (agent.isOnNavMesh && agent.isActiveAndEnabled)
		{
			agent.speed = speed;
			agent.SetDestination(target.position + (transform.position - target.position) * distPerc);
			invocationStatement = invocationTarget;
		}
	}

    public void MoveTo (Vector3 target, float speed, string invocationTarget, float distPerc)
	{
        agent.isStopped = false;
		if (agent.isOnNavMesh && agent.isActiveAndEnabled)
		{
			agent.speed = speed;
			agent.SetDestination(target + (transform.position - target) * distPerc);
			invocationStatement = invocationTarget;
		}
	}

    private void MovementHandling() {

        if (agent.isOnNavMesh)
		if (agent.remainingDistance <= 0.05f)
		{
			CompletedAction(invocationStatement);
		}
		else
		{
			stateManager.state.energy -= EnergyMovementCalculation(agent.speed) * Time.deltaTime * movementCost;
		}
    }

    public virtual float EnergyMovementCalculation (float movementSpeed) {
        return movementSpeed*entity.traits.size+stateManager.state.age*0.05f;
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
