using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UIElements;

public class Menu : MonoBehaviour
{
    public Trainer trainer;
    public CarController car;

    public GameObject menuPanel;

    public UIManager ui;

    public void StartNewTraining() {
        menuPanel.SetActive(false);
        trainer.enabled = true;
        car.enabled = true;
    }

    public void LoadTraining() {

        if (!File.Exists(ui.filePath.text))
        {
            ui.notFound.text = "File not found. Try different path";
            return;
        }
        else {
            trainer.xmlNetPath = ui.filePath.text;

            if (ui.checkBox.isOn) {
                car.playbackOnly = true;
            }
        }

        menuPanel.SetActive(false);
        trainer.enabled = true;
        car.enabled = true;
        car.loaded = true;

        trainer.bestNet = trainer.LoadNNFromXML();
        trainer.generation = trainer.bestNet.generation;
        car.NN = trainer.LoadNNFromXML();

        ui.SetBestFit(trainer.bestNet.fitness.ToString(), trainer.generation.ToString());
    }
}
