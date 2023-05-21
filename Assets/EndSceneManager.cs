using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndSceneManager : MonoBehaviour
{

    public GameObject[] PlayerCars;
    public Material[] materials;


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
    }

    // Update is called once per frame
    void Update()
    {

    }
}
