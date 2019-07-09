using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using Random = UnityEngine.Random;


//IMPORTANT: YOU WILL NEED A TIME SCALE MANAGER

public class MapManager : MonoBehaviour
{
    [Header("Biome Management")]
    public LayerMask groundMask;
    public Vector3 bounds;
    public NavMeshSurface surface;
    public Transform biomeInstance;
    public float buttonDistanceThreshold;
    public List<Mesh> biomes = new List<Mesh>();
    public List<Material> biomeMaterials = new List<Material>();
    public List<Material> biomeFurMaterials = new List<Material>();
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
            //Debug.DrawRay(hit.point,Vector3.up,Color.red,10);
            return NearestPointOnMap(hit.point);
        }
        else {
            //GetRandomPoint();
        }

        return transform.position;

    }

    public bool GetBiomeTypeFromPosition (Vector3 pos, out BiomeType typeReturn) {
        Ray ray = new Ray(pos+Vector3.up*2, Vector3.down);
        RaycastHit hit = new RaycastHit();

        if (Physics.Raycast(ray, out hit,1000, groundMask)) {
            //Debug.DrawRay(hit.point,Vector3.up,Color.red,10);
            if (hit.transform.GetComponent<Biome>() != null) {
                typeReturn = hit.transform.GetComponent<Biome>().type;
                return true;
            }
        }
        else {
            typeReturn = BiomeType.Grass;
            return false;
            //GetRandomPoint();
        }
        
        typeReturn = BiomeType.Grass;
        return false;
    }

    int bPress = 1;
    public void CreateNewBiome (Vector3 position, Button sender, Vector3 ext) {

        
        
        
        BiomeType t = BiomeType.Grass;
        Material mat=biomeMaterials[0];
        int s = Random.Range(0,4);
        if (s == 0) {mat = biomeMaterials[s];t=BiomeType.Grass;};
        if (s == 1) {mat = biomeMaterials[s];t=BiomeType.Snow;};
        if (s == 2) {mat = biomeMaterials[s];t=BiomeType.Desert;};
        if (s == 3) {mat = biomeMaterials[s];t=BiomeType.Forest;};

        int biomeSelection = Random.Range(0,biomes.Count);

        
        if (CurrencyController.Instance.RemoveCurrency(10*bPress,true)) {
            NotificationManager.Instance.CreateNotification(NotificationType.Message,"Successfully created a new biome of type " + t.ToString() + "!",false,1);
            bPress++;
            GameObject newBiome = (GameObject)Instantiate(biomeInstance.gameObject,position,biomeInstance.transform.rotation);
            int r = Random.Range(0,5);
            newBiome.transform.eulerAngles += new Vector3(0,0,90*r);
            Biome b = newBiome.transform.GetComponent<Biome>();
            newBiome.transform.GetComponent<MeshRenderer>().material = mat;
            newBiome.transform.GetComponent<MeshCollider>().sharedMesh=biomes[biomeSelection];
            newBiome.transform.GetComponent<MeshFilter>().mesh = biomes[biomeSelection];
            b.type = t;

            Button closest=b.buttons[0];
            for (int i = 0; i < b.buttons.Count; i++) {
                if (Vector3.Distance(b.buttons[i].transform.position,sender.transform.position)<Vector3.Distance(closest.transform.position,sender.transform.position)) {
                    closest = b.buttons[i];
                }
            }

            GameObject.Destroy(closest.gameObject);
            GameObject.Destroy(sender.gameObject);

            surface.BuildNavMesh();
            area += ext+bounds;


            Biome[] biomesr = GameObject.FindObjectsOfType<Biome>();
            Vector3 avg = Vector3.zero;
            for (int i = 0; i < biomesr.Length; i++) {
                avg += biomesr[i].transform.position;
                for (int x = 0; x < biomesr.Length; x++) {
                    if (i<0||x<0)
                    continue;
                    if(i>biomesr.Length-1||x>biomesr.Length)
                    continue;
                    if (biomesr[i]==biomes[x] || i==x)
                    continue;

                    for (int c = 0; c < biomesr[i].buttons.Count; c++) {

                        for (int d = 0; d < biomesr[x].buttons.Count; d++) {
                            
                            if (biomesr[i].buttons[c]==null || biomesr[x].buttons[d]==null)
                            continue;

                            if (Vector3.Distance(biomesr[i].buttons[c].transform.position,biomesr[x].buttons[d].transform.position)<=buttonDistanceThreshold) {

                                GameObject.Destroy(biomesr[i].buttons[c].transform.gameObject);
                                GameObject.Destroy(biomesr[x].buttons[d].transform.gameObject);

                            }


                        }

                    }



                }
            }

            avg/=biomesr.Length;
            avg.y = transform.position.y;
            transform.position = avg;

        }
        else {
            NotificationManager.Instance.CreateNotification(NotificationType.Cost,"Insufficient funds to create a new biome! ",false,0.2f);

        }
        //Randomly Select Biome Type
        //Charge
        //Disable opposite button on the mesh
        //Create the biome
        //Update the mesh renderer & mesh collider
        //Assign the materials
        //Change the tag of the biome
        //Update the NavMesh
        //Assign Biome Type to the Biome
        //Update bounds of this map manager to encompass. 
        //Kill the Button that sent it.

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
        GameObject go = Instantiate(entity,position,Quaternion.identity);
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
                return NearestPointOnMap(hP);
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

    public Vector3 NearestPointInMapArea (Vector3 offPoint)
    {
        return new Vector3(
                           Mathf.Clamp(offPoint.x, transform.position.x - area.x / 2, transform.position.x + area.x / 2),
                           Mathf.Clamp(offPoint.y, transform.position.y - area.y / 2, transform.position.y + area.y / 2),
                           Mathf.Clamp(offPoint.z, transform.position.z - area.z / 2, transform.position.z + area.z / 2)
                           );
    }
}
