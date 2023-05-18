using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneInitObj : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject Play;
    public Transform[] Players;
    void Awake()
    {
        Play.SetActive(true);
        FindObjectOfType<SecondServerSocketScript>().onSecondSceeneLoad(Players); 
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
