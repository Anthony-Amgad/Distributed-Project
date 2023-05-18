using UnityEngine;
using System;



public class PlayerMovement : MonoBehaviour
{

     public string[] keys = {"`","`","`","`"};
     public Rigidbody rb;

     public string[] btnins = {"a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z"};
     public Transform trans;
     public Transform road1;
     public Transform road2;
     public Material[] materials;
     Renderer rend;
     private float forwardForce = 2000f;
     private float sidewaysForce = 100f;

     private int diststore = 1010;
     private bool rflag = true;

     const float timeOffset = 5;
     float timeToGo;

     private void Start() {
          rend = GetComponent<Renderer>();
          rend.enabled = true;
          rend.sharedMaterial = materials[PlayerPrefs.GetInt("playerCount")];
          timeToGo = Time.fixedTime;
     }

     void FixedUpdate()
     {       
     if( Input.GetKey(keys[0]) && rb.velocity.z < 75){
          rb.AddForce(0, 0, forwardForce * Time.deltaTime);
     }
     if( Input.GetKey(keys[1]) ){
          rb.AddForce(sidewaysForce * Time.deltaTime, 0, 0, ForceMode.VelocityChange);
     }
     if( Input.GetKey(keys[2]) ){
          rb.AddForce(-1 * sidewaysForce * Time.deltaTime, 0, 0, ForceMode.VelocityChange);
     }
     if( Input.GetKey(keys[3]) && rb.velocity.z > -25){
          rb.AddForce(0, 0, -1 * forwardForce * Time.deltaTime);
     }
     if( (Input.GetKey(keys[1]) ||  Input.GetKey(keys[2])) && (rb.velocity.z > 0) ){
          rb.AddForce(0, 0, -1 * 100 * Time.deltaTime);
     }
     if(trans.position.z > diststore){
          if(rflag){
               road1.localPosition = new Vector3(road1.localPosition.x, road1.localPosition.y, road1.localPosition.z + 2000);
          }else{
               road2.localPosition = new Vector3(road2.localPosition.x, road2.localPosition.y, road2.localPosition.z + 2000);
          }
          diststore+=1000;
          rflag = !rflag;
     }
     if (Time.fixedTime >= timeToGo) {
          System.Random rnd = new System.Random(Guid.NewGuid().GetHashCode());
          for(int i = 0; i < 4; i++){
               int rndint = rnd.Next(0,26-i);
               keys[i] = btnins[rndint];
               string temp = btnins[25-i];
               btnins[25-i] = btnins[rndint];
               btnins[rndint] = temp;
          }
          FindObjectOfType<GameManagerScript>().updateKeyGUI(keys);
          timeToGo = Time.fixedTime + timeOffset;
     }


     }
}
