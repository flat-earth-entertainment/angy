using System;
using System.Collections.Generic;
using System.Linq;
using Ball;
using Ball.Objectives;
using Cinemachine;
using Config;
using Cysharp.Threading.Tasks;
using Player;
using Rewired;
using UI;
using UnityEngine;
using Utils;

namespace Logic
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField]
        private LineRenderer lineRenderer;

        [SerializeField]
        private UiController uiController;

        private CinemachineVirtualCamera _levelOverviewCamera;
        private CinemachineVirtualCamera _spawnPointCamera;
        private Transform _spawnPoint;
        private PlayersManager _playersManager;
        private PlayerView _currentTurnPlayer;
        private VirtualCamerasController _camerasController;
        private PanController _panController;
        private bool _isInMapOverview;

        private void Awake()
        {
            if (Time.timeScale < 1)
            {
                Time.timeScale = 1;
            }

            _spawnPoint = GameConfig.Instance.Tags.SpawnPointTag.SafeFindWithThisTag().transform;

            _spawnPointCamera = GameConfig.Instance.Tags.SpawnPointVCamTag.SafeFindWithThisTag()
                .GetComponent<CinemachineVirtualCamera>();

            _levelOverviewCamera = GameConfig.Instance.Tags.LevelOverviewVCamTag.SafeFindWithThisTag()
                .GetComponent<CinemachineVirtualCamera>();

            _playersManager = FindObjectOfType<PlayersManager>();
            if (!_playersManager)
                Debug.LogError("Can't find Players Manager! Make sure it is in scene or is not spawned after frame 1!");

            _camerasController = FindObjectOfType<VirtualCamerasController>();
            if (!_camerasController)
                Debug.LogError(
                    "Can't find Virtual Cameras Controller! Make sure it is in scene or is not spawned after frame 1!");

            _panController = FindObjectOfType<PanController>();
            if (!_panController)
                Debug.LogError(
                    "Can't find Pan Controller! Make sure it is in scene or is not spawned after frame 1!");


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
            Debug.Log($"{player.Nickname} went out of bounds");

            player.Hide();

            player.AlterAngy(AngyEvent.FellOutOfTheMap);

            if (player.Angy >= GameConfig.Instance.AngyValues.MaxAngy)
            {
                player.AlterAngy(AngyEvent.AfterFellOutOfTheMapAndReachedMaxAngy);
            }

            player.PlayerState = PlayerState.ShouldSpawn;

            //Unsubscribe as the current player should fall only as a result of shooting
            if (player == _currentTurnPlayer)
            {
                _currentTurnPlayer.BecameStill -= OnCurrentPlayerBecameStill;
                MakeTurn();
            }
        }

        private void OnPlayerEnteredHole(PlayerView player)
        {
            Time.timeScale = 0;
            var pointController = FindObjectOfType<PointController>();

            var winnerPoints = pointController.pointIds.Max();

            var winnerId = pointController.pointIds.IndexOf(winnerPoints);

            var winner = _playersManager.Players.First(p => p.PlayerId == winnerId);

            var others = new List<(PlayerView, int)>();

            for (int i = 0; i < _playersManager.Players.Count; i++)
            {
                if (i == winnerId)
                    continue;

                others.Add((_playersManager.Players.First(p => p.PlayerId == i), pointController.pointIds[i]));
            }

            uiController.ShowWinScreen((winner, winnerPoints), others.ToArray());
        }

        private async void Start()
        {
            await _camerasController.BlendTo(_levelOverviewCamera, GameConfig.Instance.LevelOverviewTime);

            await _camerasController.BlendTo(_spawnPointCamera, 2f);

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
                    nextCamera = _spawnPointCamera;
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
                _currentTurnPlayer.ExplodeHideAndResetAngy(); //Angy=0
                _currentTurnPlayer.PlayerState = PlayerState.ShouldSpawn;
                MakeTurn();
                return;
            }

            //Actual loop
            switch (_currentTurnPlayer.PlayerState)
            {
                case PlayerState.ShouldSpawnCantMove:
                    await SpawnShowJumpInAndSetCamera(_currentTurnPlayer, _currentTurnPlayer.LastStillPosition);

                    _currentTurnPlayer.PlayerState = PlayerState.ShouldMakeTurn;

                    MakeTurn();
                    return;

                case PlayerState.ShouldSpawn:
                    await SpawnShowJumpInAndSetCamera(_currentTurnPlayer, _spawnPoint.position);

                    goto case PlayerState.ShouldMakeTurn;

                case PlayerState.ShouldMakeTurn:
                    uiController.EnableAngyMeterFor(_currentTurnPlayer);

                    _currentTurnPlayer.Predict();

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

            _currentTurnPlayer.SetControlsActive(false);
            SetTrajectoryActive(false);

            _currentTurnPlayer.AlterAngy(AngyEvent.ShotMade);

            _currentTurnPlayer.PlayerState = PlayerState.ActiveInMotion;
        }

        private async void OnCurrentPlayerBecameStill()
        {
            _currentTurnPlayer.BecameStill -= OnCurrentPlayerBecameStill;

            uiController.DisableAngyMeter();

            //If angy became full
            if (_currentTurnPlayer.Angy >= GameConfig.Instance.AngyValues.MaxAngy)
            {
                _currentTurnPlayer.PlayerState = PlayerState.ShouldSpawnCantMove;

                //TODO: Play explosion animation
                _currentTurnPlayer.ExplodeHideAndResetAngy();
            }
            else
            {
                _currentTurnPlayer.PlayerState = PlayerState.ShouldMakeTurn;
            }

            await UniTask.Delay(TimeSpan.FromSeconds(GameConfig.Instance.PreNextTurnDelay));
            MakeTurn();
        }

        private async void Update()
        {
            if (_currentTurnPlayer != null
                && _currentTurnPlayer.PlayerState == PlayerState.ActiveAiming &&
                ReInput.players.GetPlayer(_currentTurnPlayer.PlayerId).GetButtonDown("CameraMode"))
            {
                _isInMapOverview = !_isInMapOverview;

                _currentTurnPlayer.SetControlsActive(!_isInMapOverview);

                if (_isInMapOverview)
                {
                    await _camerasController.BlendTo(_panController.PanningCamera, 0f);
                    _panController.EnableControls(_currentTurnPlayer);
                    uiController.SetCameraModeActive(true);
                }
                else
                {
                    _panController.DisableControls();
                    uiController.SetCameraModeActive(false);
                    await _camerasController.BlendTo(_currentTurnPlayer.BallCamera, 0f);
                }
            }
        }
    }
}