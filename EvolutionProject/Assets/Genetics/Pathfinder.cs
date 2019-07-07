using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Pathfinder : MonoBehaviour
{

    public List<PathNode> nodes = new List<PathNode>();
    
    public List<PathNode> closedSet = new List<PathNode>();

    public PathNode FindClosestNode (Vector3 v) {
        int c = 0;
        for (int i = 0; i < nodes.Count; i++) {
           if (Vector3.Distance(nodes[i].transform.position,v) < Vector3.Distance(nodes[c].transform.position,v)) {
               c = i;
           } 
        }

        return nodes[c];
    }


    public void FindPath (Vector3 startPoint, Vector3 endPoint, Action<List<PathNode>> returnMethod) {
        closedSet.Clear();
        PathNode a = FindClosestNode(startPoint);
        PathNode b = FindClosestNode(endPoint);

        a.Spread(new List<PathNode>(),this,b,returnMethod);

    }

    
 
    
}
