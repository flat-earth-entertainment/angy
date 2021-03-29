using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterPhysics : MonoBehaviour
{
    public float baseDrag = 0.25f, waterDrag = 2f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void OnTriggerEnter(Collider other){    // Must be rewritten if we end up wanting to kill players underwater
        if(other.tag == "Lemming"){
            other.attachedRigidbody.drag = waterDrag;
        }
    }
    void OnTriggerExit(Collider other){
        if(other.tag == "Lemming")
            other.attachedRigidbody.drag = baseDrag;
    }
}
