using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MonitoringInfo : MonoBehaviour
{
    //references the text gameobject in the scene
    public Text infoPanel;
    //to get the number of patients
    public Database data;
    //to get the data on what is the problem with the current patient
    public Doc_Bot bot;

    private void Update()
    {
        //retrieves the data and shows it in the infopanel UI gameobject
        infoPanel.text = 
            ("Patients:" + data.patientlist.Count.ToString() + "/6" +
            "\nstocks\nMotor:" + data.Motor.ToString() + 
            "\nSensor:" + data.Sensor.ToString() + 
            "\nCamera:" + data.Camera.ToString() + 
            "\nCPU:" + data.CPU.ToString() + 
            "\nMotherBoard:" + data.Motherboard.ToString())+
            "\ncurrent Patient problem:" + bot.broken.ToString();
    }

}
