using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity;
using UnityEngine.Events;

public class GraphManager : MonoBehaviour
{

    public Transform UIPrefab;
    public Transform startingPosition;

    public Vector3 startingPoint;
    public float gap;

    public static GraphManager Instance;


    private void Awake() {
        if (Instance == null)
            Instance = this;

        startingPosition.gameObject.SetActive(false);
        startingPoint = startingPosition.position;
    }   

    public void IncrementGeneration() {

        int total = MapManager.Instance.latestGeneration;

        for (int i = 0; i < total+1; i++) {

            UnityAction<int> action = LoadGeneration;
            string displayText = "Generation #"+(total-1).ToString();
            

        }


    }

    public void LoadGeneration (int i) {

    }

}
