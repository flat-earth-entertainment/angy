using System;
using System.Linq;
using Audio;
using Ball;
using Ball.Objectives;
using Cinemachine;
using Config;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Environment;
using ExitGames.Client.Photon;
using GameSession;
using Photon.Pun;
using Photon.Realtime;
using Player;
using UI;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utils;

namespace Logic
{
    public class GameManager : MonoBehaviour, IOnEventCallback
    {
        public static event Action RoundPassed;
        public PlayerView CurrentTurnPlayer => _currentTurnPlayer;

        [SerializeField]
        private TrajectoryLineController trajectoryLineController;

        [SerializeField]
        private UiController uiController;

        [SerializeField]
        private AngyController angyController;

        [SerializeField]
        private AbilityController abilityController;


        private PlayerView _currentTurnPlayer;
        private PlayerView _firstPlayer;
        private PlayerView _playerInOptions;
        private CinemachineVirtualCamera _levelOverviewCamera;
        private CinemachineVirtualCamera _spawnPointCamera;
        private Transform _spawnPoint;
        private PlayersManager _playersManager;
        private VirtualCamerasController _camerasController;
        private PanController _panController;
        private bool _isInMapOverview;
        private bool _playersInitialized;
        private bool _subscribedToMainEvents;
        private bool _isPassive;

        private void Awake()
        {
            _currentTurnPlayer = null;

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

            trajectoryLineController.SetTrajectoryActive(false);

            _playersManager.InitializedAllPlayers += OnAllPlayersInitialized;
        }

        private async void Start()
        {
            await _camerasController.BlendTo(_levelOverviewCamera, GameConfig.Instance.LevelOverviewTime);

            await _camerasController.BlendTo(_spawnPointCamera, 2f);

            if (!_playersInitialized)
                await UniTask.WaitUntil(() => _playersInitialized);

            MakeTurn();
        }


        private void SubscribeToMainEvents()
        {
            Hole.PlayerEnteredHole += OnPlayerEnteredHole;
            Hole.PlayerLeftHole += OnPlayerLeftHole;

            HitOtherPlayerTrigger.PlayerHit += OnPlayerGotHit;

            GoodNeutralMushroom.HoleSpawned += OnHoleAppeared;

            PlayerView.OptionsMenuRequested += OnPauseMenuOpenRequested;

            KillingTrigger.HitKillTrigger += OnPlayerHitKillTrigger;

            PhotonNetwork.AddCallbackTarget(this);

            _subscribedToMainEvents = true;
        }

        private void UnsubscribeFromMainEvents()
        {
            if (!_subscribedToMainEvents)
                return;

            Hole.PlayerEnteredHole -= OnPlayerEnteredHole;
            Hole.PlayerLeftHole -= OnPlayerLeftHole;

            UnsubscribeOutOfBounds();

            HitOtherPlayerTrigger.PlayerHit -= OnPlayerGotHit;
            GoodNeutralMushroom.HoleSpawned -= OnHoleAppeared;

            PlayerView.OptionsMenuRequested -= OnPauseMenuOpenRequested;
            KillingTrigger.HitKillTrigger -= OnPlayerHitKillTrigger;

            PhotonNetwork.RemoveCallbackTarget(this);

            Time.timeScale = GameConfig.Instance.TimeScale;
        }

        private void OnDisable()
        {
            UnsubscribeFromMainEvents();
        }

        private void UnsubscribeOutOfBounds()
        {
            foreach (var player in _playersManager.Players)
            {
                player.WentOutOfBounds -= OnPlayerWentOutOfBounds;
            }
        }


        private bool _shouldSubscribeToMenuClose;

        private void OnPauseMenuOpenRequested(PlayerView caller)
        {
            if (_playerInOptions != null
                || caller.PlayerState == PlayerState.ActivePowerMode
                || _currentTurnPlayer != null && _currentTurnPlayer.PlayerState == PlayerState.ActivePowerMode)
                return;

            if (!_shouldSubscribeToMenuClose)
            {
                _shouldSubscribeToMenuClose = true;
                DOTween.Sequence().AppendInterval(0.1f)
                    .AppendCallback(() =>
                    {
                        PlayerView.OptionsMenuRequested += OnOptionsMenuCloseRequested;
                        _shouldSubscribeToMenuClose = false;
                    });
            }

            PauseMenu.Show(() => { OnOptionsMenuCloseRequested(caller); });

            _playerInOptions = caller;
        }

        private async void OnOptionsMenuCloseRequested(PlayerView caller)
        {
            PlayerView.OptionsMenuRequested -= OnOptionsMenuCloseRequested;

            PauseMenu.Hide();

            await UniTask.Delay(TimeSpan.FromMilliseconds(100), DelayType.UnscaledDeltaTime);
            _playerInOptions = null;
        }

        private async void OnHoleAppeared(GameObject hole)
        {
            Time.timeScale = 0;

            var holeCamera = hole.GetComponentInChildren<CinemachineVirtualCamera>();

            await _camerasController.BlendTo(holeCamera,
                GameConfig.Instance.FlyToNextPlayerTime);

            await UniTask.Delay(TimeSpan.FromSeconds(GameConfig.Instance.HoleOrbitTime), DelayType.UnscaledDeltaTime);

            await _camerasController.BlendTo(_currentTurnPlayer.BallCamera, GameConfig.Instance.FlyToNextPlayerTime);

            Time.timeScale = GameConfig.Instance.TimeScale;
        }

        private void OnAllPlayersInitialized(PlayerView[] players)
        {
            _playersInitialized = true;
            _playersManager.InitializedAllPlayers -= OnAllPlayersInitialized;

            foreach (var player in players)
            {
                player.WentOutOfBounds += OnPlayerWentOutOfBounds;
            }

            SubscribeToMainEvents();
        }

        private void OnPlayerGotHit(PlayerView arg1, PlayerView arg2)
        {
            if (arg1 == _currentTurnPlayer)
            {
                angyController.AlterAngyIfActive(arg1, AngyEvent.HitSomeone);
                angyController.AlterAngyIfActive(arg2, AngyEvent.GotHit);
            }
        }

        private void OnPlayerWentOutOfBounds(PlayerView player)
        {
            //Shouldn't react when not spawned
            switch (player.PlayerState)
            {
                case PlayerState.ShouldSpawnAtSpawn:
                case PlayerState.ShouldSpawnAtLastPosition:
                case PlayerState.ShouldSpawnAtLastStandablePosition:
                case PlayerState.ShouldSpawnCanNotMove:
                    return;
            }

            Debug.Log($"{player.PlayerPreset.PlayerName.Color(player.PlayerPreset.PlayerColor)} went out of bounds");

            abilityController.GetPlayerAbility(player)?.Wrap();
            player.Hide();

            AudioManager.PlaySfx(SfxType.LemmingLaunch);

            angyController.AlterAngyIfActive(player, AngyEvent.FellOutOfTheMap);

            if (angyController[player] >= GameConfig.Instance.AngyValues.MaxAngy)
            {
                angyController.AlterAngyIfActive(player, AngyEvent.AfterFellOutOfTheMapAndReachedMaxAngy);
            }

            player.ChangeStateAndNotify(PlayerState.ShouldSpawnAtLastPosition);

            //Unsubscribe as the current player should fall only as a result of shooting
            if (player == _currentTurnPlayer)
            {
                EndTurnFor(player);
                MakeTurn();
            }
        }

        private async void OnPlayerHitKillTrigger(PlayerView player)
        {
            Utilities.EmitExplosionAtPosition(player.BallRigidbody.position);

            abilityController.GetPlayerAbility(player)?.Wrap();
            player.Hide();

            angyController.AlterAngyIfActive(player, AngyEvent.FellOutOfTheMap);

            if (angyController[player] >= GameConfig.Instance.AngyValues.MaxAngy)
            {
                angyController.AlterAngyIfActive(player, AngyEvent.AfterFellOutOfTheMapAndReachedMaxAngy);
            }

            player.ChangeStateAndNotify(PlayerState.ShouldSpawnAtLastStandablePosition);

            //Unsubscribe as the current player should fall only as a result of shooting
            if (player == _currentTurnPlayer)
            {
                EndTurnFor(player);
                await DelayAndMakeTurn();
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
            _enteredHoleTimer?.Kill();
        }

        private void OnPlayerConfirmedPresenceInHole(PlayerView player)
        {
            player.BallRigidbody.constraints = RigidbodyConstraints.FreezeAll;
            UnsubscribeFromPreStillEvents(player);
            var points = FindObjectOfType<PointController>().GetPoints();

            var winnerPoints = points.Values.Max();
            var winner = points.First(t => t.Value == winnerPoints).Key;

            CurrentGameSession.WinnerMaterial = winner.Materials[0];
            CurrentGameSession.LoserMaterial = _playersManager.Players.First(p => p != winner).Materials[0];

            CurrentGameSession.SetNextRoundPlayer(winner);

            var redPlayerScore = points.First(p =>
                p.Key == CurrentGameSession.Players.First(c => c.PresetIndex == 0).RoundPlayerView).Value;
            var bluePlayerScore = points.First(p =>
                p.Key == CurrentGameSession.Players.First(c => c.PresetIndex == 1).RoundPlayerView).Value;

            CurrentGameSession.CollectionScores
                .SetMapScore(SceneManager.GetActiveScene().name, new MapScore(redPlayerScore, bluePlayerScore));

            CurrentGameSession.SetNextRoundPlayer(winner);
            CurrentGameSession.ResetPlayerViews();

            OnLevelFinished();
        }

        private static async void OnLevelFinished()
        {
            await SceneManager.UnloadSceneAsync("Prediction");

            var currentMap = SceneManager.GetActiveScene().name;

            LeaderboardSceneUiController.SceneToLoad =
                !CurrentGameSession.IsLastMapInList(currentMap)
                    ? CurrentGameSession.GetNextMap(currentMap)
                    : GameConfig.Instance.Scenes.VictoryScene;

            PhotonShortcuts.ReliableRaiseEventToAll(GameEvent.SceneChange, GameConfig.Instance.Scenes.LeaderboardScene);
            // SceneChanger.BroadcastChangeScene(GameConfig.Instance.Scenes.LeaderboardScene, SceneChangeType.MapChange);
        }

        private async void MakeTurn()
        {
            _currentTurnPlayer = _playersManager.GetNextPlayer(_currentTurnPlayer);


            if (_currentTurnPlayer == _firstPlayer)
                RoundPassed?.Invoke();

            if (_firstPlayer == null)
                _firstPlayer = _currentTurnPlayer;

            //Here player freezes
            _playersManager.PrepareTrajectoryFor(_currentTurnPlayer);

            if (CurrentGameSession.PlayerFromPlayerView(_currentTurnPlayer) is LocalPlayer)
            {
                Debug.Log("this is local player turn");
                foreach (var player in CurrentGameSession.Players)
                {
                    player.RoundPlayerView.GetComponent<PhotonView>().TransferOwnership(PhotonNetwork.LocalPlayer);
                }

                CurrentGameSession.IsNowPassive = false;
            }
            else
            {
                CurrentGameSession.IsNowPassive = true;
            }

            _currentTurnPlayer.SetIdleAnimation();

            var nextCamera =
                _currentTurnPlayer.PlayerState == PlayerState.ShouldSpawnAtSpawn
                    ? _spawnPointCamera
                    : _currentTurnPlayer.BallCamera;

            await _camerasController.BlendTo(nextCamera, GameConfig.Instance.FlyToNextPlayerTime);

            //Explode and skip turn if needed
            if (angyController[_currentTurnPlayer] >= GameConfig.Instance.AngyValues.MaxAngy)
            {
                await OnMaxAngy(_currentTurnPlayer);
                MakeTurn();
                return;
            }

            //Actual loop
            switch (_currentTurnPlayer.PlayerState)
            {
                case PlayerState.ShouldSpawnCanNotMove:
                    await SpawnShowJumpInAndSetCamera(_currentTurnPlayer, _currentTurnPlayer.LastStillPosition);

                    _currentTurnPlayer.ChangeStateAndNotify(PlayerState.ShouldMakeTurn);
                    _currentTurnPlayer.BallRigidbody.constraints = RigidbodyConstraints.None;

                    MakeTurn();
                    return;

                case PlayerState.ShouldSpawnAtSpawn:
                    var spawnPointPosition = _spawnPoint.position;
                    _currentTurnPlayer.LastStillPosition = spawnPointPosition;
                    await SpawnShowJumpInAndSetCamera(_currentTurnPlayer, spawnPointPosition);
                    goto case PlayerState.ShouldMakeTurn;

                case PlayerState.ShouldSpawnAtLastPosition:
                    await SpawnShowJumpInAndSetCamera(_currentTurnPlayer, _currentTurnPlayer.LastStillPosition);
                    goto case PlayerState.ShouldMakeTurn;

                case PlayerState.ShouldSpawnAtLastStandablePosition:
                    await SpawnShowJumpInAndSetCamera(_currentTurnPlayer, _currentTurnPlayer.LastStandablePosition);
                    goto case PlayerState.ShouldMakeTurn;

                case PlayerState.ShouldMakeTurn:
                    uiController.EnableAngyMeter();
                    uiController.EnableAbilityUi();

                    abilityController.CopyPreviousAbilityToCurrent(_currentTurnPlayer);

                    _currentTurnPlayer.Predict();

                    _currentTurnPlayer.SetControlsActive(true);

                    trajectoryLineController.SetTrajectoryActive(true);
                    trajectoryLineController.SetGradientColor(_currentTurnPlayer.PlayerPreset.Gradient);

                    _currentTurnPlayer.ChangeStateAndNotify(PlayerState.ActiveAiming);

                    uiController.CameraModeHelperActive = true;

                    SubscribeToPreShotEvents(_currentTurnPlayer);
                    break;
            }
        }

        private void SubscribeToPreShotEvents(PlayerView player)
        {
            player.Shot += OnPlayerShot;
            player.PlayerInputs.MapViewButtonPressed += OnMapButtonPressed;
            player.PlayerInputs.FireButtonPressed += OnFireButtonPressed;
        }

        private void UnsubscribeFromPreShotEvents(PlayerView player)
        {
            player.Shot -= OnPlayerShot;
            player.PlayerInputs.MapViewButtonPressed -= OnMapButtonPressed;
            player.PlayerInputs.FireButtonPressed -= OnFireButtonPressed;
        }

        private void OnFireButtonPressed()
        {
            _currentTurnPlayer.ChangeStateAndNotify(PlayerState.ActivePowerMode);
            _currentTurnPlayer.PlayerInputs.FireButtonPressed -= OnFireButtonPressed;
        }

        private async UniTask SpawnShowJumpInAndSetCamera(PlayerView player, Vector3 spawnPosition)
        {
            player.Show();

            player.Shooter.SetBallFormActive(true);

            //TODO: Change to animation
            await player.JumpIn(spawnPosition, GameConfig.Instance.JumpInTime);

            player.Shooter.SetBallFormActive(false);

            _camerasController.SetActiveCamera(player.BallCamera, 1f);
        }

        private void OnPlayerShot()
        {
            if (abilityController.HasAbility(_currentTurnPlayer))
            {
                uiController.WobbleAbilityUi(_currentTurnPlayer, true);
            }

            UnsubscribeFromPreShotEvents(_currentTurnPlayer);

            SubscribeToPreStillEvents(_currentTurnPlayer);

            _currentTurnPlayer.SetControlsActive(false);

            trajectoryLineController.SetTrajectoryActive(false);

            uiController.CameraModeHelperActive = false;

            angyController.AlterAngyIfActive(_currentTurnPlayer, AngyEvent.ShotMade);

            _currentTurnPlayer.ChangeStateAndNotify(PlayerState.ActiveInMotion);
        }

        private void SubscribeToPreStillEvents(PlayerView player)
        {
            player.BecameStill += OnCurrentPlayerBecameStill;
            player.PlayerInputs.AbilityButtonPressed += OnAbilityButtonPressed;
        }

        private void UnsubscribeFromPreStillEvents(PlayerView player)
        {
            player.BecameStill -= OnCurrentPlayerBecameStill;
            player.PlayerInputs.AbilityButtonPressed -= OnAbilityButtonPressed;
        }

        private void OnAbilityButtonPressed()
        {
            if (abilityController.TryInvokeAbility(_currentTurnPlayer))
            {
                uiController.WobbleAbilityUi(_currentTurnPlayer, false);
            }
        }

        private async void OnCurrentPlayerBecameStill()
        {
            var currentTurnPlayerAbility = abilityController.GetPlayerAbility(_currentTurnPlayer);

            if (currentTurnPlayerAbility != null && currentTurnPlayerAbility.Active)
            {
                await UniTask.WaitUntil(() => currentTurnPlayerAbility.Finished);
            }

            EndTurnFor(_currentTurnPlayer);

            //If angy became full
            if (angyController[_currentTurnPlayer] >= GameConfig.Instance.AngyValues.MaxAngy)
            {
                await OnMaxAngy(_currentTurnPlayer);
            }
            else
            {
                _currentTurnPlayer.ChangeStateAndNotify(PlayerState.ShouldMakeTurn);
            }

            await DelayAndMakeTurn();
        }

        private UniTask OnMaxAngy(PlayerView player)
        {
            player.ChangeStateAndNotify(PlayerState.ShouldSpawnCanNotMove);

            angyController.ResetAngyFor(_currentTurnPlayer);
            //TODO: Play explosion animation
            return player.ExplodeAndHide();
        }

        private async UniTask DelayAndMakeTurn()
        {
            await UniTask.Delay(TimeSpan.FromSeconds(GameConfig.Instance.PreNextTurnDelay));
            MakeTurn();
        }

        private void EndTurnFor(PlayerView player)
        {
            UnsubscribeFromPreStillEvents(player);

            uiController.DisableAngyMeter();
            uiController.DisableAbilityUi();
            uiController.WobbleAbilityUi(player, false);
            uiController.CameraModeHelperActive = false;

            abilityController.GetPlayerAbility(_currentTurnPlayer)?.Wrap();
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

        public void OnEvent(EventData photonEvent)
        {
            if (!CurrentGameSession.IsNowPassive)
            {
                return;
            }

            if (photonEvent.Code == GameEvent.PlayerAbilityFired.ToByte())
            {
                OnAbilityButtonPressed();
            }

            if (photonEvent.Code == GameEvent.PlayerHitKillTrigger.ToByte())
            {
                var playerId = (int) photonEvent.CustomData;
                OnPlayerHitKillTrigger(CurrentGameSession.PlayerViewFromId(playerId));
            }
        }

#if UNITY_EDITOR
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.O))
            {
                OnPlayerConfirmedPresenceInHole(_currentTurnPlayer);
            }

            if (Input.GetKeyDown(KeyCode.I))
            {
                angyController.AlterAngyIfActive(_currentTurnPlayer, AngyEvent.FellOutOfTheMap);
            }

            if (Input.GetKeyDown(KeyCode.J))
            {
                _currentTurnPlayer.JumpIn(_spawnPoint.position, GameConfig.Instance.JumpInTime);
            }
        }
#endif
    }
}