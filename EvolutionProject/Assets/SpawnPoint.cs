using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
    public Vector3 area;
    
    private void OnDrawGizmos() {

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(transform.position, area);

    }

    public Vector3 GetRandomPoint() {

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

    public void Rotate (int times) {

        if (times%2 == 0) {
            return;
        }
        else {
            float x = area.z;

            area.z = area.x;
            area.x = x;
            
        }

    }

}
