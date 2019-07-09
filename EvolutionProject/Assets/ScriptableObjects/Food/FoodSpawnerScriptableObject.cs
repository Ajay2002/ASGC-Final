using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Food Spawner", menuName = "Food/Food Spawner")]
public class FoodSpawnerScriptableObject : ScriptableObject
{
    public String type;
    public FoodScriptableObject food;
    public GameObject prefab;
    public float spawnPeriod;
    public float spawnDistance;

    public int cost;
}
