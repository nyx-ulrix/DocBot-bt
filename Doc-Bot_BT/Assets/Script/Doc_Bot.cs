using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Panda;
using UnityEngine.UI;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class Doc_Bot : MonoBehaviour
{
    //na mesh agent
    public NavMeshAgent DocNav;
    //waypoints
    public GameObject recycleFacility;
    public GameObject supplyFacility;
    public GameObject disposalFacility;
    public GameObject returnPoint;

    //the player gamobject to check if the player is near teh doctor
    public GameObject Player;

    //reference to the paitentManager script in the canvas that
    //will retrieve the list of patients and beds from the spawner
    public Database data;

    //stores the current patient gameobject
    public GameObject currentPatient;

    //boolean to check if the patients is still alive
    public bool currentPatientLive;
    public bool currentPatientFix;

    //string to store what part is broken
    public string broken;

    //ui Gameobjects
    public GameObject restockUI;
    public GameObject recycleUI;
    public GameObject disposeUI;
    public GameObject returnUI;
    public GameObject RespawnDocUI;
    public GameObject spawnnerUI;


    public Text display;


    [Task]
    void idle()
    {
        DocNav.destination = returnPoint.transform.position;
        //shows text at the top for easy refence
        changeText("idle");
        if (Vector3.Distance(gameObject.transform.position, Player.transform.position) < 5 && data.patientlist.Count == 0)
        {
            spawnnerUI.active = true;


        }
        //if there is a patient(reference list from database script)
        if (data.patientlist.Count > 0)
        {
            //make sure that the current patient shows up as not alive but not fixed
            currentPatientLive = true;
            currentPatientFix = false;
            //set behaviour to be successful
            Task.current.Succeed();
        }
    }
    [Task]
    void moveToPatient()
    {
        //shows text at the top for easy refence
        changeText("moving to patient");
        //control the navmesh to go to the patient
        DocNav.SetDestination(data.patientlist[0].transform.position);
        //when the doc bot is at the patient
        if (Vector3.Distance(gameObject.transform.position, data.patientlist[0].transform.position) < 1)
        {
            //return successful
            Task.current.Succeed();
        }

    }
    [Task]
    void release()
    {
        if (currentPatientFix == true && currentPatientLive == true)
        {
            DocNav.SetDestination(returnPoint.transform.position);
            data.patientlist[0].GetComponent<PatientMovement>().gotoWaypoint(returnPoint);
            if (Vector3.Distance(gameObject.transform.position, returnPoint.transform.position) > 2)
            {
                //shows text at the top for easy reference
                changeText("moving to release");
            }
            else
            {
                //shows text at the top for easy reference
                changeText("Returned");
                removePatient();
                Task.current.Fail();
            }
        }
        else if (currentPatientFix == false && currentPatientLive == false)
        {
            removePatient();
        }
    }
    [Task]
    void virus()
    {
        //shows text at the top for easy refence
        changeText("virus check");
        //if the patients name is virusPatient(Clone)(its a prefab)
        if (data.patientlist[0].name == "VirusPatient(Clone)")
        {
            //set the broken string to be the tag of the first patient
            broken = data.patientlist[0].tag;
            //return successful
            Task.current.Succeed();
        }
        else
        {
            //set the broken string to be the tag of the first patient
            broken = data.patientlist[0].tag;
            //return fail     
            Task.current.Fail();
        }

    }
    [Task]
    void selfInfected()
    {
        //shows text at the top for easy refence
        changeText("self infected");
        //if the tag of the patient is viral
        if (data.patientlist[0].tag == "Viral")
        {
            //return fail when the bot is infected so that it transitions to  the fix self behaviour
            Task.current.Fail();
        }
        else
        {
            //returns successful when the bot is not infected so that it doesnt try to fix itself
            //an moves to repair behaviour instead
            Task.current.Succeed();
        }
    }
    [Task]
    void fixSelf()
    {
        //shows text at the top for easy refence
        changeText("fixing self");
        //if randomizer that decides if the bot is fixed
        int fixedChance = Random.Range(1, 10);
        if (fixedChance <= 9)
        {
            //shows text at the top for easy refence
            changeText("self fixed");
            //if the bot is fixed return sucessful so that it can move on to repair
            Task.current.Succeed();
        }
        else
        {
            //shows text at the top for easy refence
            changeText("infected disposing");
            //moves te doctor to the disposal facility
            DocNav.SetDestination(disposalFacility.transform.position);
            //moves patient to dispose facility
            data.patientlist[0].GetComponent<PatientMovement>().gotoWaypoint(disposalFacility);
            //when the doc and the patient is at the disposal facility
            if (Vector3.Distance(gameObject.transform.position, disposalFacility.transform.position) < 2)
            {
                //show the bot dead UI
                RespawnDocUI.active = true;
                //stop time
                Time.timeScale = 0;
                //when f is pressed
                if (Input.GetKey("f") == true)
                {
                    //turns off dispose UI
                    disposeUI.active = false;
                    //removes the patient from the list
                    removePatient();
                    //instantiate new instance of the docbot
                    GameObject current = Instantiate(gameObject, new Vector3(7, 1, -15), Quaternion.identity) as GameObject;
                    //truns off the respawn ui
                    RespawnDocUI.active = false;
                    //set time back to 0
                    Time.timeScale = 1;
                    //destroy current instance of docbot
                    Destroy(gameObject);
                }
            }

        }
    }
    [Task]
    void repair()
    {
        //shows text at the top for easy refence
        changeText("going to repair");
        //make sure that the doctor is at the patient
        DocNav.SetDestination(data.patientlist[0].transform.position);
        //when the doctor is at the patient
        if (Vector3.Distance(gameObject.transform.position, data.patientlist[0].transform.position) < 1)
        {
            //random chance that the repair succeeds or fails
            int fixedChance = Random.Range(1, 10);
            //if the fix is successful
            if (fixedChance >= 5)
            {
                //shows text at the top for easy refence
                changeText("repaired");
                //set the boolean that shows that the current patient is fixed
                currentPatientFix = true;
                //succeed to transition to next behaviour
                Task.current.Succeed();
            }
            else
            {
                //shows text at the top for easy refence
                changeText("died");
                //set the boolean that shows that the current patient is dead
                currentPatientLive = false;
                //fail to transition to recycle
                Task.current.Fail();
            }
        }

    }
    [Task]
    void repairVirus()
    {
        //shows text at the top for easy refence
        changeText("Rebooting");
        //make sure that the doctor is at the patient
        DocNav.SetDestination(data.patientlist[0].transform.position);
        //when the doctor is at the patient
        if (Vector3.Distance(gameObject.transform.position, data.patientlist[0].transform.position) < 1)
        {
            //random chance that the repair succeeds or fails
            int fixedChance = Random.Range(1, 10);
            //if the fix is successful
            if (fixedChance >= 5)
            {
                //shows text at the top for easy refence
                changeText("Fixed");
                //set the boolean that shows that the current patient is fixed
                currentPatientFix = true;
                //succeed to transition to next behaviour
                Task.current.Succeed();
            }
            else
            {
                //shows text at the top for easy refence
                changeText("Patient dead");
                //set the boolean that shows that the current patient is dead
                currentPatientLive = false;
                //fail to transition to recycle
                Task.current.Fail();
            }
        }

    }
    [Task]
    void checkStatus()
    {
        //shows text at the top for easy refence
        changeText("checking status");
        //if the patient is not fixed and has a broken part
        if (data.patientlist[0].name == "Patient(Clone)" && currentPatientFix == false && currentPatientLive == true)
        {
            //set the broken string to whatever part is broken
            broken = data.patientlist[0].tag;
            //return successful so that the bot will go to storage
            Task.current.Succeed();
        }
        else
        {
            //otherwise go to the release 
            Task.current.Fail();
        }
    }
    [Task]
    void goToStorage()
    {
        //shows text at the top for easy refence
        changeText("going to storage");
        //set the target location to be the storage facility
        DocNav.SetDestination(supplyFacility.transform.position);
        //when the bot reaches the storage facility
        if (Vector3.Distance(gameObject.transform.position, supplyFacility.transform.position) < 0.5)
        {
            //return succeed so that it will take the stock
            Task.current.Succeed();
        }

    }
    [Task]
    void checkItem()
    {
        //shows text at the top for easy refence
        changeText("looking for part");
        //all of the if else repeats but checks for a different item
        //cheks for the stock
        if (broken == "CPU" && data.CPU > 0)
        {
            //removes 1 for the stock
            data.CPU = data.CPU - 1;
            //goes to repair
            Task.current.Succeed();

        }

        else if (broken == "Sensor" && data.Sensor > 0)
        {
            data.Sensor = data.Sensor - 1;
            Task.current.Succeed();

        }

        else if (broken == "Camera" && data.Camera > 0)
        {
            data.Camera = data.Camera - 1;
            Task.current.Succeed();

        }

        else if (broken == "Motor" && data.Motor > 0)
        {
            data.Motor = data.Motor - 1;
            Task.current.Succeed();

        }

        else if (broken == "Motherboard" && data.Motherboard > 0)
        {
            data.Motherboard = data.Motherboard - 1;
            Task.current.Succeed();

        }
        //if there is no stock move to the restock behaviour
        else
        {
            Task.current.Fail();
        }
    }
    [Task]
    void restockItem()
    {
        //shows text at the top for easy refence
        changeText("restock?");
        //all of the if else repeats but checks for a different item
        //show that the restock ui
        restockUI.active = true;
        //if the y key is pressed
        if (Input.GetKey("y") == true)
        {
            //shows text at the top for easy refence
            changeText("restocking");
            //checks which part is the broken
            //(because the item that has no stock will be the one that needs to be restocked) 
            if (broken == "CPU")
            {
                //adds 3 to the stock
                data.CPU = data.CPU + 3;
                //close the restock UI
                restockUI.active = false;
                //return success so that it will go to repair
                Task.current.Succeed();
            }

            else if (broken == "Sensor")
            {
                data.Sensor = data.Sensor + 3;
                restockUI.active = false;
                Task.current.Succeed();
            }

            else if (broken == "Camera")
            {
                data.Camera = data.Camera + 3;
                restockUI.active = false;
                Task.current.Succeed();
            }

            else if (broken == "Motor")
            {
                data.Motor = data.Motor + 3;
                restockUI.active = false;
                Task.current.Succeed();
            }

            else if (broken == "Motherboard")
            {
                data.Motherboard = data.Motherboard + 3;
                restockUI.active = false;
                Task.current.Succeed();
            }

        }
        //when the n key is pressed
        else if (Input.GetKey("n") == true)
        {
            //shows text at the top for easy refence
            changeText("paitent died");
            //shows that the patient has died
            currentPatientLive = false;
            //close the restock UI
            restockUI.active = false;
            //returns fail so that it will go recycle
            Task.current.Fail();
        }

    }
    [Task]
    void recycle()
    {

        //shows text at the top for easy refence
        changeText("recycle?");
        //if the doc bot is not at the patient
        if (Vector3.Distance(data.patientlist[0].transform.position, gameObject.transform.position) > 1)
        {
            //moved the doctor to the patient
            DocNav.SetDestination(data.patientlist[0].transform.position);
        }
        //if the player is with the patient
        else if (Vector3.Distance(data.patientlist[0].transform.position, gameObject.transform.position) < 1)

        {
            //move doctor to recycle facility
            DocNav.SetDestination(recycleFacility.transform.position);
            //makes the player move to the recycle and turns on the mesh renderer
            data.patientlist[0].GetComponent<PatientMovement>().gotoWaypoint(recycleFacility);
        }

        //turns the recycle UI on
        recycleUI.active = true;
        //when the y key is pressed
        if (Input.GetKey("r") == true)
        {
            //shows text at the top for easy refence
            changeText("recycling");
            //all of the if else repeats but checks for a different item
            //checks which part is broken
            if (broken == "CPU")
            {
                //restock everything that isnt broken by 1
                data.Sensor = data.Sensor + 1;
                data.Camera = data.Camera + 1;
                data.Motor = data.Motor + 1;
                data.Motherboard = data.Motherboard + 1;
                //turns off the recycle UI
                recycleUI.active = false;
                //succeed so that it will move to throw away everything else(next behaviour)
                Task.current.Succeed();
            }

            else if (broken == "Sensor")
            {
                data.CPU = data.CPU + 1;
                data.Camera = data.Camera + 1;
                data.Motor = data.Motor + 1;
                data.Motherboard = data.Motherboard + 1;
                recycleUI.active = false;
                Task.current.Succeed();
            }

            else if (broken == "Camera")
            {
                data.CPU = data.CPU + 1;
                data.Sensor = data.Sensor + 1;
                data.Motor = data.Motor + 1;
                data.Motherboard = data.Motherboard + 1;
                recycleUI.active = false;
                Task.current.Succeed();
            }

            else if (broken == "Motor")
            {
                data.CPU = data.CPU + 1;
                data.Sensor = data.Sensor + 1;
                data.Camera = data.Camera + 1;
                data.Motherboard = data.Motherboard + 1;
                recycleUI.active = false;
                Task.current.Succeed();
            }

            else if (broken == "Motherboard")
            {
                data.CPU = data.CPU + 1;
                data.Sensor = data.Sensor + 1;
                data.Camera = data.Camera + 1;
                data.Motor = data.Motor + 1;
                recycleUI.active = false;
                Task.current.Succeed();
            }

            else if (broken == "Virus")
            {
                data.CPU = data.CPU + 1;
                data.Sensor = data.Sensor + 1;
                data.Camera = data.Camera + 1;
                data.Motor = data.Motor + 1;
                data.Motherboard = data.Motherboard + 1;
                recycleUI.active = false;
                Task.current.Succeed();
            }

            else if (broken == "Viral")
            {
                data.CPU = data.CPU + 1;
                data.Sensor = data.Sensor + 1;
                data.Camera = data.Camera + 1;
                data.Motor = data.Motor + 1;
                data.Motherboard = data.Motherboard + 1;
                recycleUI.active = false;
                Task.current.Succeed();
            }
        }
        //if n key is pressed
        else if (Input.GetKey("d") == true)
        {
            //turns off the recycle UI
            recycleUI.active = false;
            //return fail to restart the behaviour tree
            //shows text at the top for easy refence
            changeText("disposing");
            //moves te doctor to the disposal facility
            DocNav.SetDestination(disposalFacility.transform.position);
            //turns on dispose UI
            Task.current.Succeed();
        }
    }
    [Task]
    void dispose()
    {
        //shows text at the top for easy refence
        changeText("disposing");
        //moves te doctor to the disposal facility
        DocNav.SetDestination(disposalFacility.transform.position);
        //moves patient to dispose facility
        data.patientlist[0].GetComponent<PatientMovement>().gotoWaypoint(disposalFacility);
        //turns on dispose UI
        disposeUI.active = true;
        //when space bar is pressed
        if (Input.GetKey("space") == true)
        {
            //turns off dispose UI
            disposeUI.active = false;
            //removes the patient from the list
            removePatient();
            //succeed so that it will restart the behaviour and move on to the next patient
            Task.current.Succeed();
        }
    }
    [Task]
    void isBroken()
    {
        //if the paitent is alive and fixed
        if (currentPatientFix == false && currentPatientLive == false)
        {
            //shows text at the top for easy refence
            changeText("paitent dead");
            //return fail so that it doesnt get recycled
            Task.current.Succeed();
        }
        else if (currentPatientFix == true && currentPatientLive == true)
        {
            //shows text at the top for easy refence
            changeText("patient fixed");
            //return succeed to recycle or dispose the bot
            Task.current.Fail();
        }
    }
    void removePatient()
    {
        //integer to make sure that the code only runs once
        int i = 0;
        if (i == 0)
        {
            //destroy patient gameobject
            Destroy(data.patientlist[0].gameObject);
            //remove the patient from the list
            data.patientlist.Remove(data.patientlist[0]);
            //makes sure that the code only runs once
            i = i + 1;
        }
    }

    void changeText(string behaviour)
    {
        display.text = (behaviour);
    }
}

