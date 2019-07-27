using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class UIManager : MonoBehaviour
{
    
    public List<Transform> panels = new List<Transform>();

    public List<Transform> topToggle1 = new List<Transform>();
    public List<Transform> biomeToggle1 = new List<Transform>();
    public List<Transform> biomeToggle2 = new List<Transform>();

    int currentToggle = 0;
    public void TopToggle (int v) {
        
       

    

    }

    public void BiomeToggle1 (int v) {

        for (int i = 0; i < biomeToggle1.Count; i++) {

            if (i==v)
                {
                    biomeToggle1[i].GetComponent<Toggle>().isOn = true;
                    continue;
                }
            
           biomeToggle1[i].GetComponent<Toggle>().isOn = false;

        }

    }

    public void BiomeToggle2 (int v) {

        for (int i = 0; i < biomeToggle2.Count; i++) {

            if (i==v)
                continue;
            
           biomeToggle2[i].GetComponent<Toggle>().isOn = false;

        }

    }

    public void Show (int v) {

        for (int i = 0; i < panels.Count; i++) {

            if ( v == i)
                panels[i].gameObject.SetActive(true);
            else
                panels[i].gameObject.SetActive(false);

        }

    }


}
