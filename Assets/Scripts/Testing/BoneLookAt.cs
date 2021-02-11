using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoneLookAt : MonoBehaviour
{
    public Transform headTransform;
    public Transform target;

    private Quaternion _initialRotation;
    
    void Start()
    {
        _initialRotation = headTransform.localRotation;
    }
    
    void LateUpdate()
    {
        Quaternion lookRotation = Quaternion.LookRotation(target.position - headTransform.position);
        headTransform.localRotation = lookRotation * _initialRotation;
    }
}
