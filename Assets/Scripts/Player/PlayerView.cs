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

    public string Nickname => name;

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

    // [SerializeField]
    private Rigidbody _ball => _shooter.ballStorage.GetComponent<Rigidbody>();

    private BallBehaviour _ballBehaviour;
    private OutOfBoundsCheck _outOfBoundsCheck;
    private Shooter _shooter;
    private int _playerId;
    private int _angy;

    [SerializeField, ReadOnly]
    private PlayerState playerState;

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

    public void ExplodeAndHide()
    {
        Angy = GameConfig.Instance.AngyValues.MinAngy;

        _ball.GetComponent<Collider>().enabled = false;
        _ball.useGravity = false;
        _ball.transform.DOPunchScale(Vector3.one * 5, 0.5f, 0).OnComplete(delegate
        {
            _ball.GetComponent<Collider>().enabled = true;
            _ball.useGravity = true;
            Hide();
        });
    }

    public void ShouldPlayerActivate(int playerId)
    {
        _shooter.ShouldPlayerActivate(playerId);
    }

    private void OnBallBecameStill()
    {
        LastStillPosition = _ball.position;
        BecameStill?.Invoke();
    }

    private void OnBallShot()
    {
        Shot?.Invoke();
    }

    private async void OnEnable()
    {
        await UniTask.WaitUntil(() => _ballBehaviour != null);

        _ballBehaviour.BecameStill += OnBallBecameStill;
        _shooter.Shot += OnBallShot;
        _outOfBoundsCheck.WentOutOfBounds += OnWentOutOfBounds;
    }

    private void OnWentOutOfBounds()
    {
        WentOutOfBounds?.Invoke(this);
    }

    private void OnDisable()
    {
        _ballBehaviour.BecameStill -= OnBallBecameStill;
        _shooter.Shot -= OnBallShot;
        _outOfBoundsCheck.WentOutOfBounds -= OnWentOutOfBounds;
    }

    private async void Awake()
    {
        _shooter = GetComponentInChildren<Shooter>();
        if (!_shooter)
        {
            Debug.LogError("Can't find Shooter in children!");
        }

        _shooter.SetPlayer(this);

        if (_shooter.ballStorage == null || _shooter.Equals(null))
        {
            await UniTask.WaitUntil(() => _shooter.ballStorage != null);
            BallCamera.Follow = _shooter.ballStorage.transform;
        }

        _ballBehaviour = _shooter.ballStorage.GetComponent<BallBehaviour>();
        _outOfBoundsCheck = _shooter.ballStorage.GetComponent<OutOfBoundsCheck>();
    }

    public void JumpIn(Vector3 endPosition, float jumpTime = 1f)
    {
        endPosition.y += _ball.GetComponent<SphereCollider>().radius;

        _ball.constraints = RigidbodyConstraints.None;

        _ball.transform.DOMove(endPosition, jumpTime)
            .OnComplete(delegate
            {
                _ball.velocity = Vector3.zero;
                _ball.angularVelocity = Vector3.zero;
            });
    }

    public void Show()
    {
        _shooter.ballStorage.SetActive(true);
    }

    public async void Hide()
    {
        if (_shooter.ballStorage == null || _shooter.Equals(null))
        {
            await UniTask.WaitUntil(() => _shooter.ballStorage != null);
        }

        _shooter.ballStorage.SetActive(false);
    }

    public void SetControlsActive(bool toggle)
    {
        //_shooter.active = toggle;
    }

    public void SetBallPosition(Vector3 position)
    {
        position.y += _ball.GetComponent<SphereCollider>().radius;
        _ball.transform.position = position;
    }
}