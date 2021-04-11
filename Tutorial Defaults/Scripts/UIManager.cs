using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    Trainer trainer;
    public CarController car;

    //info
    public Text generation;
    public Text agent;
    public Text lastFit;
    public Text bestFit;
    public Text bestFitGen;
    public Text distance;

    public Slider timeMulti;
    public Text timeMultiText;

    //menu
    public Text notFound;
    public Text filePath;

    public Toggle checkBox;

    public void Start()
    {
        trainer = GetComponent<Trainer>();
    }
    public void Update()
    {
        generation.text = trainer.generation.ToString();
        agent.text = trainer.currentAgent.ToString();

        distance.text = car.distance.ToString();

        Time.timeScale = timeMulti.value;
        timeMultiText.text = timeMulti.value.ToString();

    }

    public void SetLastFit(float _fitness, bool _compare) {
        lastFit.text = _fitness.ToString();
        if (_compare)
        {
            lastFit.color = Color.green;
        }
        else {
            lastFit.color = Color.red;
        }
    }

    public void SetBestFit(string _bestNet, string _gen) {
        bestFit.text = _bestNet + " @gen ";
        bestFitGen.text = _gen;
    }
}
