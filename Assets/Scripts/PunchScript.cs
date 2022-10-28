using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PunchScript : MonoBehaviour
{
    public Animator animator;
    public int health;
    public Collider collider;
    public Rigidbody rb;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Punch();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        //if (collision.gameObject.tag == "Player")
        //{
        //    Punch();
        //}
        Punch();
    }

    public void Punch()
    {
        // Give animator random amount of z angle to twist from -45 to 45
        ///animator.SetFloat("Twist", Random.Range(-45, 45));
        // Animator plays punched clip with given value
        animator.SetTrigger("Punched1");
        // Decrease object health
        health--;
        if (health <= 0)
        {
            Break();
        }
    }

    public void Break()
    {
        // Enable object gravity
        rb.useGravity = true;
        // Allow object to split
        // Create impulse force in front of the object
        // Call Make() from manager

    }
}
