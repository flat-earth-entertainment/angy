using System;
using Cinemachine;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using NaughtyAttributes;
using UnityEngine;

public class PlayerView : MonoBehaviour
{
    public event Action BecameStill;
    public event Action Shot;

    public int PlayerId { get; set; }

    [field: SerializeField, ReadOnly]
    public PlayerState PlayerState { get; set; }

    [field: SerializeField]
    public CinemachineVirtualCamera BallCamera { get; private set; }

    // [SerializeField]
    private Rigidbody _ball => _shooter.ballStorage.GetComponent<Rigidbody>();

    private BallBehaviour _ballBehaviour;
    private Shooter _shooter;

    public void ShouldPlayerActivate(int playerId)
    {
        _shooter.ShouldPlayerActivate(playerId);
    }

    private void OnBallBecameStill()
    {
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
    }

    private void OnDisable()
    {
        _ballBehaviour.BecameStill -= OnBallBecameStill;
        _shooter.Shot -= OnBallShot;
    }

    private async void Awake()
    {
        _shooter = GetComponentInChildren<Shooter>();
        if (!_shooter)
        {
            Debug.LogError("Can't find Shooter in children!");
        }

        if (_shooter.ballStorage == null || _shooter.Equals(null))
        {
            await UniTask.WaitUntil(() => _shooter.ballStorage != null);
            BallCamera.Follow = _shooter.ballStorage.transform;
        }

        _ballBehaviour = _shooter.ballStorage.GetComponent<BallBehaviour>();
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
        _shooter.active = toggle;
    }

    public void SetBallPosition(Vector3 position)
    {
        position.y += _ball.GetComponent<SphereCollider>().radius;
        _ball.transform.position = position;
    }
}