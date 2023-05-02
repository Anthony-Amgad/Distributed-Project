using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

public class TCPTestClient : MonoBehaviour {  	

	private Vector3[] positions = {new Vector3((float)2.2, (float)1.1, 0),
          new Vector3((float)-2.7, (float)1.1, 0),
          new Vector3((float)-7.8, (float)1.1, 0),
          new Vector3((float)7.4, (float)1.1, 0)};
	private TcpClient socketConnection; 

	private NetworkStream nwStream;

	public PlayerMovement playerScript;

	public Transform MainPlayerTrans;

	public GameObject MainPlayerObject;

	public Transform[] Players;

	private String Positions = "['(0.0,-4.0,0.0)','(0.0,-6.0,0.0)','(0.0,-8.0,0.0)','(0.0,-10.0,0.0)']";

	float timeToGo;
	const float timeOffset = 0.02f;

	const int PORT_NO = 50000;
    const string SERVER_IP = "192.168.56.1"; //replace local host with your device ip	
	
	void Start () {
		timeToGo = Time.fixedTime + timeOffset;
		try {  			
			socketConnection = new TcpClient(SERVER_IP, PORT_NO);
			nwStream = socketConnection.GetStream();		
		} 		
		catch (Exception e) { 			
			Debug.Log("On client connect exception " + e); 		
		}
		byte[] bytesToRead = new byte[socketConnection.ReceiveBufferSize];
		int bytesRead = nwStream.Read(bytesToRead, 0, socketConnection.ReceiveBufferSize);
		//Debug.Log(Encoding.ASCII.GetString(bytesToRead, 0, bytesRead));
		playerScript.playerCount = int.Parse(Encoding.ASCII.GetString(bytesToRead, 0, bytesRead));
		MainPlayerTrans.position = positions[playerScript.playerCount];
		MainPlayerObject.SetActive(true);
		Thread thread = new Thread(new ThreadStart(Listen));
        thread.Start();
		    
	}  	
	
	//Listening thread for any other players
	void Listen(){
		while(true){
			byte[] bytesToRead = new byte[socketConnection.ReceiveBufferSize];
			int bytesRead = nwStream.Read(bytesToRead, 0, socketConnection.ReceiveBufferSize);
			Debug.Log("Received : " + Encoding.ASCII.GetString(bytesToRead, 0, bytesRead));
			Positions = Encoding.ASCII.GetString(bytesToRead, 0, bytesRead);
		}	
	}
	
	//sending position every 0.02 second **NEEDS LOWERING MOST RROB
	void FixedUpdate() {
		try{
			if (Time.fixedTime >= timeToGo) {
			byte[] bytesToSend = ASCIIEncoding.ASCII.GetBytes(MainPlayerTrans.position.ToString());
			nwStream.Write(bytesToSend, 0, bytesToSend.Length);
			timeToGo = Time.fixedTime + timeOffset;
			}
		}catch(Exception e)
		{
			Debug.Log(e.Message);
		}

		String[] tempos = StringsArrayFromString(Positions);
		Debug.Log(tempos.Length);
		for(int i = 0; i < tempos.Length; i += 2){
			if((i/2) != playerScript.playerCount){
				Players[i/2].position = Vector3FromString(tempos[i]);
			}
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

	Vector3 Vector3FromString(String Vector3string) {
		String outString;
		Vector3 outVector;
		String[] splitString = {};
		outString = Vector3string.Substring(1, Vector3string.Length -2);
		splitString = outString.Split(","[0]);
		outVector.x = float.Parse(splitString[0]);
   		outVector.y = float.Parse(splitString[1]);
   		outVector.z = float.Parse(splitString[2]);
   
		return outVector;
	}

	String[] StringsArrayFromString(String MainString){
		String outString;
		String[] splitString = {};
		outString = MainString.Substring(2, MainString.Length -3);
		splitString = outString.Split("','"[0]);
		return splitString;
	}

}

