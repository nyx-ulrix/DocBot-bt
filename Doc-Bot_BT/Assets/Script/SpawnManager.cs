using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    //spawner ui element
    public GameObject spawnner;
    //to retrieve data for the number of patients
    public Database data;

    public PatientManager counter;

    public void close()
    {
        //closes the spawnner UI when the button is pressed
        spawnner.active = false;
        //set the count back to zero because the ui can only open when there are no more patients
        counter.numberOfPatients = 0;

    }
}
