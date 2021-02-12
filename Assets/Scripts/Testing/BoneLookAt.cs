using UnityEngine;

public class BoneLookAt : MonoBehaviour
{
    public Transform headTransform;
    public Transform target;

    private Quaternion _initialRotation;

    private void OnDisable()
    {
        headTransform.localRotation = _initialRotation;
    }

    private void Start()
    {
        _initialRotation = headTransform.localRotation;
    }

    private void LateUpdate()
    {
        headTransform.LookAt(target);
    }
}