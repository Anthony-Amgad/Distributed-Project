using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyCardScript : MonoBehaviour
{
    public String server;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnJoin(){
       FindObjectOfType<FirstServerSocketScript>().JoinLobby(server);
    }
}
