using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    public Rigidbody rb;

    private float forwardForce = 2000f;
    private float sidewaysForce = 150f;

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
