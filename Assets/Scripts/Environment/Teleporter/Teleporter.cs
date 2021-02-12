using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleporter : MonoBehaviour
{
    private List<Teleporter> teleporters;
    public int teleportToId;
    // Start is called before the first frame update
    void Start()
    {
        teleporters.AddRange(GameObject.FindObjectsOfType<Teleporter>());
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter(Collider other) {
        if(other.tag == "Player"){
            other.transform.position = teleporters[teleportToId].transform.position;
        }
    }
}
