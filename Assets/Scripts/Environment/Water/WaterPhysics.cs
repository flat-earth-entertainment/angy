using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterPhysics : MonoBehaviour
{
    public float baseDrag = 0.25f, waterDrag = 2f, bounceThreshold = 5;
    [Tooltip("Should be above 3 or else bounce isn't strong enough")]
    public float waterVerticalBounce = 5;
    public bool iceAbility, bounce;
    public GameObject ice;
    private Abilities.IceBlockOnCollision player;
    private bool delay;
    void OnTriggerEnter(Collider other){    // Must be rewritten if we end up wanting to kill players underwater
        if(other.tag == "Lemming"){
            WaterEvents(other);
        }
    }
    void OnTriggerExit(Collider other){
        if(other.tag == "Lemming"){
            other.attachedRigidbody.drag = baseDrag;
        }
            
    }
    void WaterEvents(Collider other){
        // Find out what to do
            // is player ice?
            if(other.gameObject.TryGetComponent(out Abilities.IceBlockOnCollision iceAbil)){
                player = iceAbil;
                iceAbility = true;
            }

            // is the player fast enough for a bounce?
            if(other.attachedRigidbody.velocity.magnitude > bounceThreshold){
                bounce = true;
            }

        // Do stuff
        if(iceAbility){
            ice.SetActive(true);
            StartCoroutine("DisableIce");
        }else if(bounce){
            Vector3 vel = other.attachedRigidbody.velocity;
            other.attachedRigidbody.velocity = new Vector3(vel.x, waterVerticalBounce, vel.z);
        }else{  // Normal water logic
            other.attachedRigidbody.drag = waterDrag;
        }
        iceAbility = bounce = false;
    }
    IEnumerator DisableIce(){
        Player.PlayerView plView = player.transform.parent.GetComponent<Player.PlayerView>();
        while (true)
        {
            if(plView.Ability.Finished){
                break;
            }
            yield return null;
        }
        ice.SetActive(false);
    }
}
