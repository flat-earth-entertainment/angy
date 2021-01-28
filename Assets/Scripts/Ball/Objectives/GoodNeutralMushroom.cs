using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoodNeutralMushroom : MonoBehaviour
{
    public List<GameObject> fruit;
    private GameObject point;
    private void OnTriggerEnter(Collider other) {
        if(other.tag == "Lemming"){
            if(point != null){
                Destroy(point);
            }else{
                GetComponent<Renderer>().enabled = false;
            }
            point = Instantiate(fruit[other.transform.GetChild(0).GetComponent<Shooter>().playerId], transform.position + new Vector3(0,-1f,0), Quaternion.identity);
        }
    }
}
