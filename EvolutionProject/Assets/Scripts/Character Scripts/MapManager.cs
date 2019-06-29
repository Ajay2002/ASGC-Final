using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//IMPORTANT: YOU WILL NEED A TIME SCALE MANAGER

public class MapManager : MonoBehaviour
{

    public Vector3 area;

    private void OnDrawGizmos() {
        
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position, area);
        
    }

    public Vector3 GetRandomPoint () {

        float percX = Random.Range(-1f,1f);
        float percY = Random.Range(-1f,1f);

        float xPoint = transform.position.x+((area.x/2)*percX);
        float zPoint = transform.position.z+((area.z/2)*percY);

        float yPoint = 100;

        Ray ray = new Ray(new Vector3(xPoint,yPoint, zPoint), Vector3.down);
        RaycastHit hit = new RaycastHit();

        if (Physics.Raycast(ray, out hit)) {
            return hit.point;
        }

        return transform.position;

    }

    public Vector3 GetRandomPointAwayFrom (Vector3 p, float r) {
        bool conditionsMet = false;
        float xPoint=0;
        float zPoint=0;
        float yPoint=0;

        while (conditionsMet == false) {
            float percX = Random.Range(-1f,1f);
            float percY = Random.Range(-1f,1f);

            xPoint = transform.position.x+((area.x/2)*percX);
            zPoint = transform.position.z+((area.z/2)*percY);

            yPoint = 100;

            Vector3 suggestedPoint = new Vector3(xPoint,0,zPoint);
            Vector3 unSuggestedPoint = new Vector3(p.x,0,p.z);

            if (Vector3.Distance(suggestedPoint,unSuggestedPoint) >= r) {
                conditionsMet = true;
                break;
            }
            

        }

        Ray ray = new Ray(new Vector3(xPoint,yPoint, zPoint), Vector3.down);
        RaycastHit hit = new RaycastHit();

        if (Physics.Raycast(ray, out hit)) {
            Vector3 hP = hit.point;
            hP.y += 0.5f;
            return hP;
        }

        return transform.position;
    }

    
}
