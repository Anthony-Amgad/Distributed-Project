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
            Names.Add(" "+s);
        }else{
            Names = StringsListFromString(PlayerPrefs.GetString("Players"));
            Names[0] = " "+ Names[0];
        }
        PlayerNameTags = GameObject.FindGameObjectsWithTag("PlayerNameTag");
        for(int i = 0; i < Names.Count(); i++){
            PlayerNameTags[i].GetComponentInChildren<Text>().text =  (i+1).ToString() + " " + Names[i];
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
        if(ChatInputField.Text.Length !=0){
            FindObjectOfType<SecondServerSocketScript>().sendChat(ChatInputField.Text);
        }
    }

    List<String> StringsListFromString(String MainString){
		String outString;
		List<string> splitString;
		outString = MainString.Substring(1, MainString.Length -2);
		splitString = outString.Split(","[0]).ToList();
        for( int i = 0; i < splitString.Count(); i++){
            splitString[i] = splitString[i].Substring(1, splitString[i].Length -2);
        }
		return splitString;
	}

}
