using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PunchScript : MonoBehaviour
{
    public Animator animator;
    public int health;
    public Rigidbody rb;
    public Transform impulsePoint;
    public PunchManager pm;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            Punch();
    }

    /*private void OnTriggerEnter(Collider other)
    {
        Punch();
        if (other.tag == "RHand")
            StartCoroutine(Vibrate(OVRInput.Controller.RTouch));
        if (other.tag == "LHand")
            StartCoroutine(Vibrate(OVRInput.Controller.LTouch));
    }*/

    public void Punch()
    {
        // Give animator random amount of z angle to twist from -45 to 45
        ///animator.SetFloat("Twist", Random.Range(-45, 45));
        // Animator plays punched clip with given value
        animator.SetTrigger("Punched");
        // Vibrate controller
        /// if (lefthand)
        /// StartCoroutine(Vibrate(Controller.LTouch));
        /// else if (righthand)
        /// StartCoroutine(Vibrate(Controller.RTouch));
        /// else
        /// null
        // Decrease object health
        health--;

        if (health <= 0)
            Break();
    }

    public void Break()
    {
        // Enable object gravity
        rb.useGravity = true;
        // Allow object to split
        // Create impulse force in front of the object
        rb.AddExplosionForce(1, impulsePoint.position, 0.1f);
        // Call Make() from manager
        pm.Make();
    }

    public IEnumerator Vibrate(OVRInput.Controller controller)
    {
        OVRInput.SetControllerVibration(1, 1, controller);
        yield return new WaitForSecondsRealtime(0.2f);
        OVRInput.SetControllerVibration(1, 1, controller);
    }
}