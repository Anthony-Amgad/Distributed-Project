using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class EndSceneManager : MonoBehaviour
{

    public GameObject[] PlayerCars;
    public Material[] materials;
    public TextMesh[] playerNameTags;
    public Text chatText;
    public InputField ChatInputField;


    // Start is called before the first frame update
    void Start()
    {
        string ranking = PlayerPrefs.GetString("ranking");
        int c = 3;
        for(int i=0; i < ranking.Length; i++){
            if(int.Parse(ranking[i].ToString())!=0){
                PlayerCars[int.Parse(ranking[i].ToString())-1].GetComponent<Renderer>().sharedMaterial = materials[i];
            }else{
                PlayerCars[c--].SetActive(false);
            }
        }
        List<String> Names = StringsListFromString(PlayerPrefs.GetString("Players"));
        for(int i = 0; i < Names.Count(); i++){
            playerNameTags[i].text = Names[i];
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void chatView(string chatSender, string chatMessage){
        chatText.text = chatText.text + "\n" + chatSender + ": " + chatMessage;
    }

    public void SendChatMessage(){
        if(ChatInputField.text.Length !=0){
            FindObjectOfType<SecondServerSocketScript>().sendChat(ChatInputField.text);
            ChatInputField.text = "";
        }
    }

    public void leaveGame(){
        Destroy(FindObjectOfType<SecondServerSocketScript>());
        Destroy(FindObjectOfType<FirstServerSocketScript>());
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 2);
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
