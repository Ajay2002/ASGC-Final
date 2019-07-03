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
        if (MapManager.Instance.amountOfFood < MapManager.Instance.maxAmountOfFood)
        {
            Vector2 randOffset = Random.insideUnitCircle * foodSpawnerScriptable.spawnDistance;
            Vector3 position   = transform.position;
            position = MapManager.Instance.NearestPointOnMap(new Vector3(position.x + randOffset.x, position.y, position.z + randOffset.y));

            Food f = Instantiate(foodSpawnerScriptable.food.prefab, position, Quaternion.identity).GetComponent<Food>();
            f.value = foodSpawnerScriptable.food.value;
            f.canPreyEat  = foodSpawnerScriptable.food.canPreyEat;

            MapManager.Instance.amountOfFood++;
        }

        yield return new WaitForSeconds(foodSpawnerScriptable.spawnPeriod);
        StartCoroutine(nameof(FoodGen));
    }
}
