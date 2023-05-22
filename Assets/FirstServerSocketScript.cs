using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class FirstServerSocketScript : MonoBehaviour
{
    // Start is called before the first frame update
    static FirstServerSocketScript instance;
    public InputField NameInputField; 
    public GameObject ErrorText;
    public GameObject SSS;
    private TcpClient socketConnection; 
	private NetworkStream nwStream;
    
    public GameObject MatchMakingCanvas;
    public GameObject LobbyCanvas;

    const int PORT_NO = 50001;
    const string SERVER_IP = "192.168.56.1";
    void Awake(){
        if(instance != null){
            Destroy(gameObject);
        }else{
            instance = this;
            DontDestroyOnLoad(gameObject);
            PlayerPrefs.SetString("startpos","['(2.2,1.5,0.0)','(-2.7,1.5,0.0)','(-7.8,1.5,0.0)','(7.4,1.5,0.0)']");
        }
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartSearch(){

    try {  			
			socketConnection = new TcpClient(SERVER_IP, PORT_NO);
			nwStream = socketConnection.GetStream();		
		} 		
		catch (Exception e) { 			
			Debug.Log("On client connect exception " + e); 		
		}
        byte[] bytesToSend = ASCIIEncoding.ASCII.GetBytes("user");
		nwStream.Write(bytesToSend, 0, bytesToSend.Length);
        byte[] bytesToRead = new byte[socketConnection.ReceiveBufferSize];
		int bytesRead = nwStream.Read(bytesToRead, 0, socketConnection.ReceiveBufferSize);
        bytesToSend = ASCIIEncoding.ASCII.GetBytes(NameInputField.text);
		nwStream.Write(bytesToSend, 0, bytesToSend.Length);
		bytesToRead = new byte[socketConnection.ReceiveBufferSize];
		bytesRead = nwStream.Read(bytesToRead, 0, socketConnection.ReceiveBufferSize);
        String[] splitstring = {};
        splitstring = Encoding.ASCII.GetString(bytesToRead, 0, bytesRead).Split("$"[0]);
		//Debug.Log(Encoding.ASCII.GetString(bytesToRead, 0, bytesRead));
		if(splitstring[0] == "ready"){
            ErrorText.SetActive(false);
            PlayerPrefs.SetString("MainName",NameInputField.text);
            PlayerPrefs.SetString("Lobbys", splitstring[1]);
            MatchMakingCanvas.SetActive(true);
        }
        else if(splitstring[0] == "ingame"){
            PlayerPrefs.SetString("Server",splitstring[1]);
            PlayerPrefs.SetString("startpos",splitstring[2]);
            SSS.SetActive(true);
        }
        else if(Encoding.ASCII.GetString(bytesToRead, 0, bytesRead) == "AU"){
            ErrorText.GetComponent<Text>().text = "Name Already in Use";
            ErrorText.SetActive(true);
            socketConnection.Close();
        }
    }

    private void OnApplicationQuit() {
		try
		{
			socketConnection.Close();
		}
		catch(Exception e)
		{
			Debug.Log(e.Message);
		}
	}

    public void CreateLobby(){
        byte[] bytesToSend = ASCIIEncoding.ASCII.GetBytes("new$");
		nwStream.Write(bytesToSend, 0, bytesToSend.Length);
        byte[] bytesToRead = new byte[socketConnection.ReceiveBufferSize];
		int bytesRead = nwStream.Read(bytesToRead, 0, socketConnection.ReceiveBufferSize);
        PlayerPrefs.SetString("Server",Encoding.ASCII.GetString(bytesToRead, 0, bytesRead));
        SSS.SetActive(true);
        MatchMakingCanvas.SetActive(false);
        LobbyCanvas.SetActive(true);
        FindObjectOfType<LobbyScreenScript>().UpdateNames("admin");
    }

    public void JoinLobby(String s){
        //Debug.Log(s);
        byte[] bytesToSend = ASCIIEncoding.ASCII.GetBytes("join$"+s);
		nwStream.Write(bytesToSend, 0, bytesToSend.Length);
        byte[] bytesToRead = new byte[socketConnection.ReceiveBufferSize];
		int bytesRead = nwStream.Read(bytesToRead, 0, socketConnection.ReceiveBufferSize);
        if(Encoding.ASCII.GetString(bytesToRead, 0, bytesRead) == "ok"){
            PlayerPrefs.SetString("Server",s);
            SSS.SetActive(true);
            MatchMakingCanvas.SetActive(false);
            LobbyCanvas.SetActive(true);
            FindObjectOfType<LobbyScreenScript>().UpdateNames("admin");
        }
        
    }
}
