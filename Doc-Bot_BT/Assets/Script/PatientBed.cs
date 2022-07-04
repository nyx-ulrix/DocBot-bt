using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatientBed : MonoBehaviour
{
    //gameobject that will be visible when the patient reaches the bed
    public GameObject PatientSleep;
    //gameobeject that represents the quarantine wall
    public GameObject QuarantineWall;

    private void OnTriggerEnter(Collider other)
    {
        if (other.name == "Patient(Clone)")
        {
            //shows the patient on the bed
            PatientSleep.active = true;
            //hides the patient that was spawnned (but is still active so that the tag is accessible)
            other.GetComponent<MeshRenderer>().enabled = false;
        }

        else if (other.name == "VirusPatient(Clone)")
        {
            //shows the patient on the bed
            PatientSleep.active = true;
            //shows the quarantine walls
            QuarantineWall.active = true;
            //hides the patient that was spawnned (but is still active so that the tag is accessible)
            other.GetComponent<MeshRenderer>().enabled = false;
        }
    }

    public void OnTriggerExit(Collider other)
    {
        //if the patients exits the collider
        if (other.name == "Patient(Clone)" || other.name == "VirusPatient(Clone)")
        {
            //turns off sleeping patient gamobeject
            PatientSleep.active = false;
            //turns off quarantine walls
            QuarantineWall.active = false;
        }
    }
}
