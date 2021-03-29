using System;
using Cinemachine;
using UnityEngine;

namespace Player
{
    public class CameraFollowSetter : MonoBehaviour
    {
        [SerializeField]
        private PlayerView player;

        [SerializeField]
        private Transform lemming;

        [SerializeField]
        private float distance;

        [SerializeField]
        private CinemachineVirtualCamera cinemachineVirtualCamera;

        private Transform _targetTransform;

        private void Awake()
        {
            _targetTransform = new GameObject("Follow Target").transform;
            cinemachineVirtualCamera.Follow = _targetTransform;
        }

        private void Update()
        {
            switch (player.PlayerState)
            {
                case PlayerState.ShouldSpawnAtSpawn:
                case PlayerState.ShouldSpawnAtLastPosition:
                case PlayerState.ShouldSpawnCanNotMove:
                case PlayerState.ActiveInMotion:
                case PlayerState.ShouldMakeTurn:
                    _targetTransform.position = lemming.position;
                    break;
                case PlayerState.ActiveAiming:
                case PlayerState.ActivePowerMode:
                    _targetTransform.position = lemming.transform.position + lemming.transform.forward * distance;
                    break;
            }
        }
    }
}