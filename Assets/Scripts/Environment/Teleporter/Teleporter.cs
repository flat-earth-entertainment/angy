using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleporter : MonoBehaviour
{
    public Transform teleportTarget;
    [HideInInspector]
    public float teleportCooldown;
    public bool directional;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if(teleportCooldown > 0){
            teleportCooldown -= Time.deltaTime;
        }
    }
    private void OnTriggerEnter(Collider other) {
        if(other.tag == "Lemming" && teleportCooldown <= 0){
            other.transform.position = teleportTarget.position;
            if(directional){
                other.gameObject.GetComponent<Rigidbody>().velocity = teleportTarget.forward * other.gameObject.GetComponent<Rigidbody>().velocity.magnitude;
            }
            teleportTarget.GetComponentInChildren<Teleporter>().teleportCooldown = 0.1f;
        }
    }
}
