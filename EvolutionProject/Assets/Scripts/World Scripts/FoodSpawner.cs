using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodSpawner : MonoBehaviour
{
    private FoodSpawnerScriptableObject foodSpawnerScriptable;
    
    public void Initialise (FoodSpawnerScriptableObject foodSpawnerScriptable)
    {
        this.foodSpawnerScriptable = foodSpawnerScriptable;
        transform.position = new Vector3(transform.position.x, 0, transform.position.z);
    }
    
    void Start()
    {
        StartCoroutine(nameof(FoodGen));
    }

    private IEnumerator FoodGen()
    {
        Vector2 randOffset = Random.insideUnitCircle * foodSpawnerScriptable.spawnDistance;
        Vector3 position = transform.position;
        position = new Vector3(position.x + randOffset.x, position.y, position.z + randOffset.y);
        
        Food f = Instantiate(foodSpawnerScriptable.food.prefab, position, Quaternion.identity).GetComponent<Food>();
        f.energyValue = foodSpawnerScriptable.food.value;
        f.healthValue = foodSpawnerScriptable.food.value;
        f.hungerValue = foodSpawnerScriptable.food.value;
        f.canPreyEat = foodSpawnerScriptable.food.canPreyEat;
        
        yield return new WaitForSeconds(foodSpawnerScriptable.spawnPeriod);
        StartCoroutine(nameof(FoodGen));
    }
}
