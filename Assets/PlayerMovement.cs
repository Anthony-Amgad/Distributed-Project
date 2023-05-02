using UnityEngine;

// A C# program for Client
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
 


public class PlayerMovement : MonoBehaviour
{

    public int playerCount;
    public Rigidbody rb;
    public Material[] materials;
    Renderer rend;
    private float forwardForce = 2000f;
    private float sidewaysForce = 150f;

    private void Start() {
        rend = GetComponent<Renderer>();
        rend.enabled = true;
        rend.sharedMaterial = materials[playerCount];

    }

    void FixedUpdate()
    {
       
       if( Input.GetKey("w") ){
            rb.AddForce(0, 0, forwardForce * Time.deltaTime);
       }
       if( Input.GetKey("d") ){
            rb.AddForce(sidewaysForce * Time.deltaTime, 0, 0, ForceMode.VelocityChange);
       }
       if( Input.GetKey("a") ){
            rb.AddForce(-1 * sidewaysForce * Time.deltaTime, 0, 0, ForceMode.VelocityChange);
       }
    }
}
