using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System;

public class GeneticController : MonoBehaviour
{
    NavMeshAgent agent;
    GeneticEntity entity;
    
    #region  Default Methods
    private void Awake() {
        agent = GetComponent<NavMeshAgent>();
        entity = GetComponent<GeneticEntity>();
    }

    private void Update() {
        //Debug.DrawLine(transform.position,agent.destination,Color.red);
        if (agent.isOnNavMesh) {
            if (agent.remainingDistance <= 0.1f) {
                entity.CompletedAction(invocationStatement);
            }
            else {
                entity.state.energy -= entity.EnergyMovementCalculation(agent.speed)*Time.deltaTime*0.2f;
            }
        }
    }
    #endregion
    
    //TODO: Soon this will use A* however for now we will use the Navigation.AI
    public void MoveTo (Transform target, float speed, string invocationTarget, float distPerc) {
        if (agent.isOnNavMesh && agent.isActiveAndEnabled) {

            agent.speed = speed;
            agent.SetDestination(target.position+(transform.position-target.position)*distPerc);
            invocationStatement = invocationTarget;
        }
    }

    public void MoveTo (Vector3 target, float speed, string invocationTarget, float distPerc) {
        if (agent.isOnNavMesh && agent.isActiveAndEnabled) {

            agent.speed = speed;
            agent.SetDestination(target+(transform.position-target)*distPerc);

            invocationStatement = invocationTarget;

        }
    }
    
    bool initialSet = false;
    //TODO: Move this whole thing out of an Enumerator and push it into the ECS or Job System <-- This can't even sustain 90 Elements
    public void FOVChecker (float rate, float sensoryDistance) {
        
        entity.enemies.Clear();
        entity.friends.Clear();
        entity.food.Clear();

        Collider[] distanceCheck = Physics.OverlapSphere(transform.position,sensoryDistance);

        List<GeneticEntity> enemies = new List<GeneticEntity>();
        List<GeneticEntity> player = new List<GeneticEntity>();
        List<Transform> food = new List<Transform>();

        for (int i = 0; i < distanceCheck.Length; i++) {
            if (distanceCheck[i].transform == transform || distanceCheck[i].transform.root == transform)
            continue;

            //TODO: Remember to add and make sure it is in the FOV
            string tag = distanceCheck[i].gameObject.tag;

            if (tag.Equals("Enemy")) {
                enemies.Add(distanceCheck[i].transform.root.GetComponent<GeneticEntity>());
            }
            else if (tag.Equals("Player")) {
                player.Add(distanceCheck[i].transform.root.GetComponent<GeneticEntity>());
            }
            else if (tag.Equals("Food")) {
                food.Add(distanceCheck[i].transform.root);
            }

        }

        entity.SensoryUpdate(enemies,food,player);

    }
    
    #region  Extranous Vars

    //NOTE: These are all the elements in processing movement

    bool restarted = false;
    string invocationStatement;
    float timeOut = 0f;
    float rateCopy;
    float sensoryDistanceCopy;

    #endregion

}
