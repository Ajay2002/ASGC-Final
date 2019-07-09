using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Biome : MonoBehaviour
{
    public float scale;
    public float upScale;

    public BiomeType type=BiomeType.Grass;
    public List<Button> buttons = new List<Button>();
    
    private void OnDrawGizmos() {
        Gizmos.DrawLine(transform.position,transform.position+transform.up*scale);
         Gizmos.DrawLine(transform.position,transform.position+transform.right*scale);
          Gizmos.DrawLine(transform.position,transform.position-transform.right*scale);
           Gizmos.DrawLine(transform.position,transform.position-transform.up*scale);
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

}

public enum BiomeType {
    Grass,
    Desert,
    Snow,
    Forest
}
