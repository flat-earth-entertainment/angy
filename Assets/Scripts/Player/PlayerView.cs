using System;
using Ball;
using Cinemachine;
using Config;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using NaughtyAttributes;
using Player;
using UnityEngine;

public class PlayerView : MonoBehaviour
{
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
            var lemmingRenderer =
                _shooter.lemming.transform.GetChild(0).GetChild(0).GetComponent<SkinnedMeshRenderer>();

            Debug.Log(lemmingRenderer.materials[0].name);

            var newColorMaterial =
                new Material(lemmingRenderer.materials[0]);

            newColorMaterial.SetColor("Color_Primary", value);

            var oldMaterials = lemmingRenderer.materials;
            oldMaterials[0] = newColorMaterial;
            lemmingRenderer.materials = oldMaterials;
        }
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
    public CinemachineVirtualCamera BallCamera { get; private set; }

    [SerializeField]
    private Rigidbody ball;

    [SerializeField, ReadOnly]
    private PlayerState playerState;

    private BallBehaviour _ballBehaviour;
    private OutOfBoundsCheck _outOfBoundsCheck;
    private Shooter _shooter;
    private int _playerId;
    private int _angy;
    private Color _playerColor;

    public void Predict()
    {
        _shooter.predict();
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
        }
    }

    public void ExplodeHideAndResetAngy()
    {
        Angy = GameConfig.Instance.AngyValues.MinAngy;

        //TODO: Convert to proper animation
        ball.GetComponent<Collider>().enabled = false;
        ball.useGravity = false;
        ball.transform.DOPunchScale(Vector3.one * 5, 0.5f, 0).OnComplete(delegate
        {
            ball.GetComponent<Collider>().enabled = true;
            ball.useGravity = true;
            Hide();
        });
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
    }

    private void OnEnable()
    {
        _ballBehaviour.BecameStill += OnBallBecameStill;
        _shooter.Shot += OnBallShot;
        _outOfBoundsCheck.WentOutOfBounds += OnWentOutOfBounds;
    }

    private void OnDisable()
    {
        _ballBehaviour.BecameStill -= OnBallBecameStill;
        _shooter.Shot -= OnBallShot;
        _outOfBoundsCheck.WentOutOfBounds -= OnWentOutOfBounds;
    }

    private void OnBallBecameStill()
    {
        LastStillPosition = ball.position;
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
        endPosition.y += ball.GetComponent<SphereCollider>().radius;

        ball.transform.DOMove(endPosition, jumpTime)
            .OnComplete(delegate
            {
                ball.velocity = Vector3.zero;
                ball.angularVelocity = Vector3.zero;
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

    public void SetBallPosition(Vector3 position)
    {
        position.y += ball.GetComponent<SphereCollider>().radius;
        ball.transform.position = position;
    }
}