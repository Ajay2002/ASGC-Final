using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PathNode : MonoBehaviour
{

    public bool drawGizmos = false;
    public List<PathNode> neighbours = new List<PathNode>();


    private void OnDrawGizmos() {
        if (drawGizmos)
        foreach (PathNode n in neighbours) {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position,n.transform.position);
        }
    }

    public void Spread (List<PathNode> path,Pathfinder finder, PathNode needed, Action<List<PathNode>> returnMethod) {
        path.Add(this);
        finder.closedSet.Add(this);
        
        if (needed == this) {
            returnMethod.Invoke(path);
            return;
        }

        for (int i = 0; i < neighbours.Count; i++) {
            if (finder.closedSet.Contains(neighbours[i])) {
                continue;
            }

            neighbours[i].Spread(path,finder,needed,returnMethod);

        }

        return;
    }

}
