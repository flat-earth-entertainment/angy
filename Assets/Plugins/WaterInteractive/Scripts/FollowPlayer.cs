using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    [SerializeField]
    Transform target;
    [SerializeField]
    float offsetx;
    [SerializeField]
    float offsetz;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(target.position.x + offsetx, transform.position.y, target.position.z + offsetz);
    }
}
