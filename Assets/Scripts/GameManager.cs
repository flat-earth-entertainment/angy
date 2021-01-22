using System;
using Cinemachine;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private float levelOverviewTime = 2f;

    [SerializeField]
    private CinemachineVirtualCamera levelOverviewCamera;

    [SerializeField]
    private CinemachineVirtualCamera spawnPointCamera;

    [SerializeField]
    private Transform spawnPoint;

    [SerializeField]
    private float jumpInTime = 2f;


    private PlayersManager _playersManager;
    private PlayerView _currentTurnPlayerView;
    private VirtualCamerasController _camerasController;

    private void Awake()
    {
        if (spawnPoint == null || spawnPoint.Equals(null))
        {
            spawnPoint = GameObject.FindWithTag("Spawn Point").transform;

            if (!spawnPoint)
                Debug.LogError("Can't find spawn point! Make sure it has the tag \"Spawn Point\"!");
        }

        _playersManager = FindObjectOfType<PlayersManager>();
        if (!_playersManager)
            Debug.LogError("Can't find Players Manager! Make sure it is in scene or is not spawned after frame 1!");

        _camerasController = FindObjectOfType<VirtualCamerasController>();
        if (!_camerasController)
            Debug.LogError(
                "Can't find Virtual Cameras Controller! Make sure it is in scene or is not spawned after frame 1!");
    }

    private async void Start()
    {
        _camerasController.SetActiveCamera(levelOverviewCamera);
        await UniTask.Delay(TimeSpan.FromSeconds(levelOverviewTime));

        _camerasController.SetActiveCamera(spawnPointCamera);
        await UniTask.Delay(TimeSpan.FromSeconds(2f));

        MakeTurn();
    }

    private async void MakeTurn()
    {
        _currentTurnPlayerView = _playersManager.GetNextPlayer(_currentTurnPlayerView);

        _camerasController.SetActiveCamera(_currentTurnPlayerView.BallCamera, .5f);

        switch (_currentTurnPlayerView.PlayerState)
        {
            case PlayerState.Hidden:
                _currentTurnPlayerView.SetBallPosition(spawnPoint.position + Vector3.up * 20f);

                _currentTurnPlayerView.SetShowTrajectory(false);

                _currentTurnPlayerView.Show();
                _currentTurnPlayerView.JumpIn(spawnPoint.position, jumpInTime);
                await UniTask.Delay(TimeSpan.FromSeconds(jumpInTime));

                _currentTurnPlayerView.SetShowTrajectory(true);

                _currentTurnPlayerView._shooter.ballStorage.GetComponent<BallBehaviour>().BecameStill +=
                    OnCurrentPlayerBecameStill;

                _currentTurnPlayerView.PlayerState = PlayerState.ActiveAiming;

                break;
            case PlayerState.Inactive:
                _currentTurnPlayerView.SetShowTrajectory(true);

                _currentTurnPlayerView._shooter.ballStorage.GetComponent<BallBehaviour>().BecameStill +=
                    OnCurrentPlayerBecameStill;

                _currentTurnPlayerView.PlayerState = PlayerState.ActiveAiming;
                break;
            case PlayerState.ActiveAiming:
                break;
            case PlayerState.ActiveInMotion:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void OnCurrentPlayerBecameStill()
    {
        _currentTurnPlayerView._shooter.ballStorage.GetComponent<BallBehaviour>().BecameStill -=
            OnCurrentPlayerBecameStill;
        _currentTurnPlayerView.PlayerState = PlayerState.Inactive;
        _currentTurnPlayerView.SetShowTrajectory(false);
        MakeTurn();
    }
}