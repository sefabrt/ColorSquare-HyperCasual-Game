using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class spawner3 : MonoBehaviour
{
    public GameObject engeller2;
    public float time;

    void Start()
    {
        StartCoroutine(SpawnObject(time));
    }

    void Update()
    {
        
    }

    public IEnumerator SpawnObject(float time)
    {
        while (true)
        {
            Instantiate(engeller2, new Vector3(0, 30f, 0), Quaternion.identity);

            yield return new WaitForSeconds(time);
        }
    }
}
