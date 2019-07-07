using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CombineManager : MonoBehaviour
{
    private bool selecting;
    private bool a;
    [Header("UI Elements")]
    public Slider percParentA;
    public Slider percParentB;

    [Header("External References")]
    public Vector3 displacement;
    public Vector3 lookDisplacement;
    public float FOV;
    public Camera aCamera;
    public Camera bCamera;
    

    [Header("Layer Masks")]
    public LayerMask aLayerMask;
    public LayerMask bLayerMask;

    [Header("Public Testing References")]
    public EntityManager aEntity;
    public EntityManager bEntity;



    private void Update() {

        if (aEntity != null) {
            
            aEntity.transform.gameObject.layer =11;
            aEntity.transform.GetChild(0).gameObject.layer = 11;
            
        }

        if (bEntity != null) {
            bEntity.transform.gameObject.layer =12;
            bEntity.transform.GetChild(0).gameObject.layer = 12;
        }



        if (selecting == true) {

            if (Input.GetMouseButtonDown(0)) {

                Ray r = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                if (Physics.Raycast(r, out hit)) {
                    if (hit.transform.tag == "Player") {
                        if (hit.transform.GetComponent<EntityManager>() != null) 
                        {
                            if (a) {
                                if (bEntity != null && hit.transform.GetComponent<EntityManager>()==bEntity)
                                    bEntity = null;

                                aEntity = hit.transform.GetComponent<EntityManager>();
                            }
                            else {
                                if (aEntity != null && hit.transform.GetComponent<EntityManager>()==aEntity)
                                    aEntity = null;

                                bEntity = hit.transform.GetComponent<EntityManager>();

                            }
                        }
                    }
                }

                selecting = false;
                b.interactable = true;

            }


            

        }

        CameraFollow();

    }

    private void CameraFollow() {

        if (aEntity != null) {
            aCamera.fieldOfView = FOV;
            aCamera.transform.position = aEntity.position+displacement;
            aCamera.transform.LookAt(aEntity.transform.position+lookDisplacement);
            aCamera.backgroundColor = Color.clear;
        }
        else {
            aCamera.backgroundColor = Color.white;
        }

        if (bEntity != null) {
            bCamera.fieldOfView = FOV;
            bCamera.transform.position = bEntity.position+displacement;
            bCamera.transform.LookAt(bEntity.transform.position+lookDisplacement);
            bCamera.backgroundColor = Color.clear;
        }
        else {
            bCamera.backgroundColor = Color.white;
        }

    }


    public void SelectA(Button sender) {
        //Remove current and remove it's maask
        selecting = true;
        sender.interactable = false;
        b = sender;
        a = true;
        if (aEntity != null) {
            
            aEntity.transform.gameObject.layer = 0;
            aEntity.transform.GetChild(0).gameObject.layer = 0;
            aEntity = null;

        }


    }

    Button b;

    public void SelectB(Button sender) {
        sender.interactable = false;
        b = sender;
        selecting = true;
        a = false;

        if (bEntity != null) {
            
            bEntity.transform.gameObject.layer = 0;
            bEntity.transform.GetChild(0).gameObject.layer = 0;
            bEntity = null;

        }
    }

    public void Combine() {
        
        if (aEntity != null && bEntity != null) {
        
            if (CurrencyController.Instance.RemoveCurrency(200,true)==true) {

                aEntity.controller.ForceBreed(bEntity);
                if (bEntity != null) {
            
                    bEntity.transform.gameObject.layer = 0;
                    bEntity.transform.GetChild(0).gameObject.layer = 0;
                    bEntity = null;

                }

                if (aEntity != null) {
            
                    aEntity.transform.gameObject.layer = 0;
                    aEntity.transform.GetChild(0).gameObject.layer = 0;
                    aEntity = null;

                }
                aEntity = null;
                bEntity = null;

            }

        }

    }
    
}
