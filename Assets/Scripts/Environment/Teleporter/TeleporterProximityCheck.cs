using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleporterProximityCheck : MonoBehaviour
{
    public float range = 4;
    [HideInInspector]
    public bool inRange;
    [HideInInspector]
    public TeleporterProximityCheck linkedTeleporter;
    [HideInInspector]
    public Animator anim;
    public List<PlayerView> players = new List<PlayerView>();
    // Start is called before the first frame update
    void Start()
    {
        players.AddRange(GameObject.FindObjectsOfType<PlayerView>());
        linkedTeleporter = transform.parent.GetComponentInChildren<Teleporter>().teleportTarget.GetComponentInChildren<TeleporterProximityCheck>();
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        inRange = false;
        foreach (PlayerView item in players)
        {
            if(Vector3.Distance(transform.position, item.GetComponentInChildren<BallBehaviour>(true).transform.position) < range){
                inRange = true;
            }
        }
        if(inRange || linkedTeleporter.inRange){
            anim.SetBool("isOpened", true);
        }else{
            anim.SetBool("isOpened", false);
        }
    }
}
