using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class spawner2 : MonoBehaviour
{
    public GameObject engeller;
    public float time;


    void Start()
    {
        StartCoroutine(SpawnObject(time));
    }

    public IEnumerator SpawnObject(float time)
    {
        while (true)
        {

            Instantiate(engeller, new Vector3(0, 14f, 0), Quaternion.identity);
            yield return new WaitForSeconds(time);
        }

    }
}
