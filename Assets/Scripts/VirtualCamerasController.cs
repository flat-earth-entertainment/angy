using System;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class VirtualCamerasController : MonoBehaviour
{
    private const int Inactive = 0;
    private const int Active = 10;

    private readonly HashSet<CinemachineVirtualCamera> _cameras = new HashSet<CinemachineVirtualCamera>();

    private CinemachineBrain _cinemachineBrain;
    private float _defaultTransitionTime;

    private void Awake()
    {
        _cinemachineBrain = FindObjectOfType<CinemachineBrain>();
        _defaultTransitionTime = _cinemachineBrain.m_DefaultBlend.m_Time;
    }

    private void Start()
    {
        foreach (var virtualCamera in FindObjectsOfType<CinemachineVirtualCamera>())
        {
            _cameras.Add(virtualCamera);
        }
    }

    public void SetActiveCamera(CinemachineVirtualCamera newActiveCamera, float? transitionTime = null)
    {
        if (transitionTime == null)
        {
            _cinemachineBrain.m_DefaultBlend.m_Time = _defaultTransitionTime;
        }
        else
        {
            _cinemachineBrain.m_DefaultBlend.m_Time = transitionTime.Value;
        }

        foreach (var virtualCamera in _cameras)
        {
            if (virtualCamera == newActiveCamera)
            {
                virtualCamera.Priority = Active;
            }
            else
            {
                virtualCamera.Priority = Inactive;
            }
        }
    }
}