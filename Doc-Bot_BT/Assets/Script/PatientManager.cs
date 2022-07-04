using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PatientManager : MonoBehaviour
{
    //reference to the database
    public Database data;
    //to store the patient prefabs
    public GameObject Patient;
    public GameObject Virus;
    //gameobject to turn off the selecor when the hospital is full
    public GameObject selector;
    //reference to the docbot gameobject to set the list
    public GameObject docBot;
    //to check the number of patients to that no more than 6 paients is spawwned at a time
    public int numberOfPatients;


    private void Start()
    {
        //make sure that the number of patient is 0
        numberOfPatients = 0;
    }

    //function to spawn patients
    public void Spawn(string Tag)
    {
        if (numberOfPatients <= 5)
        {
            if (Tag == "Virus" || Tag == "Viral")
            {
                //spawns patient prefab
                GameObject current = Instantiate(Virus, new Vector3(5, 1, -15), Quaternion.identity) as GameObject;
                //changes the patients tag to whatever is written in the inspector
                current.tag = Tag;
                //adds the patientss to the patient list
                data.patientlist.Add(current);
                //sets the waypoint of to an empty data.bed
                current.gameObject.GetComponent<PatientMovement>().setWaypoint(data.bed[numberOfPatients]);
                //adds one to the integer so that only 6 patients can exist
                numberOfPatients = numberOfPatients + 1;
            }

            else
            {
                //spawns patient prefab
                GameObject current = Instantiate(Patient, new Vector3(7, 1, -15), Quaternion.identity) as GameObject;
                //changes the patients tag to whatever is written in the inspector
                current.tag = Tag;
                //adds the patientss to the patient list
                data.patientlist.Add(current);
                //sets the waypoint of to an empty data.bed
                current.gameObject.GetComponent<PatientMovement>().setWaypoint(data.bed[numberOfPatients]);
                //adds one to the integer so that only 6 patients can exist
                numberOfPatients = numberOfPatients + 1;
            }
        }
        else if (numberOfPatients == 6)
        {
            //turns off the selector that spawns the patients
            selector.active = false;
            //changes the number of patients back to 6
            numberOfPatients = 0;

        }
    }
}

    