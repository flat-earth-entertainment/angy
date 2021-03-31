using System;
using System.Linq;
using Abilities;
using Audio;
using Ball;
using Cinemachine;
using Config;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using NaughtyAttributes;
using Player.Input;
using Rewired;
using UnityEngine;
using Utils;

namespace Player
{
    public class PlayerView : MonoBehaviour
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

        private Ability _ability;
        private int _angy;
        private int _playerId;
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

        public int Angy
        {
            get => _angy;
            private set
            {
                if (value > GameConfig.Instance.AngyValues.MaxAngy)
                {
                    _angy = GameConfig.Instance.AngyValues.MaxAngy;
                    ReachedMaxAngy?.Invoke();
                }
                else if (value < GameConfig.Instance.AngyValues.MinAngy)
                {
                    _angy = GameConfig.Instance.AngyValues.MinAngy;
                }
                else
                {
                    _angy = value;
                }

                AngyChanged?.Invoke(value);
            }
        }

        public int PlayerId
        {
            get => _playerId;
            set
            {
                _playerId = value;
                shooter.playerId = value;
            }
        }


        public Vector3 LastStillPosition { get; set; }

        [ShowNativeProperty]
        public Vector3 LastStandablePosition { get; set; }

        public Rewired.Player RewiredPlayer => ReInput.players.GetPlayer(PlayerId);

        public Ability Ability
        {
            get => _ability;
            set
            {
                Ability?.Wrap();
                _ability = value;

                if (value != null)
                {
                    PreviousAbility = value;
                }

                NewAbilitySet?.Invoke(this, value);
            }
        }

        public Ability PreviousAbility { get; set; }

        public float Knockback { get; set; }

        public float Drag
        {
            get => BallRigidbody.drag;
            set => BallRigidbody.drag = value;
        }

        public IPlayerInputs PlayerInputs { get; set; }

        public PlayerState PlayerState
        {
            get => playerState;
            set
            {
                Debug.Log(PlayerPreset.PlayerName.Color(PlayerPreset.PlayerColor) + "'s state was " + playerState +
                          " and became " + value);
                playerState = value;
            }
        }


        private void Awake()
        {
            shooter.SetPlayer(this);
        }

        private async void OnEnable()
        {
            ballBehaviour.BecameStill += OnBallBecameStill;
            shooter.Shot += OnBallShot;
            outOfBoundsCheck.WentOutOfBounds += OnWentOutOfBounds;

            await UniTask.WaitUntil(() => PlayerInputs != null);
            PlayerInputs.MenuButtonPressed += OnMenuButtonPressed;
        }

        private void OnDisable()
        {
            ballBehaviour.BecameStill -= OnBallBecameStill;
            shooter.Shot -= OnBallShot;
            outOfBoundsCheck.WentOutOfBounds -= OnWentOutOfBounds;
            PlayerInputs.MenuButtonPressed -= OnMenuButtonPressed;
        }

        public static event Action<PlayerView> OptionsMenuRequested;

        public event Action BecameStill;
        public event Action Shot;
        public event Action<PlayerView> WentOutOfBounds;
        public event Action ReachedMaxAngy;
        public event Action<int> AngyChanged;

        public static event Action<PlayerView, Ability> NewAbilitySet;

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

        public void AlterAngy(AngyEvent angyEvent)
        {
            switch (angyEvent)
            {
                case AngyEvent.HitBadObject:
                    Angy += GameConfig.Instance.AngyValues.HitBadObject;
                    break;
                case AngyEvent.FellOutOfTheMap:
                    Angy += GameConfig.Instance.AngyValues.FellOutOfTheMap;
                    break;
                case AngyEvent.AfterFellOutOfTheMapAndReachedMaxAngy:
                    Angy = GameConfig.Instance.AngyValues.AfterFellOutOfTheMapAndReachedMaxAngy;
                    break;
                case AngyEvent.ShotMade:
                    Angy += GameConfig.Instance.AngyValues.ShotMade;
                    break;
                case AngyEvent.HitSomeone:
                    Angy += GameConfig.Instance.AngyValues.PlayerHitSomeone;
                    break;
                case AngyEvent.GotHit:
                    Angy += GameConfig.Instance.AngyValues.PlayerGotHit;
                    break;
                case AngyEvent.MushroomHit:
                    Angy -= GameConfig.Instance.AngyValues.MushroomHit;
                    break;
            }
        }

        public UniTask ExplodeHideAndResetAngy()
        {
            Angy = GameConfig.Instance.AngyValues.MinAngy;

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

        public void ShouldPlayerActivate(int playerId)
        {
            shooter.ShouldPlayerActivate(playerId);
        }

        private void OnMenuButtonPressed()
        {
            OptionsMenuRequested?.Invoke(this);
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
        }

        private void OnWentOutOfBounds()
        {
            WentOutOfBounds?.Invoke(this);
        }

        private void OnBallShot()
        {
            Shot?.Invoke();
        }

        public void JumpIn(Vector3 endPosition, float jumpTime = 1f)
        {
            endPosition.y += BallRigidbody.GetComponent<SphereCollider>().radius;

            BallRigidbody.transform.DOMove(endPosition, jumpTime)
                .OnComplete(delegate
                {
                    BallRigidbody.velocity = Vector3.zero;
                    BallRigidbody.angularVelocity = Vector3.zero;
                });
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
    }
}
