using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MatchmakingScript : MonoBehaviour
{

    public Transform Content;
    public GameObject LobbyCard;
    // Start is called before the first frame update
    void Start()
    {
        String LobbysString = PlayerPrefs.GetString("Lobbys");
        String[] Lobbys = StringsArrayFromString(LobbysString);
        for(int i = 1; i < Lobbys.Length - 1; i++){
            GameObject card = Instantiate(LobbyCard,Content);
            Text[] texts = card.GetComponentsInChildren<Text>();
            String tempString = Lobbys[i].Substring(2, Lobbys[i].Length - 2);
            String[] details = tempString.Split(":"[0]);
            String[] Users = UsersArrayFromString(details[1]);
            if(Users.Length == 4){
                Button btn = card.GetComponentInChildren<Button>();
                btn.interactable = false;
            }
            texts[0].text = Users[0] + "'s Lobby";
            texts[1].text = Users.Length + "/4";
            card.GetComponent<LobbyCardScript>().server = details[0].Substring(1,details[0].Length-2);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    String[] StringsArrayFromString(String MainString){
		String outString;
		String[] splitString = {};
		outString = MainString.Substring(1, MainString.Length -2);
		splitString = outString.Split("]"[0]);
		return splitString;
	}

    String[] UsersArrayFromString(String MainString){
		String outString;
		String[] splitString = {};
		outString = MainString.Substring(2, MainString.Length -2);
		splitString = outString.Split(","[0]);
        splitString[0] = "'" + splitString[0];
        for(int i = 0; i < splitString.Length; i++){
            splitString[i] = splitString[i].Substring(2, splitString[i].Length - 3);
        }
		return splitString;
	}
}
