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
    void Awake()
    {
        Play.SetActive(true);
        FindObjectOfType<SecondServerSocketScript>().onSecondSceeneLoad(Players); 
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void updateKeyGUI(string[] keys){
        upkey.text = keys[0];
        rightkey.text = keys[1];
        leftkey.text = keys[2];
        downkey.text = keys[3];
    }
}
