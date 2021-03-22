using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikeTrap : MonoBehaviour
{
    public List<GameObject> players = new List<GameObject>();
    public float triggerDelay = 1, timedInterval = 8, knockbackForce = 10;
    private float timedTimer, enumTimer;
    public enum TriggerMethod
    {
       trigger, timed 
    }
    public TriggerMethod triggerType;
    private SkinnedMeshRenderer spike;
    private bool enumActive;
    public GameObject blood;
    // Start is called before the first frame update
    void Start()
    {
        spike = GetComponentInChildren<SkinnedMeshRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if(triggerType == TriggerMethod.trigger){
            // Triggers when someone is inside the trigger
            if(!enumActive && players.Count > 0){
                StartCoroutine("Activate");
            }
        }
        if(triggerType == TriggerMethod.timed){
            // Trigger after x amount of time
            timedTimer += Time.deltaTime;
            if(timedTimer > timedInterval && !enumActive){
                StartCoroutine("Activate");
                timedTimer = 0;
            }
        }
    }
    IEnumerator Activate(){ // Handles the blendshape animation of the spikes
        enumActive = true;
        while (true)
        {   // Extends the spike slightly to show that they have been TRIGGURRRED
            enumTimer += Time.deltaTime * 10;
            spike.SetBlendShapeWeight(0, Mathf.Lerp(100, 75, enumTimer));
            if(enumTimer >= 1){
                enumTimer = 0;
                break;
            }
            yield return null;
        }
        yield return new WaitForSeconds(triggerDelay);  // Waits a while
        while (true)
        {   // Retracts the spikes to prepare them for maximum extension
            enumTimer += Time.deltaTime * 10;
            spike.SetBlendShapeWeight(0, Mathf.Lerp(75, 100, enumTimer));
            if(enumTimer >= 1){
                enumTimer = 0;
                break;
            }
            yield return null;
        }
        while (true)
        {   // Extends the spikes to the max
            enumTimer += Time.deltaTime * 10;
            spike.SetBlendShapeWeight(0, Mathf.Lerp(100, 0, enumTimer));
            if(enumTimer >= 1){
                // Applies damage to players
                foreach (GameObject item in players)
                {
                    Hit(item);
                }
                // Apply full extention sound

                enumTimer = 0;
                break;
            }
            yield return null;
        }
        yield return new WaitForSeconds(0.1f);
        while (true)
        {   // Slowly retract the spikes
            enumTimer += Time.deltaTime * 1;
            spike.SetBlendShapeWeight(0, Mathf.Lerp(0, 100, enumTimer));
            if(enumTimer >= 1){
                enumTimer = 0;
                break;
            }
            yield return null;
        }
        enumActive = false;
    }
    private void OnTriggerEnter(Collider other) {
        if(other.tag == "Lemming"){
            players.Add(other.gameObject);
        }
    }
    private void OnTriggerExit(Collider other) {
        if(other.tag == "Lemming"){
            players.Remove(other.gameObject);
        }
    }
    void Hit(GameObject lemming){
        // applies Knockback
        lemming.GetComponent<Rigidbody>().velocity = 
            lemming.GetComponent<Rigidbody>().velocity.normalized * knockbackForce;
        // Applies blood
        Instantiate(blood, lemming.transform.position, Quaternion.identity);
        // Apply sound

        // Apply Angy
        
    }
}
