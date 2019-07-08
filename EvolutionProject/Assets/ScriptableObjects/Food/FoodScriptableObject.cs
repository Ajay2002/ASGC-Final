using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Food", menuName = "Food/Food")]
public class FoodScriptableObject : ScriptableObject
{
	public string type;
	public GameObject prefab;
	public float value;
	public bool canPreyEat;
}
