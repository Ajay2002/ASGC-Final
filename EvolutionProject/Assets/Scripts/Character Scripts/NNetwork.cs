using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearRegression;

[System.Serializable]
//TODO: Install Math libaries
public class NNetwork
{
    #region Variables

    public Matrix<float> inputLayer = Matrix<float>.Build.Dense(1, 3);
    public List<Matrix<float>> hiddenLayers = new List<Matrix<float>>();
    public Matrix<float> outputLayer = Matrix<float>.Build.Dense(1, 2);

    public List<Matrix<float>> weights = new List<Matrix<float>>();
    public List<float> biases = new List<float>();

    public float fitness;
    public bool random = false;
    
    #endregion

    #region Initialise Network  

    int inpCount;
    int outCount;

    public void Initialise (int hiddenLayerCount, int hiddenNeuronCount, int inputCount, int outputCount)
    {
        random = true;
        inputLayer.Clear();
        hiddenLayers.Clear();
        outputLayer.Clear();
        weights.Clear();
        biases.Clear();

        inputLayer = Matrix<float>.Build.Dense(1, inputCount);
        outputLayer = Matrix<float>.Build.Dense(1, outputCount);

        inpCount = inputCount;
        outCount = outputCount;

        for (int i = 0; i < hiddenLayerCount+1; i++)
        {
           
            Matrix<float> f = Matrix<float>.Build.Dense(1, hiddenNeuronCount);
            hiddenLayers.Add(f);
            biases.Add(UnityEngine.Random.Range(-1f, 1f));
            //First
            if (i == 0)
            {
                Matrix<float> inputToH1 = Matrix<float>.Build.Dense(inputCount, hiddenNeuronCount);
                weights.Add(inputToH1);
                continue;
            }

           
            Matrix<float> HtoH = Matrix<float>.Build.Dense(hiddenNeuronCount, hiddenNeuronCount);
            weights.Add(HtoH);

        }

        Matrix<float> OutputWeight = Matrix<float>.Build.Dense(hiddenNeuronCount, outputCount);
        weights.Add(OutputWeight);
        biases.Add(UnityEngine.Random.Range(-1f, 1f));

        RandomiseWeights();

    }

    public void InitialiseHidden(int hiddenLayerCount, int hiddenNeuronCount)
    {
        random = false;
        inputLayer.Clear();
        hiddenLayers.Clear();
        outputLayer.Clear();

        for (int i = 0; i < hiddenLayerCount + 1; i++)
        {

            Matrix<float> f = Matrix<float>.Build.Dense(1, hiddenNeuronCount);
            hiddenLayers.Add(f);
           
        }
        
    }

    public void RandomiseWeights()
    {
        for (int i = 0; i < weights.Count; i++)
        {
            for (int x = 0; x < weights[i].RowCount; x++)
            {
                for (int y = 0; y < weights[i].ColumnCount; y++)
                {
                    weights[i][x, y] = UnityEngine.Random.Range(-1f, 1f);
                }
            }
        }
    }

    public NNetwork InitialiseCopy (int hiddenLayerCount, int hiddenNeuronCount)
    {
        NNetwork n = new NNetwork();
       
        List<Matrix<float>> newWeights = new List<Matrix<float>>();
        
        for (int i = 0; i < weights.Count; i++)
        {

            //Loop through matrix
            Matrix<float> newDim = Matrix<float>.Build.Dense(weights[i].RowCount, weights[i].ColumnCount);

            for (int x = 0; x < newDim.RowCount; x++)
            {
                for (int y = 0; y < newDim.ColumnCount; y++)
                {
                    newDim[x, y] = weights[i][x, y];

                }
            }

            newWeights.Add(newDim);


        }

        List<float> newBiases = new List<float>();

        for (int i = 0; i < biases.Count; i++)
        {
            newBiases.Add(biases[i]);
        }


        n.weights = newWeights;
        n.biases = newBiases;

        n.InitialiseHidden(hiddenLayerCount, hiddenNeuronCount);

        return n;

    }

    #endregion

    #region Network

    public List<float> RunNetwork (List<float> inputs)
    {
        
        for (int i = 0; i < inpCount; i++) {
            inputLayer[0,i] = inputs[i];
        }

        //inputLayer = SigmoidActivate(inputLayer);

        hiddenLayers[0] = ((inputLayer * weights[0]) + biases[0]).PointwiseTanh();

        for (int i = 1; i < hiddenLayers.Count; i++)
        {
            hiddenLayers[i] = ((hiddenLayers[i - 1] * weights[i]) + biases[i]).PointwiseTanh();
        }

        outputLayer = ((hiddenLayers[hiddenLayers.Count - 1] * weights[weights.Count - 1]) + biases[biases.Count - 1]).PointwiseTanh();

        List<float> returnList = new List<float>();

        for (int i = 0 ; i < outCount; i++) {
            returnList.Add( Sigmoid(outputLayer[0,i]) );
        }        

        return returnList;
    }

    public Matrix<float> SigmoidActivate (Matrix<float> a)
    {
        Matrix<float> ar = a;

        for (int x = 0; x < ar.RowCount; x++)
        {
            for (int y = 0; y < ar.ColumnCount; y++)
            {
                ar[x, y] = Sigmoid(ar[x, y]);
            }
        }
        return ar;
    }

    private float Sigmoid(float s)
    {
        return (1 / (1 + Mathf.Exp(-s)));
    }

    #endregion

}
