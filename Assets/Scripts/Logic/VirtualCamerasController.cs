using System;
using System.Collections.Generic;
using Cinemachine;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class VirtualCamerasController : MonoBehaviour
{
    private const int Inactive = 0;
    private const int Active = 10;

    private HashSet<CinemachineVirtualCamera> _cameras = new HashSet<CinemachineVirtualCamera>();

    private CinemachineBrain _cinemachineBrain;
    private float _defaultTransitionTime;

    public UniTask BlendTo(CinemachineVirtualCamera virtualCamera, float? blendTime = null)
    {
        if (blendTime == null)
        {
            SetActiveCamera(virtualCamera);
            return UniTask.Delay(TimeSpan.FromSeconds(_defaultTransitionTime), DelayType.Realtime);
        }

        SetActiveCamera(virtualCamera, blendTime);
        return UniTask.Delay(TimeSpan.FromSeconds(blendTime.Value), DelayType.Realtime);
    }

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

        _cameras = new HashSet<CinemachineVirtualCamera>(FindObjectsOfType<CinemachineVirtualCamera>());

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