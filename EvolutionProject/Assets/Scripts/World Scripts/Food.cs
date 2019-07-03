using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Food : MonoBehaviour
{
    public float value;
    public bool canPreyEat = true;

    public void Eat ()
    {
        Destroy(gameObject);
        MapManager.Instance.amountOfFood--;
    }
}
