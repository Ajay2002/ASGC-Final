using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GeneticUIController : MonoBehaviour
{

    public Canvas dynamicCanvas;

    public Transform selectedEntity;

    private void Update() {
        
        Vector3 position = selectedEntity.position + Vector3.up*6;
        dynamicCanvas.transform.position = position;

        dynamicCanvas.transform.LookAt(Camera.main.transform.position);

    }

}
