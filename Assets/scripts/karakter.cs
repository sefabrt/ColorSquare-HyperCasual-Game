using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class karakter : MonoBehaviour
{
    public GameManager managerGame;
    public GameObject DeathScreen;

    float dirX;

    public float duvarGuc;
    float moveSpeed = 20f;

    public float jumpPower;
    private Rigidbody2D rb2d;

    public GameObject leftWall;
    public GameObject rightWall;

    private float mySpeedX;             //pc'de test için yazdm
    private float myTestSpeed = 5;      //pc'de test için yazdm

    public int randomSayi;

    public AudioSource ses1;

    Color32 blueColor = new Color32(0, 27, 255, 255);
    Color32 purpleColor = new Color32(255, 61, 237, 255);
    Color32 yellowColor = new Color32(138, 138, 138, 255);

    Color32 blueArkaPlan = new Color32(0, 14, 39, 0);
    Color32 purpleArkaPlan = new Color32(48, 0, 38, 0);
    Color32 yellowArkaPlan = new Color32(60, 60, 60, 0);

    SpriteRenderer playerSprite;
    SpriteRenderer sagDuvarSprite;
    SpriteRenderer solDuvarSprite;

    public Text scoreTxt;
    public AudioSource gameOverVoice;
    public AudioSource level2Voice;
    public AudioSource level3Voice;



    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
        Time.timeScale = 1;

        playerSprite = gameObject.GetComponent<SpriteRenderer>();
        sagDuvarSprite = rightWall.GetComponent<SpriteRenderer>();
        solDuvarSprite = leftWall.GetComponent<SpriteRenderer>();

    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            rb2d.velocity = Vector2.up * jumpPower;
        }

        //dirX = Input.acceleration.x * moveSpeed;
        //transform.position = new Vector2(Mathf.Clamp(transform.position.x, -7.5f, 7.5f), transform.position.y);

        mySpeedX = Input.GetAxis("Horizontal");
        rb2d.velocity = new Vector2(mySpeedX * myTestSpeed, rb2d.velocity.y);

        if (managerGame.score == 9)
        {
            level2Voice.Play();
        }

        if (managerGame.score == 29)
        {
            level3Voice.Play();
        }

    }

    //void FixedUpdate()
    //{
    //    rb2d.velocity = new Vector2(dirX, rb2d.velocity.y);
    //}


    private void OnTriggerEnter2D(Collider2D col1)
    {

        if (col1.gameObject.tag == gameObject.tag)
        {
            randomSayi = Random.Range(0, 10);
            renkDegis();
            managerGame.UpdateScore();
            ses1.Play();
            
        }

        else if (col1.gameObject.tag == "forDestroy")
        {
            col1.gameObject.GetComponent<engel>().speed = 15;
            Debug.Log("Engel Destroy Ediliyor.");
        }

        else
        {
            Destroy(scoreTxt);
            managerGame.muzik.volume = 0.3f;
            Debug.Log("Dead");
            gameOverVoice.Play();
            Time.timeScale = 0;
            DeathScreen.SetActive(true);
        }
    }


    void renkDegis()
    {
        if (gameObject.tag == "yellow")
        {
            if (randomSayi > 5)
            {
                transform.gameObject.tag = "blue";
                playerSprite.color = blueColor;
                solDuvarSprite.color = blueColor;
                sagDuvarSprite.color = blueColor;
                Camera.main.backgroundColor = blueArkaPlan;
            }
            else
            {
                transform.gameObject.tag = "purple";
                playerSprite.color = purpleColor;
                solDuvarSprite.color = purpleColor;
                sagDuvarSprite.color = purpleColor;
                Camera.main.backgroundColor = purpleArkaPlan;
            }
        }

        else if (gameObject.tag == "blue")
        {
            if (randomSayi > 5)
            {
                transform.gameObject.tag = "purple";
                playerSprite.color = purpleColor;
                solDuvarSprite.color = purpleColor;
                sagDuvarSprite.color = purpleColor;
                Camera.main.backgroundColor = purpleArkaPlan;
            }

            else
            {
                transform.gameObject.tag = "yellow";
                playerSprite.color = yellowColor;
                solDuvarSprite.color = yellowColor;
                sagDuvarSprite.color = yellowColor;
                Camera.main.backgroundColor = yellowArkaPlan;
            }

        }

        else if (gameObject.tag == "purple")
        {
            if (randomSayi > 5)
            {
                transform.gameObject.tag = "yellow";
                playerSprite.color = yellowColor;
                solDuvarSprite.color = yellowColor;
                sagDuvarSprite.color = yellowColor;
                Camera.main.backgroundColor = yellowArkaPlan;
            }
            else
            {
                transform.gameObject.tag = "blue";
                playerSprite.color = blueColor;
                solDuvarSprite.color = blueColor;
                sagDuvarSprite.color = blueColor;
                Camera.main.backgroundColor = blueArkaPlan;
            }
        }
    }
}
