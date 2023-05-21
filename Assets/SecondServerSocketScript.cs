using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;


public class SecondServerSocketScript : MonoBehaviour {  	


    static SecondServerSocketScript instance;
	public bool ready { get; private set;}
	private Vector3[] positions = {new Vector3((float)2.2, (float)1.5, 0),
          new Vector3((float)-2.7, (float)1.5, 0),
          new Vector3((float)-7.8, (float)1.5, 0),
          new Vector3((float)7.4, (float)1.5, 0)};
	private TcpClient socketConnection;
	
	public List<String> PlayersNames;

	public bool raceStarted = false;
	private bool uN = false;
	private bool sR = false;
	private bool cR = false;
	private bool fR = false;
	private bool gE = false;
	private string nts;
	private string chatSender;
	private string chatMessage;
	private string rankings;
	private int seed;
	private int rank = 0;


	private NetworkStream nwStream;

	public PlayerMovement playerScript;

	public Transform MainPlayerTrans;

	public GameObject MainPlayerObject;

	public Transform[] Players;
	private int playernum;

	private String Positions = "['(0.0,-4.0,0.0)','(0.0,-6.0,0.0)','(0.0,-8.0,0.0)','(0.0,-10.0,0.0)']";

	float timeToGo;
	const float timeOffset = 0.01f;

	private int PORT_NO = 0;
    private string SERVER_IP = "";
	

	void Awake(){
        if(instance != null){
            Destroy(gameObject);
        }else{
            instance = this;
            DontDestroyOnLoad(gameObject);
			SERVER_IP = PlayerPrefs.GetString("Server").Split("#"[0])[0];
			PORT_NO = int.Parse(PlayerPrefs.GetString("Server").Split("#"[0])[1]);
			try {  			
				socketConnection = new TcpClient(SERVER_IP, PORT_NO);
				nwStream = socketConnection.GetStream();		
			} 		
			catch (Exception e) { 			
				Debug.Log("On client connect exception " + e); 		
			}
			byte[] bytesToSend = ASCIIEncoding.ASCII.GetBytes(PlayerPrefs.GetString("MainName"));
			nwStream.Write(bytesToSend, 0, bytesToSend.Length);
			byte[] bytesToRead = new byte[socketConnection.ReceiveBufferSize];
			int bytesRead = nwStream.Read(bytesToRead, 0, socketConnection.ReceiveBufferSize);
			String[] msg = Encoding.ASCII.GetString(bytesToRead, 0, bytesRead).Split("$"[0]);
			PlayerPrefs.SetInt("playerCount",int.Parse(msg[0]));
			playernum = int.Parse(msg[0]);
			PlayerPrefs.SetString("Players", msg[1]);
			Thread thread = new Thread(new ThreadStart(Listen));
			thread.Start();
			ready = true;
        }
    }

	void Start () {
		
		//Debug.Log(Encoding.ASCII.GetString(bytesToRead, 0, bytesRead));
		/*playerScript.playerCount = int.Parse(Encoding.ASCII.GetString(bytesToRead, 0, bytesRead));
		MainPlayerTrans.position = positions[playerScript.playerCount];
		MainPlayerObject.SetActive(true);
		*/
		    
	}  	
	
	//Listening thread for any other players
	void Listen(){
		while(true){
			byte[] bytesToRead = new byte[socketConnection.ReceiveBufferSize];
			int bytesRead = nwStream.Read(bytesToRead, 0, socketConnection.ReceiveBufferSize);
			String[] msg = Encoding.ASCII.GetString(bytesToRead, 0, bytesRead).Split("$"[0]);

			if(msg[0] == "pos" && raceStarted){
				Positions = msg[1];
			}
			else if(msg[0] == "join"){				
				nts = msg[1];
				uN = true;
			}else if(msg[0] == "start"){
				seed = int.Parse(msg[1]);
				sR = true;
			}
			else if(msg[0]=="chat"){
				chatSender = msg[1];
				chatMessage = msg[2];
				cR = true;
			}else if(msg[0]=="rank"){
				rank = int.Parse(msg[1]);
				fR = true;
			}else if(msg[0]=="end"){
				rankings = msg[1];
				gE = true;
			}

		}	
	}
	
	//sending position every 0.01 second **NEEDS LOWERING MOST RROB
	void FixedUpdate() {
		if(raceStarted){
			try{
				if (Time.fixedTime >= timeToGo) {
				byte[] bytesToSend = ASCIIEncoding.ASCII.GetBytes("pos$"+MainPlayerTrans.position.ToString());
				nwStream.Write(bytesToSend, 0, bytesToSend.Length);
				timeToGo = Time.fixedTime + timeOffset;
				}
			}catch(Exception e){
				Debug.Log(e.Message);
			}
			String[] tempos = StringsArrayFromString(Positions);
			FindObjectOfType<GameManagerScript>().updategpoints(tempos, playernum);
			//Debug.Log(Positions);
			//Debug.Log(tempos.Length);
			//Debug.Log(tempos[0]+"||"+tempos[1]+"||"+tempos[2]+"||"+tempos[3]+"||"+tempos[4]+"||"+tempos[5]+"||"+tempos[6]+"||"+tempos[7]);
			for(int i = 0; i < tempos.Length; i+=2){
				if((i/2) != playernum){
					Players[(i/2)].position = Vector3FromString(tempos[i]);
				}
			}
		}
		if(uN){
			FindObjectOfType<LobbyScreenScript>().UpdateNames(nts);
			uN = false;
		}
		if(sR){
			PlayerPrefs.SetInt("seed",seed);
			SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
			sR = false;
		}
		if(cR){
			Debug.Log(chatSender+chatMessage);
			if(raceStarted){
				FindObjectOfType<GameManagerScript>().chatView(chatSender,chatMessage);
			}else{
				FindObjectOfType<LobbyScreenScript>().chatView(chatSender,chatMessage);
			}
			cR = false;
		}
		if(fR){
			FindObjectOfType<GameManagerScript>().recievedRank(rank);
			fR = false;
		}if(gE){
			raceStarted = false;
			PlayerPrefs.SetString("ranking",rankings);
			FindObjectOfType<GameManagerScript>().EndGame();
			gE = false;
			//Destroy(gameObject);
		}
	}

	public void sendChat(String s){
		byte[] bytesToSend = ASCIIEncoding.ASCII.GetBytes("chat$"+s);
		nwStream.Write(bytesToSend, 0, bytesToSend.Length);
	}

	public void onSecondSceeneLoad(Transform[] playertranss){
		
		Players = playertranss;
		MainPlayerObject = GameObject.FindGameObjectWithTag("MainPlayer");
		MainPlayerTrans = MainPlayerObject.GetComponent<Transform>();
		playerScript =  FindObjectOfType<PlayerMovement>();
		MainPlayerTrans.position = positions[PlayerPrefs.GetInt("playerCount")];
		timeToGo = Time.fixedTime;
		raceStarted = true;
		//MainPlayerObject.SetActive(true);
	}
		
	public void sendStartSignal(){
		byte[] bytesToSend = ASCIIEncoding.ASCII.GetBytes("start$");
		nwStream.Write(bytesToSend, 0, bytesToSend.Length);
	}
	
	public void sendFinishSignal(){
		byte[] bytesToSend = ASCIIEncoding.ASCII.GetBytes("finish$");
		nwStream.Write(bytesToSend, 0, bytesToSend.Length);
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

