using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class StartMenu : MonoBehaviour
{
    public InputField NameInputField; 
    public GameObject ErrorText;
    public GameObject CreditsCanvas;

 //replace local host with your device ip

    void Start()
    {	
		//Thread thread = new Thread(new ThreadStart(Listen));
        //thread.Start();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CreditsButton(){
        CreditsCanvas.SetActive(true);
    } 

    public void CloseCredits(){
        CreditsCanvas.SetActive(false);
    }

    public void StartGame(){
        if(NameInputField.text.Length == 0){
            ErrorText.GetComponent<Text>().text = "Please Input Name";
            ErrorText.SetActive(true);
            return;
        }
    FindObjectOfType<FirstServerSocketScript>().StartSearch();

    }
}