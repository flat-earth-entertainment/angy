using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class SplatterThis : MonoBehaviour
{
    public GameObject shroom;
    public ParticleSystem splatter;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            Destroy(shroom);
            splatter.Play(true);
            StartCoroutine(SpawnSchoom());
        }
    }

    IEnumerator SpawnSchoom()
    {
        yield return new WaitForSeconds(3f);
        Instantiate(shroom, transform.position, quaternion.identity);
    }
}
