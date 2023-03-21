using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Menu : MonoBehaviour
{
    public Text hscoreText;

    void Start()
    {
        hscoreText.text = "" + PlayerPrefs.GetInt("highscore");
    }

    void Update()
    {
        
    }
}
