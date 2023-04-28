using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

public class TCPTestClient : MonoBehaviour {  	

	private TcpClient socketConnection; 

	private NetworkStream nwStream;	

	public Transform Player;

	public Transform Opp;
	private String Oppos = "(0.0,11.32,0)";

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
		Thread thread = new Thread(new ThreadStart(Listen));
        thread.Start();
		    
	}  	
	
	//Listening thread for any other players
	void Listen(){
		while(true){
			byte[] bytesToRead = new byte[socketConnection.ReceiveBufferSize];
			int bytesRead = nwStream.Read(bytesToRead, 0, socketConnection.ReceiveBufferSize);
			Debug.Log("Received : " + Encoding.ASCII.GetString(bytesToRead, 0, bytesRead));
			Oppos = Encoding.ASCII.GetString(bytesToRead, 0, bytesRead);
		}	
	}
	
	//sending position every 0.02 second **NEEDS LOWERING MOST RROB
	void FixedUpdate() {
		try{
			if (Time.fixedTime >= timeToGo) {
			byte[] bytesToSend = ASCIIEncoding.ASCII.GetBytes(Player.position.ToString());
			nwStream.Write(bytesToSend, 0, bytesToSend.Length);
			timeToGo = Time.fixedTime + timeOffset;
			}
		}catch(Exception e)
		{
			Debug.Log(e.Message);
		}

		Opp.position = Vector3FromString(Oppos);
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

}

