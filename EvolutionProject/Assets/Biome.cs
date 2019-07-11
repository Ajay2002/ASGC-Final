using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;
public class Biome : MonoBehaviour
{
    public float scale;
    public float upScale;

    public BiomeType type=BiomeType.Grass;
    public List<Button> buttons = new List<Button>();
    public LayerMask groundMask;
    public LayerMask waterMask;
    public Vector3 area;

    
    private void OnDrawGizmos() {
        Gizmos.DrawLine(transform.position,transform.position+transform.up*scale);
         Gizmos.DrawLine(transform.position,transform.position+transform.right*scale);
          Gizmos.DrawLine(transform.position,transform.position-transform.right*scale);
           Gizmos.DrawLine(transform.position,transform.position-transform.up*scale);
           Gizmos.color = Color.green;
            Gizmos.DrawWireCube(transform.position,area);
    }

    public void UpSender (Button obj) {

        Vector3 pos = transform.position-transform.up*upScale;
        MapManager.Instance.CreateNewBiome(pos,obj,Vector3.forward*upScale);

    }

    public void RightSender (Button obj) {
        Vector3 pos = transform.position+transform.right*scale;
        MapManager.Instance.CreateNewBiome(pos,obj,Vector3.right*scale);
    }

    public void LeftSender (Button obj) {
        Vector3 pos = transform.position-transform.right*scale;
        MapManager.Instance.CreateNewBiome(pos,obj,Vector3.right*scale);
    }

    public void DownSender (Button obj) {
        Vector3 pos = transform.position+transform.up*upScale;
        MapManager.Instance.CreateNewBiome(pos,obj,Vector3.forward*upScale);
    }

    public Vector3 getRandomPointGround (Vector3 def){

        float percX = Random.Range(-1f,1f);
        float percY = Random.Range(-1f,1f);

        float xPoint = transform.position.x+((area.x/2)*percX);
        float zPoint = transform.position.z+((area.z/2)*percY);

        float yPoint = 100;

        Ray ray = new Ray(new Vector3(xPoint,yPoint, zPoint), Vector3.down);
        RaycastHit hit = new RaycastHit();

        if (Physics.Raycast(ray, out hit, groundMask)) {
            //Debug.DrawRay(hit.point,Vector3.up,Color.red,10);
            return MapManager.Instance.NearestPointOnMap(hit.point);
        }
        else {
            //GetRandomPoint();
        }
        
        return def;
    }

    List<Vector3> tried=new List<Vector3>();
    public (Vector3,bool) getRandomWaterPoint (int tries) {
        tried.Clear();
        for (int i = 0; i < tries; i++) {
            float percX = Random.Range(-1f,1f);
            float percY = Random.Range(-1f,1f);

            float xPoint = transform.position.x+((area.x/2)*percX);
            float zPoint = transform.position.z+((area.z/2)*percY);

            float yPoint = 100;
            if (tried.Contains(new Vector3(xPoint,yPoint, zPoint)))
                continue;
            tried.Add(new Vector3(xPoint,yPoint, zPoint));
            Ray ray = new Ray(new Vector3(xPoint,yPoint, zPoint), Vector3.down);
            RaycastHit hit = new RaycastHit();

            if (Physics.Raycast(ray, out hit,1000f, groundMask)) {
                // if (hit.transform.tag.CompareTo("Water")==1) {
                //     print ("Found water");
                //     //Debug.DrawRay(hit.point,Vector3.up,Color.red,10);
                //     Debug.DrawLine(hit.point,hit.point+Vector3.up*100,Color.red,20);

                //     return (MapManager.Instance.NearestPointOnMap(hit.point),true);
                // }
                // (MapManager.Instance.GetRandomPoint(),false);
            }
            else {
                return (MapManager.Instance.NearestPointOnMap(new Vector3(xPoint,0, zPoint)),true);
            }
        }
        
        return (Vector3.zero,false);
//        return (NavMesh.SamplePosition(MapManager.Instance.GetRandomPoint(),,false);
    }

}

public enum BiomeType {
    Grass,
    Desert,
    Snow,
    Forest
}
