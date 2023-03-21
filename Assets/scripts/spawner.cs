using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class spawner : MonoBehaviour
{

    public GameObject engel1;
    public GameObject engel1_2;
    public GameObject engel1_3;

    public float time;
    public float randomSayii;


    void Start()
    {
        StartCoroutine(SpawnObject(time));
    }

    public IEnumerator SpawnObject(float time)
    {
        while (true)
        {
            randomSayii = Random.Range(0,3);

            //Debug.Log(randomSayii);


            if (randomSayii == 0)
            {
                Instantiate(engel1, new Vector3(0, 10, 0), Quaternion.identity);
                yield return new WaitForSeconds(time);
            }  

            if (randomSayii == 1)
            {
                Instantiate(engel1_2, new Vector3(0, 10, 0), Quaternion.identity);
                yield return new WaitForSeconds(time);
            }

            if (randomSayii == 2)
            {
                Instantiate(engel1_3, new Vector3(0, 10, 0), Quaternion.identity);
                yield return new WaitForSeconds(time);
            }          
            
        }

    }
}
