using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RingInstancer : MonoBehaviour
{
    [SerializeField] private GameObject m_ringsInstance;
    [SerializeField] private float m_time;
    void Start()
    {
        InvokeRepeating("InstantiateRings", 1, m_time);
    }

    void InstantiateRings()
    {
        GameObject _rings = Instantiate(m_ringsInstance, transform);
    }
}
