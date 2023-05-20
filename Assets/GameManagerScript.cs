using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManagerScript : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject Play;
    public Transform[] Players;

    public Text upkey;
    public Text downkey;
    public Text rightkey;
    public Text leftkey;

    public Transform[] road1blockers;
    public Transform[] road2blockers;

    void Awake()
    {
        Play.SetActive(true);
        FindObjectOfType<SecondServerSocketScript>().onSecondSceeneLoad(Players); 
    }

    void Start(){
        System.Random rnd = new System.Random(Guid.NewGuid().GetHashCode());
        for(int i=0; i<road1blockers.Length; i++){
            road1blockers[i].position = new Vector3(rnd.Next(-5,6),0.5f,(200*(i+1)));
        }
        for(int i=0; i<road2blockers.Length; i++){
            road2blockers[i].position = new Vector3(rnd.Next(-5,6),0.5f,1000+(200*(i+1)));
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void updateblockers(bool rflag, int diststore){
        diststore += 900;
        System.Random rnd = new System.Random(Guid.NewGuid().GetHashCode());
        if(rflag){
            for(int i=0; i<road1blockers.Length; i++){
            road1blockers[i].position = new Vector3(rnd.Next(-5,6),0.5f,diststore+(200*(i+1)));
            }
            //road1.localPosition = new Vector3(road1.localPosition.x, road1.localPosition.y, road1.localPosition.z + 2000);
          }else{
            for(int i=0; i<road2blockers.Length; i++){
            road2blockers[i].position = new Vector3(rnd.Next(-5,6),0.5f,diststore+(200*(i+1)));
            }
            //road2.localPosition = new Vector3(road2.localPosition.x, road2.localPosition.y, road2.localPosition.z + 2000);
          }
    }

    public void updateKeyGUI(string[] keys){
        upkey.text = keys[0];
        rightkey.text = keys[1];
        leftkey.text = keys[2];
        downkey.text = keys[3];
    }
}
