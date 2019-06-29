using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Food : MonoBehaviour
{
    public float energyValue;
    public float hungerValue;
    public float healthValue;
    public bool canPreyEat = true;

    public void Eat ()
    {
        Destroy(gameObject);
    }
}
