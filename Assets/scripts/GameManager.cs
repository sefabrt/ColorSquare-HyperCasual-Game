using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public int score;
    public Text ScoreText;

    public GameObject spawner1;
    public GameObject spawner2;
    public GameObject spawner3;


    public AudioSource muzik;

    void Start()
    {
        
        //Application.targetFrameRate = 60;
        Time.timeScale = 1;
        score = 0;
        ScoreText.text = score.ToString();
        
        
    }

    void Update()
    {
        if(score == 10)
        {
            spawner1.SetActive(false);
            spawner2.SetActive(true);
        }

        if (score == 29)
        {
            spawner2.SetActive(false);
            spawner3.SetActive(true);
        }
    }

    public void UpdateScore()
    {

        score++;
        ScoreText.text = score.ToString();
    }

    public void RestartGame()
    {
        if(score > PlayerPrefs.GetInt("highscore"))
        {
            PlayerPrefs.SetInt("highscore", score);
        }

        SceneManager.LoadScene(1);
    }

    public void MenuButton()
    {
        if (score > PlayerPrefs.GetInt("highscore"))
        {
            PlayerPrefs.SetInt("highscore", score);
        }

        SceneManager.LoadScene(0);  
    }

}
