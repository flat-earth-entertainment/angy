using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedPad : MonoBehaviour
{
    public float speed = 10;
    private void OnTriggerEnter(Collider other) {
        if(other.tag == "Lemming"){
            other.GetComponent<Rigidbody>().velocity = transform.forward * speed;
        }
    }
}
