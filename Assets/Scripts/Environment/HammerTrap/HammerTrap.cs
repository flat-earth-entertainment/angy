using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HammerTrap : MonoBehaviour
{
    public GameObject blood;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter(Collider other) {
        if(other.tag == "Lemming"){
            GameObject bld = Instantiate(blood,other.transform.position, Quaternion.identity);
            bld.GetComponent<ParticleSystem>().Play();
            Destroy(bld, 8f);
        }
    }
}
