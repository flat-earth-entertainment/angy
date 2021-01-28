using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoodNeutralMushroom : MonoBehaviour
{
    public List<GameObject> fruit;
    private GameObject point;
    public GameObject goal;
    private void OnTriggerEnter(Collider other) {
        if(other.tag == "Lemming"){
            if(point != null){
                Destroy(point);
            }else{
                int layermask = 1 << 15;
                RaycastHit hit;
                if(Physics.Raycast(transform.position + Vector3.up, Vector3.down, out hit, 10f, layermask)){
                    print("HIT");
                    GameObject hitObject = hit.collider.gameObject;
                    Instantiate(goal, hitObject.transform.position, Quaternion.identity);
                    Destroy(hitObject);
                }
                GetComponent<Renderer>().enabled = false;
            }
            point = Instantiate(fruit[other.transform.GetChild(0).GetComponent<Shooter>().playerId], transform.position + new Vector3(0,-1f,0), Quaternion.identity);
        }
    }
}
