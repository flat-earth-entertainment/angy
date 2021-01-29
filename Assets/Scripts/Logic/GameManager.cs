using System;
using System.Threading.Tasks;
using Ball;
using Ball.Objectives;
using Cinemachine;
using Config;
using Cysharp.Threading.Tasks;
using Player;
using UI;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private CinemachineVirtualCamera levelOverviewCamera;

    [SerializeField]
    private CinemachineVirtualCamera spawnPointCamera;

    [SerializeField]
    private Transform spawnPoint;

    [SerializeField]
    private LineRenderer lineRenderer;

    [SerializeField]
    private UiController uiController;

    private PlayersManager _playersManager;
    private PlayerView _currentTurnPlayer;
    private VirtualCamerasController _camerasController;

    private void Awake()
    {
        if (Time.timeScale < 1)
        {
            Time.timeScale = 1;
        }

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


        uiController.HideAllUi();
    }

    private void OnEnable()
    {
        Hole.PlayerEnteredHole += OnPlayerEnteredHole;

        _playersManager.InitializedAllPlayers += OnAllPlayersInitialized;

        HitOtherPlayerTrigger.PlayerHit += OnPlayerGotHit;
    }

    private void OnDisable()
    {
        Hole.PlayerEnteredHole -= OnPlayerEnteredHole;

        foreach (var player in _playersManager.Players)
        {
            player.WentOutOfBounds -= OnPlayerWentOutOfBounds;
        }

        HitOtherPlayerTrigger.PlayerHit -= OnPlayerGotHit;
    }

    private void OnAllPlayersInitialized(PlayerView[] players)
    {
        foreach (var player in players)
        {
            Debug.Log("Subscribed " + player.Nickname);
            player.WentOutOfBounds += OnPlayerWentOutOfBounds;
        }
    }

    private void OnPlayerGotHit(PlayerView arg1, PlayerView arg2)
    {
        if (arg1 == _currentTurnPlayer)
        {
            arg1.AlterAngy(AngyEvent.HitSomeone);
            arg2.AlterAngy(AngyEvent.GotHit);
        }
    }

    private void OnPlayerWentOutOfBounds(PlayerView player)
    {
        //Unsubscribe as the current player should fall only as a result of shooting
        if (player == _currentTurnPlayer)
        {
            _currentTurnPlayer.BecameStill -= OnCurrentPlayerBecameStill;
        }

        player.Hide();

        player.AlterAngy(AngyEvent.FellOutOfTheMap);

        if (player.Angy >= GameConfig.Instance.AngyValues.MaxAngy)
        {
            player.AlterAngy(AngyEvent.AfterFellOutOfTheMapAndReachedMaxAngy);
        }

        player.PlayerState = PlayerState.ShouldSpawn;

        MakeTurn();
    }

    private void OnPlayerEnteredHole(PlayerView player)
    {
        Time.timeScale = 0;
        //TODO: Add points for a player entered
        //TODO: Find and show a winner
        uiController.ShowWinScreen(null);
    }

    private async void Start()
    {
        await _camerasController.BlendTo(levelOverviewCamera, GameConfig.Instance.LevelOverviewTime);

        await _camerasController.BlendTo(spawnPointCamera, 2f);

        MakeTurn();
    }

    private void SetTrajectoryActive(bool state)
    {
        lineRenderer.enabled = state;
    }

    private async void MakeTurn()
    {
        _currentTurnPlayer = _playersManager.GetNextPlayer(_currentTurnPlayer);
        _playersManager.PrepareTrajectoryFor(_currentTurnPlayer);

        CinemachineVirtualCamera nextCamera = null;

        //Decide where to move camera to
        switch (_currentTurnPlayer.PlayerState)
        {
            case PlayerState.ShouldSpawn:
                nextCamera = spawnPointCamera;
                break;
            //TODO: Decide where should spawn in this case
            case PlayerState.ShouldSpawnCantMove:
            case PlayerState.ShouldMakeTurn:
                nextCamera = _currentTurnPlayer.BallCamera;
                break;
        }

        await _camerasController.BlendTo(nextCamera, GameConfig.Instance.FlyToNextPlayerTime);

        if (_currentTurnPlayer.Angy >= GameConfig.Instance.AngyValues.MaxAngy)
        {
            _currentTurnPlayer.ExplodeAndHide();
            _currentTurnPlayer.PlayerState = PlayerState.ShouldSpawn;
            MakeTurn();
        }

        switch (_currentTurnPlayer.PlayerState)
        {
            case PlayerState.ShouldSpawnCantMove:
                await SpawnShowJumpInAndSetCamera(_currentTurnPlayer, _currentTurnPlayer.LastStillPosition);

                _currentTurnPlayer.PlayerState = PlayerState.ShouldMakeTurn;

                MakeTurn();
                break;

            case PlayerState.ShouldSpawn:
                await SpawnShowJumpInAndSetCamera(_currentTurnPlayer, spawnPoint.position);

                goto case PlayerState.ShouldMakeTurn;

            case PlayerState.ShouldMakeTurn:
                uiController.EnableAngyMeterFor(_currentTurnPlayer);

                _currentTurnPlayer.SetControlsActive(true);
                SetTrajectoryActive(true);

                _currentTurnPlayer.Shot += OnPlayerShot;

                _currentTurnPlayer.PlayerState = PlayerState.ActiveAiming;
                break;
        }
    }

    private async UniTask SpawnShowJumpInAndSetCamera(PlayerView player, Vector3 spawnPosition)
    {
        //TODO: Change to animation
        player.SetBallPosition(spawnPosition + Vector3.up * 20f);

        player.Show();

        //TODO: Change to animation
        player.JumpIn(spawnPosition, GameConfig.Instance.JumpInTime);
        await UniTask.Delay(TimeSpan.FromSeconds(GameConfig.Instance.JumpInTime));

        _camerasController.SetActiveCamera(player.BallCamera, 1f);
    }

    private void OnPlayerShot()
    {
        _currentTurnPlayer.Shot -= OnPlayerShot;

        _currentTurnPlayer.BecameStill += OnCurrentPlayerBecameStill;

        _currentTurnPlayer.AlterAngy(AngyEvent.ShotMade);

        SetTrajectoryActive(false);
    }

    private void OnCurrentPlayerBecameStill()
    {
        _currentTurnPlayer.BecameStill -= OnCurrentPlayerBecameStill;

        _currentTurnPlayer.SetControlsActive(false);
        uiController.DisableAngyMeter();

        //If angy became full
        if (_currentTurnPlayer.Angy >= GameConfig.Instance.AngyValues.MaxAngy)
        {
            _currentTurnPlayer.PlayerState = PlayerState.ShouldSpawnCantMove;

            //TODO: Play explosion animation
            _currentTurnPlayer.ExplodeAndHide();
        }
        else
        {
            _currentTurnPlayer.PlayerState = PlayerState.ShouldMakeTurn;
        }

        MakeTurn();
    }

    private void Update()
    {
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.KeypadPlus))
        {
            _currentTurnPlayer.AlterAngy(AngyEvent.HitBadObject);
        }
#endif
    }
}