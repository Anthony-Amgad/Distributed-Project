using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class GameManagerScript : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject Play;
    public Transform[] Players;

    public Transform[] playergpoints;

    public Text upkey;
    public Text downkey;
    public Text rightkey;
    public Text leftkey;

    public Text chatText;

    public InputField ChatInputField;
    
    private System.Random rnd;

    public bool chatopen = false;

    public Transform[] road1blockers;
    public Transform[] road2blockers;
    public Animator ChatPanelAnimator;
    public TextMesh[] playertags;
    public GameObject finishPanel;
    public GameObject keysPanel;
    public Text rankText;


    void Awake()
    {
        Play.SetActive(true);
        FindObjectOfType<SecondServerSocketScript>().onSecondSceeneLoad(Players);
        List<String> Names = StringsListFromString(PlayerPrefs.GetString("Players"));
        for(int i = 0; i < Names.Count(); i++){
            playertags[i].text = Names[i];
        }

    }

    void Start(){
        rnd = new System.Random(PlayerPrefs.GetInt("seed"));
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
        if(rflag){
            for(int i=0; i<road1blockers.Length; i++){
            road1blockers[i].position = new Vector3(rnd.Next(-5,6),0.5f,diststore+(200*(i+1)));
            }
          }else{
            for(int i=0; i<road2blockers.Length; i++){
            road2blockers[i].position = new Vector3(rnd.Next(-5,6),0.5f,diststore+(200*(i+1)));
            }
          }
    }

    public void updategpoints(string[] poss, int playernum){
        for(int i = 0; i < poss.Length; i+=2){
				playergpoints[i/2].transform.localPosition = new Vector3(((675*Vector3FromString(poss[i]).z)/6000)-335,(18-(6*i)),0);
			}
    }

    public void chatView(string chatSender, string chatMessage){
        chatText.text = chatText.text + "\n" + chatSender + ": " + chatMessage;
    }

    public void chatPanelBtn(){
        chatopen = !chatopen;
        ChatPanelAnimator.SetBool("isOpen",chatopen);        
    }

    public void SendChatMessage(){
        if(ChatInputField.text.Length !=0){
            FindObjectOfType<SecondServerSocketScript>().sendChat(ChatInputField.text);
            ChatInputField.text = "";
        }
    }

    public void updateKeyGUI(string[] keys){
        upkey.text = keys[0];
        rightkey.text = keys[1];
        leftkey.text = keys[2];
        downkey.text = keys[3];
    }

    public void playerFinished(){
        FindObjectOfType<SecondServerSocketScript>().sendFinishSignal();
    }

    public void recievedRank(int rank){
        rankText.text = rank.ToString();
        keysPanel.SetActive(false);
        finishPanel.SetActive(true);
        if(!chatopen){
            chatPanelBtn();
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

    List<String> StringsListFromString(String MainString){
		String outString;
		List<string> splitString;
		outString = MainString.Substring(1, MainString.Length -2);
		splitString = outString.Split(","[0]).ToList();
        for( int i = 0; i < splitString.Count(); i++){
            splitString[i] = splitString[i].Replace(" ","");
            splitString[i] = splitString[i].Replace("'","");
        }
		return splitString;
	}


}
