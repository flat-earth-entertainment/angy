using System;
using System.Collections.Generic;
using System.Linq;
using Ball;
using Ball.Objectives;
using Cinemachine;
using Config;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Player;
using Rewired;
using UI;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utils;

namespace Logic
{
    public class GameManager : MonoBehaviour
    {
        public static PlayerView CurrentTurnPlayer => _currentTurnPlayer;

        [SerializeField]
        private LineRenderer lineRenderer;

        [SerializeField]
        private UiController uiController;

        private static PlayerView _currentTurnPlayer;
        private CinemachineVirtualCamera _levelOverviewCamera;
        private CinemachineVirtualCamera _spawnPointCamera;
        private Transform _spawnPoint;
        private PlayersManager _playersManager;
        private VirtualCamerasController _camerasController;
        private PanController _panController;
        private bool _isInMapOverview;


        private void Awake()
        {
            Time.timeScale = GameConfig.Instance.TimeScale;

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
            Hole.PlayerLeftHole += OnPlayerLeftHole;


            _playersManager.InitializedAllPlayers += OnAllPlayersInitialized;

            HitOtherPlayerTrigger.PlayerHit += OnPlayerGotHit;

            GoodNeutralMushroom.BecameHole += OnHoleAppeared;
        }

        private void OnDisable()
        {
            Hole.PlayerEnteredHole -= OnPlayerEnteredHole;
            Hole.PlayerLeftHole -= OnPlayerLeftHole;

            foreach (var player in _playersManager.Players)
            {
                player.WentOutOfBounds -= OnPlayerWentOutOfBounds;
            }

            HitOtherPlayerTrigger.PlayerHit -= OnPlayerGotHit;
            GoodNeutralMushroom.BecameHole -= OnHoleAppeared;
        }

        private async void OnHoleAppeared(GameObject obj)
        {
            Time.timeScale = 0;

            await _camerasController.BlendTo(obj.GetComponentInChildren<CinemachineVirtualCamera>(),
                GameConfig.Instance.FlyToNextPlayerTime);

            await UniTask.Delay(TimeSpan.FromSeconds(GameConfig.Instance.HoleOrbitTime), DelayType.Realtime);

            await _camerasController.BlendTo(_currentTurnPlayer.BallCamera, GameConfig.Instance.FlyToNextPlayerTime);

            Time.timeScale = GameConfig.Instance.TimeScale;
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

            player.PlayerState = PlayerState.ShouldSpawnFirstTime;

            //Unsubscribe as the current player should fall only as a result of shooting
            if (player == _currentTurnPlayer)
            {
                _currentTurnPlayer.BecameStill -= OnCurrentPlayerBecameStill;
                MakeTurn();
            }
        }

        private Tween _enteredHoleTimer;

        private void OnPlayerEnteredHole(PlayerView player)
        {
            _enteredHoleTimer = DOTween.Sequence()
                .AppendInterval(.75f)
                .OnComplete(delegate { OnPlayerConfirmedPresenceInHole(player); })
                .SetUpdate(true);
        }

        private void OnPlayerLeftHole(PlayerView obj)
        {
            _enteredHoleTimer?.Kill(false);
        }

        private void OnPlayerConfirmedPresenceInHole(PlayerView player)
        {
            Time.timeScale = 0;

            var points = FindObjectOfType<PointController>().GetPoints();

            var winnerPoints = points.Max();
            var winnerId = points.IndexOf(winnerPoints);
            var winner = _playersManager.Players.First(p => p.PlayerId == winnerId);

            var others = new List<(PlayerView, int)>();

            for (int i = 0; i < _playersManager.Players.Count; i++)
            {
                if (i == winnerId)
                    continue;

                others.Add((_playersManager.Players.First(p => p.PlayerId == i), points[i]));
            }

            uiController.ShowWinScreen((winner, winnerPoints), others.ToArray());


            CurrentGameSession.Leaderboard
                .Add(new MapScore(SceneManager.GetActiveScene().name, points[0], points[1]));
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

            _currentTurnPlayer.SetIdleAnimation();

            CinemachineVirtualCamera nextCamera = null;

            //Decide where to move camera to
            switch (_currentTurnPlayer.PlayerState)
            {
                case PlayerState.ShouldSpawnFirstTime:
                    nextCamera = _spawnPointCamera;
                    break;
                //TODO: Decide where should spawn in this case
                case PlayerState.ShouldSpawnCanNotMove:
                case PlayerState.ShouldMakeTurn:
                    nextCamera = _currentTurnPlayer.BallCamera;
                    break;
            }

            await _camerasController.BlendTo(nextCamera, GameConfig.Instance.FlyToNextPlayerTime);

            //Explode and skip turn if needed
            if (_currentTurnPlayer.Angy >= GameConfig.Instance.AngyValues.MaxAngy)
            {
                _currentTurnPlayer.ExplodeHideAndResetAngy();
                _currentTurnPlayer.PlayerState = PlayerState.ShouldSpawnFirstTime;
                MakeTurn();
                return;
            }

            //Actual loop
            switch (_currentTurnPlayer.PlayerState)
            {
                case PlayerState.ShouldSpawnCanNotMove:
                    await SpawnShowJumpInAndSetCamera(_currentTurnPlayer, _currentTurnPlayer.LastStillPosition);

                    _currentTurnPlayer.PlayerState = PlayerState.ShouldMakeTurn;

                    MakeTurn();
                    return;

                case PlayerState.ShouldSpawnFirstTime:
                    await SpawnShowJumpInAndSetCamera(_currentTurnPlayer, _spawnPoint.position);
                    goto case PlayerState.ShouldMakeTurn;

                case PlayerState.ShouldSpawnCanMove:
                    await SpawnShowJumpInAndSetCamera(_currentTurnPlayer, _currentTurnPlayer.LastStillPosition);
                    goto case PlayerState.ShouldMakeTurn;

                case PlayerState.ShouldMakeTurn:
                    uiController.EnableAngyMeterFor(_currentTurnPlayer);

                    _currentTurnPlayer.Predict();

                    _currentTurnPlayer.SetControlsActive(true);
                    // _currentTurnPlayer.SetLookAtTrajectory(true);
                    SetTrajectoryActive(true);

                    _currentTurnPlayer.Shot += OnPlayerShot;
                    _currentTurnPlayer.PlayerInputs.MapViewButtonPressed += OnMapButtonPressed;

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
            _currentTurnPlayer.PlayerInputs.MapViewButtonPressed -= OnMapButtonPressed;

            _currentTurnPlayer.BecameStill += OnCurrentPlayerBecameStill;
            _currentTurnPlayer.PlayerInputs.AbilityButtonPressed += OnAbilityButtonPressed;

            _currentTurnPlayer.SetControlsActive(false);
            // _currentTurnPlayer.SetLookAtTrajectory(false);
            SetTrajectoryActive(false);

            _currentTurnPlayer.AlterAngy(AngyEvent.ShotMade);

            _currentTurnPlayer.PlayerState = PlayerState.ActiveInMotion;
        }

        private void OnAbilityButtonPressed()
        {
            if (_currentTurnPlayer.Ability != null)
            {
                _currentTurnPlayer.Ability.InvokeAbility(_currentTurnPlayer);
                _currentTurnPlayer.Ability = null;
            }
            else
            {
                Debug.LogWarning($"Player {_currentTurnPlayer.Nickname} doesn't have an ability!");
            }
        }

        private async void OnCurrentPlayerBecameStill()
        {
            _currentTurnPlayer.BecameStill -= OnCurrentPlayerBecameStill;
            _currentTurnPlayer.PlayerInputs.AbilityButtonPressed -= OnAbilityButtonPressed;

            uiController.DisableAngyMeter();

            //If angy became full
            if (_currentTurnPlayer.Angy >= GameConfig.Instance.AngyValues.MaxAngy)
            {
                _currentTurnPlayer.PlayerState = PlayerState.ShouldSpawnCanNotMove;

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

        private async void OnMapButtonPressed()
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

        private void Update()
        {
#if UNITY_EDITOR
            if (Input.GetKeyDown(KeyCode.O))
            {
                OnPlayerConfirmedPresenceInHole(null);
            }
#endif
        }
    }
}