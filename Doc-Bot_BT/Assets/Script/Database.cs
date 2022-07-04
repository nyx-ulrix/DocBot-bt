using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Database : MonoBehaviour
{
    //list of beds location
    public List<GameObject> bed;
    //list of patients that are currently in the scene
    public List<GameObject> patientlist;
    //integers to store the components
    public int Motor = 1;
    public int Sensor = 1;
    public int Camera = 1;
    public int CPU = 1;
    public int Motherboard = 1;
}
