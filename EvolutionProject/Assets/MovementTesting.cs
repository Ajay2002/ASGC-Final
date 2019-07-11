using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System;

public class MovementTesting : MonoBehaviour
{

    public MapManager manager;


    private void Start() {

    }

    private void Update () {

        if (Input.GetKeyDown(KeyCode.V)) {

            StopAndReset();
        //    NavMeshManager.Instance.RequestPath(new NavMeshQuery(transform,GotPath,transform.GetComponent<NavMeshAgent>(), MapManager.Instance.GetRandomPoint(),UnityEngine.Random.Range(0.2f,4)));
        }

    }

    
    public void GotPath (NavMeshPath queryReturn) {

        transform.GetComponent<NavMeshAgent>().ResetPath();
        transform.GetComponent<NavMeshAgent>().isStopped = false;

        transform.GetComponent<NavMeshAgent>().speed=UnityEngine.Random.Range(0.2f,4);
        if (queryReturn.status == NavMeshPathStatus.PathInvalid)
        {Debug.LogError("Some Error with the actual path..."); return;}    
        
        for (int i = 0; i < queryReturn.corners.Length; i++) {
            if (i == 0)
            continue;

            //Do a job transform here
            Debug.DrawLine(queryReturn.corners[i],queryReturn.corners[i-1], Color.red, 2);

        }
        
        transform.GetComponent<NavMeshAgent>().SetPath(queryReturn);

    }

    public void StopAndReset () {
        transform.GetComponent<NavMeshAgent>().isStopped = true;
        NavMeshManager.Instance.RequestPath(new NavMeshQuery(transform,GotPath,transform.GetComponent<NavMeshAgent>(), MapManager.Instance.GetRandomPoint(),UnityEngine.Random.Range(0.2f,4)));
    }

}
