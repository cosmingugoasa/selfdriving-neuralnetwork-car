using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

[Serializable]
public class NN 
{
    
    public int[] layers;
    public float[][] neurons;
    public float[][] biases;
    public float[][][] weights;

    public float fitness = 0;
    public int generation = 0;

    //empty constructor required for xml serialization
    public NN() { }
    public NN(int[] _layers) {
        layers = new int[_layers.Length];
        for (int i = 0; i < _layers.Length; i++) {
            layers[i] = _layers[i];
        }

        //allocate neurons memory
        List<float[]> _neurons = new List<float[]>();

        for (int l = 0; l < layers.Length; l++) {
            _neurons.Add(new float[layers[l]]);
        }
        neurons = _neurons.ToArray();
    }

    public void Randomize() {

        //initialize and randomize biases
        List<float[]> _biases = new List<float[]>();

        for (int b = 0; b < layers.Length; b++)
        {
            float[] _b = new float[layers[b]];
            for (int i = 0; i < layers[b]; i++)
            {
                _b[i] = Random.Range(-0.5f, 0.5f);
            }
            _biases.Add(_b);
        }
        biases = _biases.ToArray();

        //initialize and randomize weights

        List<float[][]> _weightsList = new List<float[][]>();
        for (int i = 1; i < layers.Length; i++)
        {
            List<float[]> _layerWeightsList = new List<float[]>();
            int _neuronsInPreviousLayer = layers[i - 1];
            for (int n = 0; n < neurons[i].Length; n++)
            {
                float[] _neuronWeights = new float[_neuronsInPreviousLayer];
                for (int k = 0; k < _neuronsInPreviousLayer; k++)
                {
                    _neuronWeights[k] = Random.Range(-0.5f, 0.5f);
                }
                _layerWeightsList.Add(_neuronWeights);
            }
            _weightsList.Add(_layerWeightsList.ToArray());
        }
        weights = _weightsList.ToArray();
    }

    public (float, float) Run(float _A, float _B, float _C, float _L, float _R) {
        
        neurons[0][0] = _A;
        neurons[0][1] = _B;
        neurons[0][2] = _C;
        neurons[0][3] = _L;
        neurons[0][4] = _R;

        //feed forward process
        for (int l = 1; l < layers.Length; l++) {
            for (int cn = 0; cn < layers[l]; cn++) {                                        //cn = current neuron
                for (int pn = 0; pn < layers[l - 1]; pn++) {                                //pn = previous neuron
                    neurons[l][cn] += (neurons[l - 1][pn] * weights[l - 1][cn][pn]);
                }
                //activation function
                if (l == layers.Length - 1)
                {
                    neurons[l][0] = Sigmoid(neurons[l][0] + biases[l][0]);                  //accelerator (0, 1)
                    neurons[l][1] = (float)Math.Tanh(neurons[l][1] + biases[l][1]);         //steering (-1, 1)
                }
                else {
                    neurons[l][cn] = (float)Math.Tanh(neurons[l][cn] + biases[l][cn]);
                }
            }
        }

        return (neurons[layers.Length-1][0], neurons[layers.Length-1][1]);
    }

    float Sigmoid(float _x) {
        return 1 / (1 + Mathf.Exp(-_x));
    }
}
