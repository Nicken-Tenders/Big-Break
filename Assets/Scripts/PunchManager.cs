using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PunchManager : MonoBehaviour
{
    public GameObject[] queue;

    // Start is called before the first frame update
    void Start()
    {
        Make();
    }
    
    public void Make()
    {
        // ? Fade out current object
        // Fade in next object from queue
        // Make next object the current object
    }

    //private void OnTriggerEnter(Collider other)
    //{
    //    if (other.transform.parent.name == "PrimaryHand")
    //    {
    //        BroadcastMessage("Punch");
    //        BroadcastMessage("Vibrate", OVRInput.Controller.RTouch);

    //    }
    //    if (other.transform.parent.name == "SecondaryHand")
    //    {
    //        BroadcastMessage("Punch");
    //        BroadcastMessage("Vibrate", OVRInput.Controller.LTouch);
    //    }
    //}
}
