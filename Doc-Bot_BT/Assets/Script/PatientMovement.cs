using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PatientMovement : MonoBehaviour
{
    //target location
    public GameObject WayPoint;
    //nav mesh agent
    public NavMeshAgent patientNav;

    private void Update()
    {
        //sets the destination of the player
        patientNav.SetDestination(WayPoint.transform.position);
    }

    //used in patient manager script
    public void setWaypoint(GameObject waypoint)
    {
        ///set the waypoint to be the bed specified
        WayPoint = waypoint.gameObject;
    }

    //used in the task script under the return task
    public void gotoWaypoint(GameObject returnWaypoint)
    {
        //truns on the renderer of the patient that was off
        gameObject.GetComponent<MeshRenderer>().enabled = true;
        //sets the waypoint to be thr return loaction
        WayPoint = returnWaypoint.gameObject;
    }
}