using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;

public class GraphHealer : MonoBehaviour
{
    public static GraphHealer Instance;

    public GraphManager A;
    public GraphManager B;
    public TextMeshProUGUI textMax;
    public TextMeshProUGUI textMin;
    public TextMeshProUGUI textMid;

    
    private void Awake() {
        if (Instance == null)
            Instance = this;
    }
    public void Init() {

    }

    public float maxX=0;
    public float maxY=0;
    public float minX=0;
    public float minY=0;

    public void ResetY() {
        maxY = 0;
        minY = 0;
    }

    public void SetText() {
        textMax.text = Math.Round(maxY,2).ToString();
        textMin.text = Math.Round(minY,2).ToString();
        textMid.text = Math.Round(((maxY+minY)/2),2).ToString();
    }

    public void SetY (Point p) {
        float yP =p.yCoord;
        if (yP >= maxY) {
            if (yP==0) maxY=0.0001f; else maxY = yP;}

        if (yP <= minY) {
            if (yP==0) minY=0.0001f; else minY = yP;}
    
        SetText();

    }

    /*public (float,float,float,float, int) CalculatingMinMax(GraphManager m, GraphManager other) {

        int viable = 0;
        float maxX=m.points[0].xCoord;
        float maxY=m.points[0].yCoord;
        float minX=m.points[0].xCoord;
        float minY=m.points[0].yCoord;

        for (int i = 0; i < m.points.Count; i++) {
            if (m.points[i] == null)
            continue;

            float xP =m.points[i].xCoord;
            float yP =m.points[i].yCoord;

            if (xP >= maxX)
            {if (xP==0) maxX=0.0001f; else maxX = xP;}

            if (xP <= minX) {
                if (xP==0) minX=0.0001f; else minX = xP; }

            if (yP >= maxY) {
                if (yP==0) maxY=0.0001f; else maxY = yP;}

            if (yP <= minY) {
                if (yP==0) minY=0.0001f; else minY = yP;}

            viable ++;

            
        }

        for (int i = 0; i < other.points.Count; i++) {
            if (other.points[i] == null)
            continue;

            float xP =other.points[i].xCoord;
            float yP =other.points[i].yCoord;

            if (xP >= maxX)
            {if (xP==0) maxX=0.0001f; else maxX = xP;}

            if (xP <= minX) {
                if (xP==0) minX=0.0001f; else minX = xP; }

            if (yP >= maxY) {
                if (yP==0) maxY=0.0001f; else maxY = yP;}

            if (yP <= minY) {
                if (yP==0) minY=0.0001f; else minY = yP;}
            
        }

        return (maxX, minX, maxY, minY, viable);

    }*/

}

