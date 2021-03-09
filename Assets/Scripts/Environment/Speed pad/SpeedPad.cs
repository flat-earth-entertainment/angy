using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedPad : MonoBehaviour
{
    public float speed = 10;
    private void OnCollisionEnter(Collision other) {
        if(other.gameObject.tag == "Lemming"){
            other.gameObject.GetComponent<Rigidbody>().velocity = transform.forward * speed;
        }
    }
}
