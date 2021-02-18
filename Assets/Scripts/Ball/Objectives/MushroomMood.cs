using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MushroomMood : MonoBehaviour
{
    private int playerInTriggerCount;
    private Animator anim;
    private Material mat;
    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter(Collider other) {
        if(other.tag == "Lemming"){
            playerInTriggerCount++;
            UpdateMood();
        }
    }
    private void OnTriggerExit(Collider other) {
        if(other.tag == "Lemming"){
            playerInTriggerCount--;
            UpdateMood();
        }
        
    }
    void UpdateMood(){
        if(playerInTriggerCount > 0){
            anim.SetBool("isTerrifyed", true);
            transform.GetChild(1).GetComponent<SkinnedMeshRenderer>().materials[0].SetFloat("MushroomFace", 1);
        }else{
            anim.SetBool("isTerrifyed", false);
            transform.GetChild(1).GetComponent<SkinnedMeshRenderer>().materials[0].SetFloat("MushroomFace", 0);
        }
    }
}
