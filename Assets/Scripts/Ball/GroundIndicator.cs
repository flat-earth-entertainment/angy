using UnityEngine;

public class GroundIndicator : MonoBehaviour
{
    public GameObject hitIndicator;
    [HideInInspector]
    public GameObject spawnedIndicator;
    private int timer;
    public bool isPlayer;
    void OnCollisionEnter(Collision collision){
        
        if(timer == 1 && !isPlayer){
            ContactPoint contact = collision.contacts[0];
            spawnedIndicator = Instantiate(hitIndicator, transform.position, Quaternion.identity);
            Vector3 direction = contact.point - transform.position;
            RaycastHit hit;            
            if(Physics.Raycast(transform.position, Vector3.down, out hit, 10, LayerMask.GetMask("IgnoredMap"))){
                spawnedIndicator.transform.up = hit.normal;
                spawnedIndicator.transform.position = new Vector3(0,0.01f,0) + hit.point;
            }
            //spawnedIndicator.transform.rotation = Quaternion.LookRotation(direction, transform.up);
            print("Done");
            
        }
        timer++;
    }
}
