using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class LobbyScreenScript : MonoBehaviour
{

    public InputField ChatInputField; 
    public Transform Content;
    public GameObject ChatCard;
    GameObject[] PlayerNameTags;
    List<String> Names;

    public Button gameStartBtn;

    // Start is called before the first frame update
    void Start()
    {
        if(PlayerPrefs.GetInt("playerCount") != 0){
            gameStartBtn.interactable = false;
        }else{
            gameStartBtn.interactable = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateNames(String s){
        if(s != "admin"){
            Names.Add(s);
            PlayerPrefs.SetString("Players","["+string.Join(", ", Names.ToArray())+"]");
        }else{
            Names = StringsListFromString(PlayerPrefs.GetString("Players"));
        }
        PlayerNameTags = GameObject.FindGameObjectsWithTag("PlayerNameTag");
        for(int i = 0; i < Names.Count(); i++){
            PlayerNameTags[i].GetComponentInChildren<Text>().text =  (i+1).ToString() + "  " + Names[i];
        }
        for(int i = Names.Count(); i < 4; i++){
            PlayerNameTags[i].GetComponentInChildren<Text>().text = (i+1).ToString();
        }
        
    }

    public void StartGame(){
        //FindObjectOfType<SecondServerSocketScript>().PlayersNames = Names;
        FindObjectOfType<SecondServerSocketScript>().sendStartSignal();
        //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void SendChatMessage(){
        if(ChatInputField.text.Length !=0){
            FindObjectOfType<SecondServerSocketScript>().sendChat(ChatInputField.text);
        }
    }

    public void chatView(string sender, string message){
        GameObject card = Instantiate(ChatCard,Content);
        Text text = card.GetComponentInChildren<Text>();
        Debug.Log("sheesh");
        text.text = sender + ": "+message;
        // text.text = Users.Length + "/4";
        // card.GetComponent<LobbyCardScript>().server = details[0].Substring(1,details[0].Length-2);
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
