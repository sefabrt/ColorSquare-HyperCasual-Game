using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class engel : MonoBehaviour
{
    public float speed;

    public GameObject yellowParticle;
    public GameObject yellowParticle2;
    public GameObject blueParticle;
    public GameObject blueParticle2;
    public GameObject purpleParticle;
    public GameObject purpleParticle2;

    void Start()
    {

    }

    void Update()
    {

    }
    void FixedUpdate()
    {
        transform.position += Vector3.down * speed * Time.deltaTime;
    }


    void OnTriggerEnter2D(Collider2D col)
    {

        if (col.gameObject.name == "Player")
        {
            yellowParticle.SetActive(true);
            blueParticle.SetActive(true);
            purpleParticle.SetActive(true);
            Destroy(gameObject, 1);
            yellowParticle2.SetActive(true);
            blueParticle2.SetActive(true);
            purpleParticle2.SetActive(true);
            Destroy(gameObject, 1);

        }

    }

}

