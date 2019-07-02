using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;


//IMPORTANT: YOU WILL NEED A TIME SCALE MANAGER

public class MapManager : MonoBehaviour
{
    public static MapManager Instance;
    
    public bool enemyGraph = false;
    public string graph;
    public GraphHelp help;
    public GameObject entity;
    public GameObject foodObject;
    public float mutationChance;
    public Vector3 area;

    public GameObject enemyEntity;

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
        yield return new WaitForSeconds(0.5f);
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
        else {
            //GetRandomPoint();
        }

        return transform.position;

    }

    private void Awake()
    {
        StartCoroutine("FoodGen");

        help.AddGraph("SelectedTrait",Color.blue);
        help.AddGraph("Population",Color.red);
    }

    float t = 0;
    private void Update() {
        
        if (Input.GetKeyDown(KeyCode.Space))
            Application.LoadLevel(0);
        t += Time.deltaTime;

        float average = 0f;
        GeneticEntity_T[] T = GameObject.FindObjectsOfType<GeneticEntity_T>();
        int L  = 0;
        for (int i = 0; i < T.Length; i++) {
            if (enemyGraph && T[i].type == GeneticEntity_T.GeneticType.Creature)
                continue;
            if (!enemyGraph && T[i].type == GeneticEntity_T.GeneticType.Predator)
                continue;

            if (graph == "Speed")
                average += T[i].traits.speed*100;
            if (graph == "SightRange")
                average += T[i].traits.sightRange*100;
            if (graph == "Size")
                average += T[i].traits.size*100;
            if (graph == "Strength")
                average += T[i].traits.strength*100;
            if (graph == "DangerSense") 
                average += T[i].traits.dangerSense*100;
            if (graph == "Attractiveness")
                average += T[i].traits.attractiveness*100;
            if (graph == "HI")
                average += T[i].traits.HI*100;
            if (graph == "AI")
                average += T[i].traits.AI*100;
            if (graph == "FI")
                average += T[i].traits.FI*100;
            if (graph == "HUI")
                average += T[i].traits.HUI*100;
            if (graph == "SI")
                average += T[i].traits.SI*100;
            if (graph == "RI")
                average += T[i].traits.RI*100;

            L++;
        }
        average = average/L;
        help.Plot(t,average,0);
        help.Plot(t,L,1);
    }

    public Transform SpawnEntity (Vector3 position) {
        GameObject go = (GameObject)GameObject.Instantiate(entity,position,Quaternion.identity);
        return go.transform;
    }

    public Transform SpawnEntityEnemy (Vector3 position) {
        GameObject go = (GameObject)GameObject.Instantiate(enemyEntity,position,Quaternion.identity);
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

            bool distGr = false;
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
                conditionsMet = true;
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
