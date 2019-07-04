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

    public int hiddenLayer,hiddenNeuron;
    public bool tryNetwork = false;

    public List<FoodSpawnerScriptableObject> foodSpawnerScriptableObjects;

    public List<float> worldSpawnedFoodSpawnPeriods;
    public List<FoodScriptableObject> worldSpawnedFoodScriptableObjects;

    public List<Tuple<float, FoodScriptableObject>> worldSpawnedFood;

    public int maxAmountOfFood;
    public int amountOfFood;

    public bool enemyGraph = false;
    public string graph;
    public GraphHelp help;
    public GameObject entity;
    public float mutationChance;
    public Vector3 area;

    public GameObject enemyEntity;

    public GeneticTraits idealTraits;

    private void OnDrawGizmos() {
        
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position, area);
        
    }
    
    private IEnumerator FoodGen(int i)
    {
        if (amountOfFood < maxAmountOfFood)
        {
            Food f = Instantiate(worldSpawnedFood[i].Item2.prefab, NearestPointOnMap(GetRandomPoint()), Quaternion.identity).GetComponent<Food>();
            f.value      = worldSpawnedFood[i].Item2.value;
            f.canPreyEat = worldSpawnedFood[i].Item2.canPreyEat;

            amountOfFood++;
        }

        yield return new WaitForSeconds(worldSpawnedFood[i].Item1);
        StartCoroutine(nameof(FoodGen), i);
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

    public float GetAverageFitness (GTYPE type) {
        
        EntityManager[] m = GameObject.FindObjectsOfType<EntityManager>();

        int L = 0;
        float avg = 0;
        for (int i = 0; i < m.Length; i++) {
            if (m[i].type == type) {
                L++;
                avg += m[i].stateManagement.fitness;
            }
        }

        return avg/L;

    }

    private void Awake()
    {
        if (Instance == null) Instance = this;
        
        if (foodSpawnerScriptableObjects == null) foodSpawnerScriptableObjects = new List<FoodSpawnerScriptableObject>();
        if (worldSpawnedFood == null) worldSpawnedFood = new List<Tuple<float, FoodScriptableObject>>();
        if (worldSpawnedFoodSpawnPeriods == null) worldSpawnedFoodSpawnPeriods = new List<float>();
        if (worldSpawnedFoodScriptableObjects == null) worldSpawnedFoodScriptableObjects = new List<FoodScriptableObject>();
        
        
        for (int i = 0; i < 10; i++)
        {
            FoodSpawner fs = Instantiate(foodSpawnerScriptableObjects[0].prefab, GetRandomPoint(), Quaternion.identity).GetComponent<FoodSpawner>();
            fs.Initialise(foodSpawnerScriptableObjects[0]);
        }

        help.AddGraph("SelectedTrait", Color.blue);
        help.AddGraph("Population",    Color.red);
        
        
        if (worldSpawnedFoodSpawnPeriods.Count != worldSpawnedFoodScriptableObjects.Count) throw new Exception("World Spawned Food Spawn Periods is not the " +
                                                                                                                        "same length as World Spawned Food Scriptable Objects.");

        for (int i = 0; i < worldSpawnedFoodSpawnPeriods.Count; i++)
            worldSpawnedFood.Add(new Tuple<float, FoodScriptableObject>(worldSpawnedFoodSpawnPeriods[i], worldSpawnedFoodScriptableObjects[i]));

        for (int i = 0; i < worldSpawnedFood.Count; i++) StartCoroutine(nameof(FoodGen), i);
    }

    float t = 0;
    private void Update() {
        
        if (Input.GetKeyDown(KeyCode.Space))
            Application.LoadLevel(0);
        t += Time.deltaTime;

        float average = 0f;
        EntityManager[] T = GameObject.FindObjectsOfType<EntityManager>();
        int L  = 0;
        for (int i = 0; i < T.Length; i++) {
            if (enemyGraph && T[i].type == GTYPE.Creature)
                continue;
            if (!enemyGraph && T[i].type == GTYPE.Predator)
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
