using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Experimental.AI;
using UnityEngine.Jobs;
using Unity.Jobs;

public class NavMeshManager : MonoBehaviour
{

    [Header("Pathfinding Performance Options")]
    public int maxBatchCount;
    public float batchUpdate;
    public bool pauseForNavMeshUpdate;

    public int queryCounter {
        get {
            return queryLists.Count-1;
        }
    }

    public List< List<NavMeshQuery> > queryLists = new List < List <NavMeshQuery> >();
    public List<NavMeshQuery> currentQList = new List<NavMeshQuery>();
    
    public List<int> tempIdInBatch = new List<int>();

    float t = 0f;
    public enum CalculationState {Nothing, Waiting, Finished, Calculating};
    public CalculationState state;

    public static NavMeshManager Instance;

    private void Awake() {
        if (Instance == null)
            Instance = this;
            
        t = batchUpdate;
        queryLists.Add(new List<NavMeshQuery>());
    }

    private void Update() {

        t -= Time.deltaTime;

        if ( t <= 0) {

            if (state == CalculationState.Finished || state == CalculationState.Nothing) {

                if (queryLists.Count > 0 && queryLists[0].Count > 0) {

                    currentQList = queryLists[0];
                    BeginCalculating();
                    state = CalculationState.Calculating;
                    t = batchUpdate;
                }

            }

            if (state == CalculationState.Calculating) {
                //Check if all has been completed (bool return)
                if (CompletionCheck() == true) {
                    state = CalculationState.Finished;
                    queryLists.RemoveAt(0);
                    currentQList.Clear();
                }
                t = batchUpdate;
            }

        }
        else {
            //Processing
        }

    }   

    private void BeginCalculating() {

        for (int i = 0; i < currentQList.Count; i++) {
            if (currentQList[i].agent == null || !currentQList[i].agent.isOnNavMesh) { 
                currentQList.RemoveAt(i);
                continue;
            }
            currentQList[i].agent.CalculatePath(currentQList[i].target,currentQList[i].path);

        }

    }

    private bool CompletionCheck() {

        bool foundAllPath = true;
        for (int i = 0; i < currentQList.Count; i++) {
            if (currentQList[i].agent != null)
            if (currentQList[i].agent.pathPending == true) {
                
                foundAllPath = false;
                break;

            }

        }

        if (foundAllPath == true) {

            for (int i = 0; i < currentQList.Count; i++) {
                if (currentQList[i].agent != null)
                currentQList[i].queryReturn.Invoke(currentQList[i].path);

            }

        }

        return foundAllPath;

    }

    //Remove from query list
    private void CompletedQueryList () {


    }

    public void RequestPath (NavMeshQuery query) {
        if (queryLists.Count == 0)
            queryLists.Add(new List<NavMeshQuery>());
        if (tempIdInBatch.Contains(query.t.GetInstanceID())) {
            Debug.LogError("Don't send more than one path request per frame, inneficient. I'm dissapointed in you Ajay :(");
            return;
        }
        
        if (queryLists[queryCounter].Count <= maxBatchCount) 
            queryLists[queryCounter].Add(query);
        
        if (queryLists[queryCounter].Count > maxBatchCount)
        {
            queryLists.Add(new List<NavMeshQuery>());
            tempIdInBatch.Clear();
        }

    }

    //Calculate Job (Need a movement & checking job as well)
    struct NavMeshCalculateJob : IJobParallelForTransform {

        [ReadOnly]
        public NativeArray<Vector3> nextVertext;
        [ReadOnly]
        public NativeArray<float> speed;

        public float deltaTime;

        public void Execute (int i, TransformAccess transform) {
        
           

        }

        

    }

}

//Should probably be a struct, oh well 

[System.Serializable]
public class NavMeshQuery {

    public Transform t;
    public Action<NavMeshPath> queryReturn;
    public NavMeshAgent agent;
    public NavMeshPath path = new NavMeshPath();
    public Vector3 target;
    public float speed;

    public NavMeshQuery (Transform transform, Action<NavMeshPath> returnAction, NavMeshAgent agent, Vector3 target, float speed) {
        t = transform;
        this.queryReturn = returnAction;
        this.agent = agent;
        this.path = new NavMeshPath();
        this.target = target;
        this.speed = speed;
    }

}