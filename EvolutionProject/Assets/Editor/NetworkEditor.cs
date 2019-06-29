using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearRegression;


public class NetworkEditor : EditorWindow
{

    public GeneticEntity entity;
    public SenseNetwork network;

    [MenuItem("AI/Network Editor")]
    static void Init() {
        NetworkEditor window = (NetworkEditor)EditorWindow.GetWindow(typeof(NetworkEditor));
        window.Show();
    }

    void OnGUI() {
        Repaint();
        
        entity = (GeneticEntity)EditorGUI.ObjectField(r(0,0,200,20),entity,typeof(GeneticEntity));
        
        if (entity != null) {
            
            Rect field = r(0,0,200,20);
            for (int i = 0; i < entity.brain.tags.Count; i++) {
                Rect current = o(field,field.size.x+(100*i)+20,0,100,40);
                if (GUI.Button(current,entity.brain.tags[i])) {
                    network = entity.brain.correspondingNetwork[i];
                }
            }

            if (network != null) {
                DrawInputs();
                DrawHiddens();
                DrawOutputs();
                DrawWeights();
            }
            
        }

        Repaint();
    }   

    void DrawInputs() {

        Rect inputs = r(10,100,100,400);
        GUI.Box(inputs,"");
        
        for (int i = 0; i < network.inputValues.Count; i++) {
            Rect val = o(inputs,10,30+i*30,inputs.width-10,30);
            GUI.Label(val,network.inputValues[i].ToString());
            Rect button = o(val, inputs.width-20,0,10,10);
            if (GUI.Button(button,"W")) {
                weightMatrix = network.network.weights[0];
                drawWeights = true;
                targetRow = true;
                columnRowIndex = i;
            }
        }

    }

    bool drawWeights = false;
    Matrix<float> weightMatrix;
    bool targetRow;
    bool targetColumn;
    int columnRowIndex;

    void DrawWeights() {
        if (drawWeights) {

            if (targetRow) {

                for (int i = 0; i < weightMatrix.ColumnCount; i++) {

                    
                }

            }

        }
    }

    void DrawOutputs() {
        
        Rect inputs = r(position.width-210,100,200,400);
        GUI.Box(inputs,"");
        for (int i = 0; i < network.outputValues.Count; i++) {
            Rect val = o(inputs,10,30+i*30,inputs.width-10,30);
            GUI.Label(val,network.possibleActions[i] + " : " + network.outputValues[i].ToString());
            Rect button = o(val, inputs.width-20,0,10,10);
            if (GUI.Button(button,"W")) {

            }
        }
    }   

    public float wieghtLength;
    void DrawHiddens() {
        
        Rect inputs = r((10+100),100,100,400);
        
        float difference = ((position.width-210)-100);
        float spacing = (difference-(100*network.network.hiddenLayers.Count))/network.network.hiddenLayers.Count;

    

        for (int i = 0; i < network.network.hiddenLayers.Count; i++) {
            Matrix<float> currentHiddenLayer = network.network.hiddenLayers[i];
            Rect layer = o (inputs,(120*i)+spacing,0,100,400);

            GUI.Box(layer,"");

            for (int w = 0; w < currentHiddenLayer.ColumnCount; w++) {
                Rect val = o(layer,10,30+w*30,layer.width,30);
                GUI.Label(val,currentHiddenLayer[0,w].ToString());
                Rect button = o(val, layer.width-20,0,10,10);
                if (GUI.Button(button,"W")) {

                }
            }

            
        }

    }

#region Helpers
    Rect r(float x, float y, float xS, float yS)
    {
        return new Rect(new Vector2(x, y), new Vector2(xS, yS));
    }
    Rect rOut(float x, float y, float xS, float yS, out Rect a)
    {
        Rect rect = new Rect(new Vector2(x, y), new Vector2(xS, yS));
        a = rect;
        return rect;
    }

    Rect o(Rect r, float xOff, float yOff, float xS, float yS)
    {
        return new Rect(new Vector2(r.x + xOff, r.y + yOff), new Vector2(xS, yS));
    }
    Rect oOut(Rect r, float xOff, float yOff, float xS, float yS, out Rect a)
    {
        Rect l = new Rect(new Vector2(r.x + xOff, r.y + yOff), new Vector2(xS, yS));
        a = l;
        return l;
    }

    Rect duplicate(Rect r, float xOff, float yOff)
    {
        return new Rect(new Vector2(r.x + xOff, r.y + yOff), r.size);
    }

    private Texture2D MakeTex(int width, int height, Color col)
    {
        Color[] pix = new Color[width * height];
        for (int i = 0; i < pix.Length; ++i)
        {
            pix[i] = col;
        }
        Texture2D result = new Texture2D(width, height);
        result.SetPixels(pix);
        result.Apply();
        return result;
    }
#endregion

}
