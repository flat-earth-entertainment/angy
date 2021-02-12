using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleporter : MonoBehaviour
{
    public List<Teleporter> teleporters;
    public int teleportToId;
    [HideInInspector]
    public float teleportCooldown;
    public bool directional;
    // Start is called before the first frame update
    void Start()
    {
        teleporters.AddRange(GameObject.FindObjectsOfType<Teleporter>());
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
            other.transform.position = teleporters[teleportToId].transform.position;
            if(directional){
                other.gameObject.GetComponent<Rigidbody>().velocity = teleporters[teleportToId].transform.forward * other.gameObject.GetComponent<Rigidbody>().velocity.magnitude;
            }
            teleporters[teleportToId].teleportCooldown = 0.1f;
        }
    }
}
