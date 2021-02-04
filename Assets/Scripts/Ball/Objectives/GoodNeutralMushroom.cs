using System;
using System.Collections;
using System.Collections.Generic;
using Config;
using UnityEngine;

public class GoodNeutralMushroom : MonoBehaviour
{
    public static event Action<GameObject> BecameHole;
    
    public List<GameObject> fruit;
    private GameObject point;
    public GameObject goal;
    private PointController pointController;
    public ParticleSystem splatter;
    // Owner Id 99 refers to no ownership, shouldn't be a problem unless we want 100 players.
    public int ownerId = 99, pointValue = 1;
    private void Start() {
        pointController = GameObject.FindObjectOfType<PointController>();
    }
    private void OnTriggerEnter(Collider other) {
        Trigger(other);
    }
    private void OnTriggerExit(Collider other) {
        Trigger(other);
    }
    void Trigger(Collider other){
        if(other.tag == "Lemming"){
            int hitId = other.transform.GetChild(0).GetComponent<Shooter>().playerId;
            ownerId = hitId;
            if(point != null){
                Destroy(point);
            }else{
                pointController.EnemyHit(hitId);
                GetComponent<Renderer>().enabled = false;
                splatter.Play(true);
            }
            point = Instantiate(fruit[hitId], transform.position + new Vector3(0,1f,0), Quaternion.identity);
            pointController.UpdateScore();
        }

    }
    public void SpawnGoal(){
        pointValue++;
        RaycastHit hit;
        if(Physics.Raycast(transform.position + Vector3.up, Vector3.down, out hit, 10f, LayerMask.GetMask("IgnoredMap"))){
            GameObject hitObject = hit.collider.gameObject;
            print($"HIT {hitObject.name} {LayerMask.LayerToName(hitObject.layer)}");
            Instantiate(goal, hitObject.transform.position, Quaternion.identity);
            BecameHole?.Invoke(hitObject);
            Destroy(hitObject);
        }
        GetComponent<Renderer>().enabled = false;
        point = Instantiate(fruit[2], transform.position + new Vector3(0,1,0), Quaternion.identity);
    }
}
