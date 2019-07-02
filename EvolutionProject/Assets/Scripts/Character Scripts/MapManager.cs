﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;


//IMPORTANT: YOU WILL NEED A TIME SCALE MANAGER

public class MapManager : MonoBehaviour
{
    public static MapManager Instance;
    
    public GameObject entity;
    public GameObject foodObject;
    public float mutationChance;
    public Vector3 area;

    public GeneticTraits idealTraits;

    private void Start ()
    {
        if (Instance == null) Instance = this;
    }

    private void OnDrawGizmos() {
        
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position, area);
        
    }

    private IEnumerator FoodGen() {
        Instantiate(foodObject,GetRandomPoint(),Quaternion.identity);
        yield return new WaitForSeconds(0.2f);
        StartCoroutine("FoodGen");
    }

    public Vector3 GetRandomPoint () {

        float percX = Random.Range(-1f,1f);
        float percY = Random.Range(-1f,1f);

        float xPoint = transform.position.x+((area.x/2)*percX);
        float zPoint = transform.position.z+((area.z/2)*percY);

        float yPoint = 100;

        Ray ray = new Ray(new Vector3(xPoint,yPoint, zPoint), Vector3.down);
        RaycastHit hit = new RaycastHit();

        if (Physics.Raycast(ray, out hit)) {
            return hit.point;
        }

        return transform.position;

    }

    private void Awake() {StartCoroutine("FoodGen");}

    public Transform SpawnEntity (Vector3 position) {
        GameObject go = (GameObject)GameObject.Instantiate(entity,position,Quaternion.identity);
        return go.transform;
    }

    public Vector3 GetRandomPointAwayFrom (Vector3 p, float r) {
        bool conditionsMet = false;
        float xPoint=0;
        float zPoint=0;
        float yPoint=0;

        while (conditionsMet == false) {
            float percX = Random.Range(-1f,1f);
            float percY = Random.Range(-1f,1f);

            xPoint = transform.position.x+((area.x/2)*percX);
            zPoint = transform.position.z+((area.z/2)*percY);

            yPoint = 100;

            Vector3 suggestedPoint = new Vector3(xPoint,0,zPoint);
            Vector3 unSuggestedPoint = new Vector3(p.x,0,p.z);

            if (Vector3.Distance(suggestedPoint,unSuggestedPoint) >= r) {
                conditionsMet = true;
                break;
            }
            

        }

        Ray ray = new Ray(new Vector3(xPoint,yPoint, zPoint), Vector3.down);
        RaycastHit hit = new RaycastHit();

        if (Physics.Raycast(ray, out hit)) {
            Vector3 hP = hit.point;
            hP.y += 0.5f;
            return hP;
        }

        return transform.position;
    }

    public Vector3 NearestPointOnMap (Vector3 offPoint)
    {
        NavMeshHit hit;
        if (NavMesh.SamplePosition(offPoint, out hit, Mathf.Infinity, NavMesh.AllAreas)) {
            return hit.position;
        }
        
        throw new Exception("NearestPointOnMap failed, nav mesh error");
    }
}
