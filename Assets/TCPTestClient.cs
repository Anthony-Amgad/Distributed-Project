using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

public class TCPTestClient : MonoBehaviour {  	

	private TcpClient socketConnection; 

	private NetworkStream nwStream;	

	public Transform Player;

	float timeToGo;
	const float timeOffset = 0.1f;

	const int PORT_NO = 50000;
    const string SERVER_IP = "localhost"; //replace local host with your device ip	
	
	void Start () {
		timeToGo = Time.fixedTime + timeOffset;
		try {  			
			socketConnection = new TcpClient(SERVER_IP, PORT_NO);
			nwStream = socketConnection.GetStream();		
		} 		
		catch (Exception e) { 			
			Debug.Log("On client connect exception " + e); 		
		}
		    
	}  	
	// Update is called once per frame
	void FixedUpdate() {
		if (Time.fixedTime >= timeToGo) {
			byte[] bytesToSend = ASCIIEncoding.ASCII.GetBytes(Player.position.ToString());
			nwStream.Write(bytesToSend, 0, bytesToSend.Length);
			byte[] bytesToRead = new byte[socketConnection.ReceiveBufferSize];
			int bytesRead = nwStream.Read(bytesToRead, 0, socketConnection.ReceiveBufferSize);
			Debug.Log("Received : " + Encoding.ASCII.GetString(bytesToRead, 0, bytesRead));
			timeToGo = Time.fixedTime + timeOffset;
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
	
}