using System;
using System.Linq;
using Audio;
using Ball;
using Cinemachine;
using Config;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using ExitGames.Client.Photon;
using GameSession;
using Logic;
using NaughtyAttributes;
using Photon.Pun;
using Photon.Realtime;
using Player.Input;
using UnityEngine;
using Utils;

namespace Player
{
    public class PlayerView : MonoBehaviour, IPunInstantiateMagicCallback, IOnEventCallback
    {
        [SerializeField]
        [ReadOnly]
        private PlayerState playerState;

        [SerializeField]
        private SkinnedMeshRenderer skinnedMeshRenderer;

        [SerializeField]
        private Shooter shooter;

        [SerializeField]
        private BallBehaviour ballBehaviour;

        [SerializeField]
        private OutOfBoundsCheck outOfBoundsCheck;

        [field: SerializeField]
        public GameObject Ball { get; private set; }

        [field: SerializeField]
        public Animator Animator { get; private set; }

        [field: SerializeField]
        public CinemachineVirtualCamera BallCamera { get; private set; }

        [field: SerializeField]
        public Rigidbody BallRigidbody { get; private set; }

        private PlayerPreset _playerPreset;

        public PlayerPreset PlayerPreset
        {
            get => _playerPreset;
            set
            {
                _playerPreset = value;
                SetFresnelColor(_playerPreset.FresnelColor);
                SetPlayerColor(_playerPreset.PlayerColor);
            }
        }

        private void SetFresnelColor(Color color)
        {
            var bodyMaterial = Materials[0];

            bodyMaterial.SetColor("Fresnel_Color1", color);

            SetBodyMaterial(bodyMaterial);
        }

        public void SetPlayerColor(Color color)
        {
            var oldMaterials = Materials;

            oldMaterials[0].SetColor("Color_Primary", color);

            Materials = oldMaterials;
        }

        public float ExpandPercent
        {
            get => skinnedMeshRenderer.GetBlendShapeWeight(0);
            set => skinnedMeshRenderer.SetBlendShapeWeight(0, value);
        }

        public Material[] Materials
        {
            get => shooter.lemming.transform.GetChild(0).GetChild(0).GetComponent<SkinnedMeshRenderer>().materials
                .Select(s => new Material(s)).ToArray();
            set => shooter.lemming.transform.GetChild(0).GetChild(0).GetComponent<SkinnedMeshRenderer>().materials =
                value;
        }


        public Vector3 LastStillPosition { get; set; }

        [ShowNativeProperty]
        public Vector3 LastStandablePosition { get; set; }

        public float Knockback { get; set; }

        public float Drag
        {
            get => BallRigidbody.drag;
            set => BallRigidbody.drag = value;
        }

        public IPlayerInputs PlayerInputs { get; set; }

        public static event Action<PlayerView, PlayerState, PlayerState> PlayerStateChanged;

        public PlayerState PlayerState
        {
            get => playerState;
            set
            {
                Debug.Log(PlayerPreset.PlayerName.Color(PlayerPreset.PlayerColor) + "'s state was " + playerState +
                          " and became " + value);

                PlayerStateChanged?.Invoke(this, playerState, value);

                playerState = value;
            }
        }

        public void ChangeStateAndNotify(PlayerState newPlayerState)
        {
            PlayerState = newPlayerState;
            if (!PhotonNetwork.OfflineMode)
            {
                var playerFromPlayerView = CurrentGameSession.PlayerFromPlayerView(this);
                if (playerFromPlayerView != null)
                {
                    PhotonShortcuts.ReliableRaiseEventToOthers(GameEvent.PlayerStateChanged,
                        new object[] {playerFromPlayerView.Id, (byte) newPlayerState});
                }
            }
        }


        private void Awake()
        {
            shooter.SetPlayer(this);
        }

        private async void OnEnable()
        {
            PhotonNetwork.AddCallbackTarget(this);

            ballBehaviour.BecameStill += OnBallBecameStill;
            shooter.Shot += OnBallShot;
            outOfBoundsCheck.WentOutOfBounds += OnWentOutOfBounds;

            await UniTask.WaitUntil(() => PlayerInputs != null);
            PlayerInputs.MenuButtonPressed += OnMenuButtonPressed;
        }

        private void OnDisable()
        {
            PhotonNetwork.RemoveCallbackTarget(this);

            ballBehaviour.BecameStill -= OnBallBecameStill;
            shooter.Shot -= OnBallShot;
            outOfBoundsCheck.WentOutOfBounds -= OnWentOutOfBounds;
            PlayerInputs.MenuButtonPressed -= OnMenuButtonPressed;
        }

        public static event Action<PlayerView> OptionsMenuRequested;

        public event Action BecameStill;
        public event Action Shot;
        public event Action<PlayerView> WentOutOfBounds;

        public void Predict()
        {
            shooter.Invoke(nameof(Shooter.Predict), 1f);
            shooter.Predict();
        }

        public void SetIdleAnimation()
        {
            if (shooter.lemmingAnim != null && !shooter.lemmingAnim.Equals(null))
            {
                shooter.lemmingAnim.SetBool("isBall", false);
                shooter.lemmingAnim.SetBool("isKnockback", false);
            }
        }

        public void SetBodyMaterial(Material material)
        {
            var originalMaterials = Materials;
            originalMaterials[0] = material;
            Materials = originalMaterials;
        }


        public UniTask ExplodeAndHide()
        {
            //TODO: Convert to proper animation
            BallRigidbody.GetComponent<Collider>().enabled = false;
            BallRigidbody.useGravity = false;

            const float expandTime = .5f;
            const float shakeTime = 1f;

            return DOTween.Sequence()
                .Append(Ball.transform.DOShakePosition(shakeTime, .2f, 50))
                .Join(Ball.transform.DOShakeScale(shakeTime, .2f, 50))
                .AppendCallback(delegate { Animator.SetBool("isInflated", true); })
                .Append(BallRigidbody.transform.DOScale(Vector3.one * 5, expandTime))
                .Join(DOTween.To(() => ExpandPercent, f => ExpandPercent = f, 100, expandTime)
                    .SetEase(Ease.OutElastic)
                    .SetUpdate(UpdateType.Fixed))
                .AppendCallback(delegate
                {
                    var splat = Instantiate(GameConfig.Instance.BloodSplat, Ball.transform.position,
                        GameConfig.Instance.BloodSplat.transform.rotation);
                    Destroy(splat, 5f);
                    splat?.GetComponent<ParticleSystem>().Play();
                    BallRigidbody.GetComponent<Collider>().enabled = true;
                    BallRigidbody.useGravity = true;
                    BallRigidbody.transform.localScale = Vector3.one;
                    Hide();
                    Animator.SetBool("isInflated", false);
                    AudioManager.PlaySfx(SfxType.LemmingExplosion);
                    ExpandPercent = 0f;
                })
                .SetUpdate(true)
                .SetEase(Ease.Linear).ToUniTask();
        }

        public void ShouldPlayerActivate(PlayerView playerView)
        {
            shooter.ShouldPlayerActivate(playerView);
        }

        private void OnMenuButtonPressed()
        {
            OptionsMenuRequested?.Invoke(this);
            PhotonShortcuts.ReliableRaiseEventToOthers(GameEvent.PlayerMenuOpened,
                CurrentGameSession.PlayerFromPlayerView(this).Id);
        }

        private void OnBallBecameStill()
        {
            var lastStillPosition = BallRigidbody.position;
            if (Physics.Raycast(BallRigidbody.position, Vector3.down, out var hit, 10f,
                LayerMask.GetMask("IgnoredMap")))
            {
                lastStillPosition = hit.point;
            }

            LastStillPosition = lastStillPosition;

            BecameStill?.Invoke();
            var playerFromPlayerView = CurrentGameSession.PlayerFromPlayerView(this);
            if (playerFromPlayerView != null)
                PhotonShortcuts.ReliableRaiseEventToOthers(GameEvent.PlayerBecameStill,
                    playerFromPlayerView.Id);
        }

        private void OnWentOutOfBounds()
        {
            WentOutOfBounds?.Invoke(this);
            PhotonShortcuts.ReliableRaiseEventToOthers(GameEvent.PlayerWentOutOfBounds,
                CurrentGameSession.PlayerFromPlayerView(this).Id);
        }

        private void OnBallShot()
        {
            Shot?.Invoke();
        }

        public UniTask JumpIn(Vector3 endPosition, float jumpTime = 1f)
        {
            endPosition.y += BallRigidbody.GetComponent<SphereCollider>().radius;

            return BallRigidbody.transform.DOMove(endPosition, jumpTime)
                .OnComplete(delegate
                {
                    BallRigidbody.velocity = Vector3.zero;
                    BallRigidbody.angularVelocity = Vector3.zero;
                })
                .SetEase(Ease.OutBounce)
                .SetUpdate(true).ToUniTask();
        }

        public void Show()
        {
            Ball.SetActive(true);
        }

        public void Hide()
        {
            Ball.SetActive(false);
        }

        public void SetControlsActive(bool toggle)
        {
            shooter.enabled = toggle;
        }

        public void SetBallPosition(Vector3 position)
        {
            position.y += BallRigidbody.GetComponent<SphereCollider>().radius;
            BallRigidbody.transform.position = position;
        }

        public void OnPhotonInstantiate(PhotonMessageInfo info)
        {
            var addresseeActorNumber = (int) info.photonView.InstantiationData[0];

            if (PhotonNetwork.OfflineMode)
            {
                var player = CurrentGameSession.Players.First(p => p.RoundPlayerView == null);
                player.RoundPlayerView = this;
                if (player is LocalPlayer localPlayer)
                {
                    PlayerInputs = RewiredPlayerInputs.AttachToPlayer(this, localPlayer.RewiredPlayer);
                }
            }
            else
            {
                if (PhotonNetwork.LocalPlayer.ActorNumber == addresseeActorNumber)
                {
                    var player = CurrentGameSession.Players.First(p => p is LocalPlayer);
                    player.RoundPlayerView = this;
                    PlayerInputs = RewiredPlayerInputs.AttachToPlayer(this, ((LocalPlayer) player).RewiredPlayer);
                }
                else
                {
                    CurrentGameSession.Players
                        .First(p =>
                            p is OnlinePlayer onlinePlayer &&
                            onlinePlayer.PhotonPlayer.ActorNumber == addresseeActorNumber)
                        .RoundPlayerView = this;

                    PlayerInputs = new NetworkPlayerInputs();
                }
            }

            FindObjectOfType<PlayersManager>().AddNewPlayer(this);
        }

        public void OnEvent(EventData photonEvent)
        {
            if (!CurrentGameSession.IsNowPassive)
            {
                return;
            }

            if (!Enum.IsDefined(typeof(GameEvent), photonEvent.Code) ||
                photonEvent.Code == GameEvent.PlayerStateChanged.ToByte())
            {
                return;
            }

            var gameEvent = (GameEvent) photonEvent.Code;

            var senderPlayer = OnlinePlayer.SessionPlayerByActorNumber(photonEvent.Sender);

            if (!(photonEvent.CustomData is int id) || id != CurrentGameSession.PlayerFromPlayerView(this).Id)
            {
                return;
            }

            switch (gameEvent)
            {
                case GameEvent.PlayerShot:
                    Shot?.Invoke();
                    break;
                case GameEvent.PlayerBecameStill:
                    BecameStill?.Invoke();
                    break;
                case GameEvent.PlayerWentOutOfBounds:
                    WentOutOfBounds?.Invoke(this);
                    break;
                case GameEvent.PlayerMenuOpened:
                    OptionsMenuRequested?.Invoke(this);
                    break;
            }
        }
    }
}