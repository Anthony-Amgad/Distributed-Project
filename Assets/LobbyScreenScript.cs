using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class LobbyScreenScript : MonoBehaviour
{

    GameObject[] PlayerNameTags;
    List<String> Names;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateNames(String s){
        if(s != "admin"){
            Names.Add(s);
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
