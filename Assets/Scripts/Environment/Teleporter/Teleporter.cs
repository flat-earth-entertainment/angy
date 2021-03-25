using UnityEngine;
using System.Collections.Generic;
using Audio;

public class Teleporter : MonoBehaviour
{
    public Transform teleportTarget;
    [HideInInspector]
    public float teleportCooldown;
    public bool directional;
    public enum TargetMode{
        single, multiple
    }
    public TargetMode teleportTargetType;
    public List<Transform> multiTeleTarget;
    // Start is called before the first frame update
    void Awake()
    {
        if(teleportTarget == null && multiTeleTarget.Count > 0){
            teleportTarget = multiTeleTarget[0];
        }
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
            AudioManager.PlaySfx(SfxType.TeleporterEngage);
            if(TargetMode.single == teleportTargetType){
                other.transform.position = teleportTarget.GetComponentInChildren<Teleporter>().transform.position;
                if(directional){
                    other.gameObject.GetComponent<Rigidbody>().velocity = teleportTarget.GetComponentInChildren<Teleporter>().transform.forward * other.gameObject.GetComponent<Rigidbody>().velocity.magnitude;
                }
                teleportTarget.GetComponentInChildren<Teleporter>().teleportCooldown = 0.25f;
                teleportTarget.GetComponentInChildren<Animator>().SetTrigger("isTriggered");
                transform.parent.GetComponentInChildren<Animator>().SetTrigger("isTriggered");
            }
            if(TargetMode.multiple == teleportTargetType){
                int selectedTeleporter = Random.Range(0, multiTeleTarget.Count);
                other.transform.position = multiTeleTarget[selectedTeleporter].GetComponentInChildren<Teleporter>().transform.position;
                if(directional){
                    other.gameObject.GetComponent<Rigidbody>().velocity = teleportTarget.GetComponentInChildren<Teleporter>().transform.forward * other.gameObject.GetComponent<Rigidbody>().velocity.magnitude;
                }
                multiTeleTarget[selectedTeleporter].GetComponentInChildren<Teleporter>().teleportCooldown = 0.25f;
                multiTeleTarget[selectedTeleporter].GetComponentInChildren<Animator>().SetTrigger("isTriggered");
                transform.parent.GetComponentInChildren<Animator>().SetTrigger("isTriggered");
            }

        }
    }
}
