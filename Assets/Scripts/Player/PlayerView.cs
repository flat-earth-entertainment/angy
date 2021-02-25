using System;
using System.Linq;
using Abilities;
using Ball;
using Cinemachine;
using Config;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using NaughtyAttributes;
using Player;
using Player.Input;
using Rewired;
using UnityEngine;

public class PlayerView : MonoBehaviour
{
    public static event Action<PlayerView> OptionsMenuRequested;

    public event Action BecameStill;
    public event Action Shot;
    public event Action<PlayerView> WentOutOfBounds;
    public event Action ReachedMaxAngy;
    public event Action<int> AngyChanged;

    public string Nickname { get; set; }

    public Color PlayerColor
    {
        get => _playerColor;
        set
        {
            _playerColor = value;

            var oldMaterials = Materials;

            oldMaterials[0].SetColor("Color_Primary", value);

            Materials = oldMaterials;
        }
    }

    public float ExpandPercent
    {
        get => skinnedMeshRenderer.GetBlendShapeWeight(0);
        set => skinnedMeshRenderer.SetBlendShapeWeight(0, value);
    }

    public Color FresnelColor
    {
        set
        {
            var oldMaterials = Materials;

            oldMaterials[0].SetColor("Fresnel_Color1", value);

            Materials = oldMaterials;
        }
    }

    public Material[] Materials
    {
        get => _shooter.lemming.transform.GetChild(0).GetChild(0).GetComponent<SkinnedMeshRenderer>().materials
            .Select(s => new Material(s)).ToArray();
        set => _shooter.lemming.transform.GetChild(0).GetChild(0).GetComponent<SkinnedMeshRenderer>().materials = value;
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
            else if (value != _angy)
            {
                AngyChanged?.Invoke(value);
                _angy = value;
            }
        }
    }

    public int PlayerId
    {
        get => _playerId;
        set
        {
            _playerId = value;
            _shooter.playerId = value;
        }
    }


    public Vector3 LastStillPosition { get; private set; }
    public GameObject Ball => _ballBehaviour.gameObject;
    public Rewired.Player RewiredPlayer => ReInput.players.GetPlayer(PlayerId);
    public Ability Ability { get; set; }

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
            Debug.Log(Nickname + "'s state was " + playerState + " and became " + value);
            playerState = value;
        }
    }

    [field: SerializeField]
    public Animator Animator { get; private set; }

    [field: SerializeField]
    public CinemachineVirtualCamera BallCamera { get; private set; }


    [field: SerializeField]
    public Rigidbody BallRigidbody { get; private set; }

    [SerializeField, ReadOnly]
    private PlayerState playerState;

    [SerializeField]
    private SkinnedMeshRenderer skinnedMeshRenderer;

    private BallBehaviour _ballBehaviour;
    private OutOfBoundsCheck _outOfBoundsCheck;
    private Shooter _shooter;
    private int _playerId;
    private int _angy;
    private Color _playerColor;
    private BoneLookAt _boneLookAt;

    public void Predict()
    {
        _shooter.predict();
    }

    public void SetIdleAnimation()
    {
        if (_shooter.lemmingAnim != null && !_shooter.lemmingAnim.Equals(null))
        {
            _ballBehaviour.ResetRotation();
            _shooter.lemmingAnim?.SetBool("isBall", false);
            _shooter.lemmingAnim?.SetBool("isKnockback", false);
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

    public void ExplodeHideAndResetAngy()
    {
        Angy = GameConfig.Instance.AngyValues.MinAngy;

        //TODO: Convert to proper animation
        BallRigidbody.GetComponent<Collider>().enabled = false;
        BallRigidbody.useGravity = false;

        var tweensByTarget = DOTween.TweensByTarget(BallRigidbody.transform);
        if (tweensByTarget != null && tweensByTarget.Count > 0)
        {
            BallRigidbody.GetComponent<Collider>().enabled = true;
            BallRigidbody.useGravity = true;
            Hide();
        }
        else
        {
            BallRigidbody.transform.localScale = Vector3.one;
            BallRigidbody.transform.DOPunchScale(Vector3.one * 5, 0.5f, 0).OnComplete(delegate
            {
                BallRigidbody.GetComponent<Collider>().enabled = true;
                BallRigidbody.useGravity = true;
                Hide();
            });
        }
    }

    public void ShouldPlayerActivate(int playerId)
    {
        _shooter.ShouldPlayerActivate(playerId);
    }

    private void Awake()
    {
        _shooter = GetComponentInChildren<Shooter>();
        if (!_shooter)
        {
            Debug.LogError("Can't find Shooter in children!");
        }

        _shooter.SetPlayer(this);

        _ballBehaviour = _shooter.BallStorage.GetComponent<BallBehaviour>();
        _outOfBoundsCheck = _shooter.BallStorage.GetComponent<OutOfBoundsCheck>();
        _boneLookAt = _shooter.BallStorage.GetComponentInChildren<BoneLookAt>();
    }

    private async void OnEnable()
    {
        _ballBehaviour.BecameStill += OnBallBecameStill;
        _shooter.Shot += OnBallShot;
        _outOfBoundsCheck.WentOutOfBounds += OnWentOutOfBounds;

        await UniTask.WaitUntil(() => PlayerInputs != null);
        PlayerInputs.MenuButtonPressed += OnMenuButtonPressed;
    }

    private void OnDisable()
    {
        _ballBehaviour.BecameStill -= OnBallBecameStill;
        _shooter.Shot -= OnBallShot;
        _outOfBoundsCheck.WentOutOfBounds -= OnWentOutOfBounds;
        PlayerInputs.MenuButtonPressed -= OnMenuButtonPressed;
    }

    private void OnMenuButtonPressed()
    {
        OptionsMenuRequested?.Invoke(this);
    }


    private void OnBallBecameStill()
    {
        var lastStillPosition = BallRigidbody.position;
        if (Physics.Raycast(BallRigidbody.position, Vector3.down, out var hit, 10f, LayerMask.GetMask("IgnoredMap")))
        {
            lastStillPosition = hit.point;
        }

        // lastStillPosition.y += BallRigidbody.GetComponent<SphereCollider>().radius;
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
        _shooter.BallStorage.SetActive(true);
    }

    public void Hide()
    {
        _shooter.BallStorage.SetActive(false);
    }

    public void SetControlsActive(bool toggle)
    {
        _shooter.enabled = toggle;
    }

    public void SetLookAtTrajectory(bool state)
    {
        _boneLookAt.enabled = state;
    }

    public void SetBallPosition(Vector3 position)
    {
        position.y += BallRigidbody.GetComponent<SphereCollider>().radius;
        BallRigidbody.transform.position = position;
    }
}