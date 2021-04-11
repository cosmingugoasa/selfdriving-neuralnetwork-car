using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using UnityEngine;

public class Trainer : MonoBehaviour
{
    //agent
    public CarController car;

    //population
    public int generation = 0;
    public const int population = 50;
    public int currentAgent = 0;

    public NN bestNet;
    UIManager ui;

    //mutation
    public float mutationRate = 0.25f;
    public float mutationStrenght = 0.005f;

    //data file
    public string xmlNetPath; //= @"C:/Users/cosmi/Documents/nn.xml";

    private void Start()
    {
        ui = GetComponent<UIManager>();
    }

    public NN AnalyzeNetwork(NN _nn) {

        currentAgent++;
        if (currentAgent > population)
        {
            currentAgent = 0;
            generation++;
        }

        //check if new network is better than the current best network
        if (_nn.fitness > bestNet.fitness)
        {
            ui.SetLastFit(_nn.fitness, true);
            ui.SetBestFit(_nn.fitness.ToString("F3"), generation.ToString());
            
            //get new best network
            bestNet = _nn;
            bestNet.generation = generation;
            //save network to file
            SaveNNToXML(bestNet);

            return DeepCopyAndMutate(bestNet);            
        }
        else {
            ui.SetLastFit(_nn.fitness, false);
            //mutate and give back the best net
            return DeepCopyAndMutate(bestNet);
        }        
    }

    NN DeepCopyAndMutate(NN _nn) {
        NN _copy = new NN(new int[] { 5, 4, 2 });
        _copy.Randomize();

        //copy weights       
        for (int x = 0; x < _nn.weights.Length; x++){
            for (int y = 0; y < _nn.weights[x].Length; y++){
                for (int z = 0; z < _nn.weights[x][y].Length; z++){
                    _copy.weights[x][y][z] = _nn.weights[x][y][z];

                    //weight mutation
                    if (Random.Range(0f, 1f) < mutationRate){
                        _copy.weights[x][y][z] = _copy.weights[x][y][z] + Random.Range(-mutationStrenght, mutationStrenght);

                        //keep value between -1 and 1
                        _copy.weights[x][y][z] = Mathf.Clamp(_copy.weights[x][y][z], -1f, 1f);
                    }
                }
            }
        }

        //copy biases
        for (int x = 0; x < _nn.biases.Length; x++) {
            for (int y = 0; y < _nn.biases[x].Length; y++){
                _copy.biases[x][y] = _nn.biases[x][y];

                //bias mutation
                if (Random.Range(0f, 1f) < mutationRate) {
                    _copy.biases[x][y] = _copy.biases[x][y] + Random.Range(-mutationStrenght, mutationStrenght);
                    _copy.biases[x][y] = (float)Mathf.Clamp(_copy.biases[x][y], -1f, 1f);
                }                
            }
        }

        return _copy;
    }

    public void SaveNNToXML(NN _nn) {

        XmlSerializer writer = new XmlSerializer(typeof(NN));
        using (FileStream file = File.Create(xmlNetPath)) {

            writer.Serialize(file, _nn);
        }      
    }

    public NN LoadNNFromXML() {
        NN _loadedNN = new NN(new int[] { 5, 4, 2});
        
        XmlSerializer deserializer = new XmlSerializer(typeof(NN));

        using (FileStream reader = new FileStream(xmlNetPath, FileMode.Open)) {
            _loadedNN = (NN)deserializer.Deserialize(reader);
        }
        
        return _loadedNN;
    }
}
