using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;
using System;

public class GraphManager : MonoBehaviour
{

    public RectTransform graphHolder;
    public static GraphManager Instance;
    public List<UILineRenderer> lines = new List<UILineRenderer>();
    public float testScale;

    private Vector3 startingPosition {
        get {
            return new Vector3(graphHolder.rect.position.x, graphHolder.rect.position.y, graphHolder.position.z);
        }
    }
    private Vector3 endingPosition {

        get {
            return new Vector3(graphHolder.rect.position.x+graphHolder.rect.width, graphHolder.rect.position.y+graphHolder.rect.height, graphHolder.position.z);
        }

    }


    [Header("Graph Private Variables")]
    public GraphManager otherGraph;
    public int maxPointsPerGraph;
    public float maxYValue;
    public float minYValue;
    
    public float maxXValue;
    public float minXValue;
    
    
    private void Awake() {
        if (Instance == null)
            Instance = this;
        
        UnityAction<UILineRenderer> a = ResetLine;
        new LoopThrough<UILineRenderer, float>(lines,a).PassWithAction();

    }   

    private float t;
    private void LateUpdate() {
       if (currentPoint != null) {
           Plot(currentPoint.xCoord,currentPoint.yCoord,0);
           currentPoint = null;
       }
    }

    public void ResetLine (UILineRenderer r) {
        
        // r.Points = new Vector2[maxPointsPerGraph];
        // r.Points[0] = startingPosition;

    }
    private Point currentPoint = null;
    public void SendPlotRequest (Point p) {
        if (currentPoint == null)
            currentPoint = p;
    }

    [Header("Points")]
    public List<Point> points = new List<Point>();

    private void Plot (float x, float y, int graph) {
        
        if (points.Count > maxPointsPerGraph) {
            points.RemoveAt(0);
        }
        
        int c=lines[graph].Points.Length+1;

        points.Add(new Point (x,y,graph));

        int viable=0;
        for (int i = 0;i < points.Count; i++) {
            if (points[i]==null)
                break;
            viable++;
        }
        //(maxXValue,minXValue, maxYValue, minYValue, viable)=GraphHealer.Instance.CalculatingMinMax(this, otherGraph);
        /*
        int viable = 0;

        maxXValue = points[0].xCoord;
        maxYValue = points[0].yCoord;
        minYValue = points[0].yCoord;
        minXValue = points[0].xCoord;
        for (int i = 0; i < points.Count; i++) {
            if (points[i] == null)
            continue;

            float xP =points[i].xCoord;
            float yP =points[i].yCoord;

            if (xP >= maxXValue)
            {if (xP==0) maxXValue=0.0001f; else maxXValue = xP;}

            if (xP <= minXValue) {
                if (xP==0) minXValue=0.0001f; else minXValue = xP; }

            if (yP >= maxYValue) {
                if (yP==0) maxYValue=0.0001f; else maxYValue = yP;}

            if (yP <= minYValue) {
                if (yP==0) minYValue=0.0001f; else minYValue = yP;}

            viable ++;
        }*/

        lines[graph].Points = new Vector2[viable];
        
        maxXValue = points[points.Count-1].xCoord;
        minXValue = points[0].xCoord;
        GraphHealer.Instance.SetY(points[points.Count-1]);
        minYValue = GraphHealer.Instance.minY;
        maxYValue = GraphHealer.Instance.maxY;

        int max = 0;
        for (int i = points.Count-1; i > -1; i--) {
            
            if (points[i] == null)
                break;

            // float percX = Mathf.Clamp(points[i].xCoord/((maxXValue-minXValue)),0f,1f);
            // float percY =Mathf.Clamp((points[i].yCoord)/(maxYValue-minYValue),0f,1f);

            float percX = (points[i].xCoord-minXValue)/(maxXValue-minXValue);
            

            float percY = (points[i].yCoord-minYValue)/(maxYValue-minYValue);
            
            float xpos = startingPosition.x + ((endingPosition.x-startingPosition.x)*percX);
            float ypos = startingPosition.y + ((endingPosition.y-startingPosition.y)*percY);
            float zpos = startingPosition.z;

            lines[points[i].lineNumber].Points[max] = new Vector2(xpos,ypos);

            max++;
        }

    }

    private void CullPoints () {
        
    }

}

[System.Serializable]
public class Point {

    public float xCoord;
    public float yCoord;
    public int lineNumber;
    
    public Point (float x, float y, int lineNumber)
    {
        this.xCoord = x;
        this.yCoord = y;
        this.lineNumber = lineNumber;
    }
}

/*

    CREATED BY : AJAY VENKAT
    PURPOSE : I created this function in order to simplify looping through elements of any type easier, this mimicks the function of the JOB SYSTEM without the multithreading, 
    it allows for more minimalistic code by easily identifying the function applied on each element and not worry about the loop logic. 

*/

public class LoopThrough<T,L> {

    public T[] objArray;
    public List<UnityAction<T>> action;

    public List<Func<T, L>> returnActions;
    
    public LoopThrough (T[] array, UnityAction<T> action) {
        this.objArray = array;
        this.action = new List<UnityAction<T>>();
        this.action.Add(action);
    }

    public LoopThrough (List<T> array, UnityAction<T> action) {
        objArray = array.ToArray();
        this.action = new List<UnityAction<T>>();
        this.action.Add(action);
    }

    public LoopThrough (T[] array, List<UnityAction<T>> actionsInOrder) {
        this.objArray = array;
        this.action = new List<UnityAction<T>>();
        
        for (int i = 0; i < actionsInOrder.Count; i++) {
            this.action.Add(actionsInOrder[i]);
        }
    }

    public LoopThrough (List<T> array, List<UnityAction<T>> actionsInOrder) {
        objArray = array.ToArray();
        this.action = new List<UnityAction<T>>();

        
        for (int i = 0; i < actionsInOrder.Count; i++) {
            this.action.Add(actionsInOrder[i]);
        }

    }

    public LoopThrough (T[] array, Func<T, L> action) {
        this.objArray = array;
        this.returnActions = new List<Func<T, L>>();
        this.returnActions.Add(action);
    }

    public LoopThrough (List<T> array, Func<T, L> action) {
        objArray = array.ToArray();
        this.returnActions = new List<Func<T, L>>();
        this.returnActions.Add(action);
    }




    public L[] PassWithReturn () {
        if (returnActions == null || returnActions.Count == 0)
            Debug.LogError("Can't do a return method if you haven't assigned the functions");
        
        L[] returnVals = new L[objArray.Length];
        for (int i = 0; i < objArray.Length; i++) {
                returnVals[i] = returnActions[0].Invoke(objArray[i]);
        }

        return returnVals;
    }

    public void PassWithAction () {
        for (int i = 0; i < objArray.Length; i++) {
            for (int a = 0; a < action.Count; a++) {
                action[a].Invoke(objArray[i]);
            }
        }
    }


}