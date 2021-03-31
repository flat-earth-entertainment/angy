﻿using System;
using System.Collections;
using Audio;
using Coffee.UIExtensions;
using Config;
using Logic;
using Player;
using Rewired;
using UnityEngine;
using UnityEngine.UI;
using Utils;

namespace Ball
{
    public class Shooter : MonoBehaviour
    {
        public ParticleSystem dust;

        public GameObject lemming;

        [HideInInspector]
        public Animator lemmingAnim;

        public int playerId = 1;

        public GameObject firePoint;
        public GameObject ballPrefab;
        public float power;
        public float rotationSpeed;

        [Range(180, 270)]
        [Header("Must be a multiplicative of 5")]
        public int xMinAngle = 180, xMaxAngle = 270;

        [SerializeField]
        private GameObject ballStorage;

        public bool activateShootingRetinae = true, active = true;

        public float vertSnap = 180, horSnap = 90;

        // how many degrees the shooting retinae should snap. MUST add up to 360
        public int vertSnapAngle = 5, horSnapAngle = 5, greatSnapAngle = 30;
        public float snapCooldown = 0.2f;

        [SerializeField]
        private Rigidbody rb;

        public float forcePercent = 1;

        [HideInInspector]
        public Slider powerSlider;

        // Rotation cooldown modifier end
        // Ball Spin
        public Vector3 spinDirection = new Vector3(0, 0, 0);
        public float spinIncrement = 0.2f;

        private Vector3 _currentPosition;
        private Quaternion _currentRotation;
        private bool _movedRet, _forcePercentBool, _maxPower;
        private Rewired.Player _rewiredPlayer;
        private float _vertSnapCooldownTimer, _horSnapCooldownTimer;

        private Transform _spinIndicator;

        // Rotation cooldown modifier
        private float _vertRotTimeMultiplier = 1, _horRotTimeMultiplier = 1;
        private float _vertSnapMultiplier = 0.1f, _horSnapMultiplier = 0.1f;

        public PlayerView PlayerView { get; private set; }

        private void Start()
        {
            _rewiredPlayer = ReInput.players.GetPlayer(playerId);

            _currentPosition = transform.position;
            _currentRotation = transform.rotation;
            //predict(); Doesn't work atm since i had to move the physics scene creation by one frame

            ballStorage = transform.parent.gameObject;
            rb = ballStorage.GetComponent<Rigidbody>();

            ShouldPlayerActivate(playerId);

            // MUST BE IMPROVED
            powerSlider = GameObject.FindGameObjectWithTag("TEMPFINDSLIDER").transform.GetChild(0).GetChild(0)
                .GetComponent<Slider>();
            _spinIndicator = powerSlider.transform.parent.GetChild(6).GetChild(2).GetChild(0).transform;

            lemmingAnim = lemming.GetComponentInChildren<Animator>();

            FindObjectOfType<LemmingInitialDirection>().RotateLemming(this);
        }


        private void Update()
        {
            if (activateShootingRetinae && active)
            {
                // Vertical movement controls
                var vertical = _rewiredPlayer.GetAxis("Move Vertical");
                if (_vertSnapCooldownTimer <= 0)
                {
                    // delays snapping intervals
                    if (Mathf.Abs(vertical) > 0.66f)
                    {
                        vertSnap += Mathf.Sign(vertical) * _vertSnapMultiplier;
                        vertSnap = Mathf.Clamp(vertSnap, xMinAngle / vertSnapAngle, xMaxAngle / vertSnapAngle);
                        _vertSnapCooldownTimer = snapCooldown;
                        _movedRet = true;
                    }
                }
                else
                {
                    _vertSnapCooldownTimer -= Time.deltaTime;
                }

                if (Mathf.Abs(vertical) > 0.66f)
                {
                    if (_vertRotTimeMultiplier < 0.95f)
                    {
                        _vertSnapMultiplier = 1;
                    }

                    if (_vertRotTimeMultiplier < 0.67f)
                    {
                        _vertSnapMultiplier = 2;
                    }

                    if (_vertRotTimeMultiplier < 0.33f)
                    {
                        _vertSnapMultiplier = 3;
                    }

                    _vertRotTimeMultiplier -= Time.deltaTime / 4;
                }
                else
                {
                    _vertRotTimeMultiplier = 1;
                    _vertSnapMultiplier = 0.1f;
                }

                // Horizontal movement controls
                var horizontal = _rewiredPlayer.GetAxis("Move Horizontal");
                if (_horSnapCooldownTimer <= 0)
                {
                    // delays snapping intervals
                    if (Mathf.Abs(horizontal) > 0.66f)
                    {
                        horSnap += Mathf.Sign(horizontal) * _horSnapMultiplier;
                        _horSnapCooldownTimer = snapCooldown;
                        _movedRet = true;
                        lemmingAnim.SetBool("isRot", true);
                        AudioManager.PlaySfx(SfxType.LemmingRotate);
                    }
                    else
                    {
                        lemmingAnim.SetBool("isRot", false);
                    }

                    if (_rewiredPlayer.GetButtonDown("SnapLeft"))
                    {
                        horSnap -= greatSnapAngle / horSnapAngle;
                        _horSnapCooldownTimer = snapCooldown;
                        _movedRet = true;
                        AudioManager.PlaySfx(SfxType.LemmingRotate);
                    }

                    if (_rewiredPlayer.GetButtonDown("SnapRight"))
                    {
                        horSnap += greatSnapAngle / horSnapAngle;
                        _horSnapCooldownTimer = snapCooldown;
                        _movedRet = true;
                        AudioManager.PlaySfx(SfxType.LemmingRotate);
                    }
                }
                else
                {
                    _horSnapCooldownTimer -= Time.deltaTime;
                }

                if (Mathf.Abs(horizontal) > 0.66f)
                {
                    if (_horRotTimeMultiplier < 0.95f)
                    {
                        _horSnapMultiplier = 1;
                    }

                    if (_horRotTimeMultiplier < 0.80f)
                    {
                        _horSnapMultiplier = 2;
                    }

                    if (_horRotTimeMultiplier < 0.6f)
                    {
                        _horSnapMultiplier = 3;
                    }

                    if (_horRotTimeMultiplier < 0.4f)
                    {
                        _horSnapMultiplier = 4;
                    }

                    if (_horRotTimeMultiplier < 0.2f)
                    {
                        _horSnapMultiplier = 5;
                    }

                    if (_horRotTimeMultiplier < 0f)
                    {
                        _horSnapMultiplier = 7;
                    }

                    _horRotTimeMultiplier -= Time.deltaTime / 4;
                }
                else
                {
                    _horRotTimeMultiplier = 1;
                    _horSnapMultiplier = 0.1f;
                }

                if (_movedRet)
                {
                    transform.rotation = Quaternion.Euler(vertSnap * vertSnapAngle, horSnap * horSnapAngle, 0);

                    // Draw prediction curve if player has moved curve
                    if (_currentRotation != transform.rotation)
                    {
                        Predict();
                        _currentRotation = transform.rotation;
                    }

                    if (_currentPosition != transform.position)
                    {
                        Predict();
                        _currentPosition = transform.position;
                    }

                    _movedRet = false;
                }

                // Shoot the ball
                if (_rewiredPlayer.GetButtonDown("Confirm"))
                {
                    StartCoroutine(nameof(PreShot));
                }
            }
        }

        public event Action Shot;

        // \/ \/ \/ REMOVE, ONLY FOR TESTING \/ \/ \/
        // /\ /\ /\ REMOVE, ONLY FOR TESTING /\ /\ /\

        // Max power particle
        public void SetBallFormActive(bool state)
        {
            lemmingAnim.SetBool("isKnockback", state);
        }

        public void SetPlayer(PlayerView playerView)
        {
            PlayerView = playerView;
        }

        private void Shoot()
        {
            rb.constraints = RigidbodyConstraints.None;
            rb.AddForce(CalculateForce(), ForceMode.Impulse);
            ballStorage.GetComponent<BallBehaviour>().inMotion = true;

            Shot?.Invoke();
            if (spinDirection.magnitude > 0)
            {
                StartCoroutine(ballStorage.GetComponent<BallBehaviour>().BallSpin(spinDirection));
            }

            AudioManager.PlaySfx(SfxType.LemmingLaunch);
            AudioManager.PlaySfx(SfxType.LemmingLaunchVoice);

            Destroy(PredictionManager.Instance.indicatorHolder);
        }

        public Vector3 CalculateForce()
        {
            return transform.forward * (power * forcePercent);
        }

        private IEnumerator PreShot()
        {
            activateShootingRetinae = false;
            lemmingAnim.SetBool("isBall", true);

            // change spin and tilt goes here //
            powerSlider.transform.parent.gameObject.SetActive(true);
            //yield return StartCoroutine("CalculateSpin");


            // Power Goes Here //
            yield return StartCoroutine(nameof(CalculateShootForce));
            if (_maxPower)
            {
                yield return new WaitForSeconds(1);
            }

            powerSlider.transform.parent.gameObject.SetActive(false);
            // Power Ends Here //

            // Shoot the ball
            Shoot();
            CreateDust();
            DisableRetinae();
            active = false;
            _maxPower = false;

            // Wait 1 frame and Reset system
            yield return null;
            forcePercent = 1;
            activateShootingRetinae = true;
        }

        private IEnumerator CalculateSpin()
        {
            yield return null;
            var timer = 0.2f;
            while (!_rewiredPlayer.GetButtonDown("Confirm"))
            {
                var horizontal = _rewiredPlayer.GetAxis("Move Horizontal");
                var vertical = _rewiredPlayer.GetAxis("Move Vertical");
                if (timer > 0.2f)
                {
                    if (Mathf.Abs(horizontal) > 0)
                    {
                        spinDirection.x += Mathf.Sign(horizontal) * spinIncrement;
                        timer = 0;
                        if (spinDirection.x > 1)
                        {
                            spinDirection.x = 1;
                        }

                        if (spinDirection.x < -1)
                        {
                            spinDirection.x = -1;
                        }
                    }

                    if (Mathf.Abs(vertical) > 0)
                    {
                        spinDirection.z += Mathf.Sign(vertical) * spinIncrement;
                        timer = 0;
                        if (spinDirection.z > 1)
                        {
                            spinDirection.z = 1;
                        }

                        if (spinDirection.z < -1)
                        {
                            spinDirection.z = -1;
                        }
                    }

                    _spinIndicator.localPosition = new Vector3(spinDirection.x, spinDirection.z, 0);
                }

                timer += Time.deltaTime;
                yield return null;
            }
        }

        private IEnumerator CalculateShootForce()
        {
            var currentAngy = Mathf.Lerp(1, 0.25f,
                (float) GameManager.CurrentTurnPlayer.Angy / GameConfig.Instance.AngyValues.MaxAngy);
            forcePercent = 0;
            yield return null;
            while (!_rewiredPlayer.GetButtonDown("Confirm") && forcePercent >= 0)
            {
                powerSlider.value = forcePercent;
                if (!_forcePercentBool)
                {
                    forcePercent += Time.deltaTime / 3 / currentAngy;
                    if (forcePercent >= 1.033f)
                    {
                        _forcePercentBool = true;
                    }
                }
                else
                {
                    forcePercent -= Time.deltaTime / 3 / currentAngy;
                }

                yield return null;
            }

            if (forcePercent >= 1)
            {
                // Play full power particle
                GameObject.FindGameObjectWithTag("TEMPFINDSLIDER").GetComponentInChildren<UIParticle>().Play();
                AudioManager.PlaySfx(SfxType.PowerMeterMax);
                _maxPower = true;
            }

            if (forcePercent < 0)
            {
                forcePercent = 0;
            }

            if (forcePercent > 1)
            {
                forcePercent = 1;
            }

            _forcePercentBool = false;
            StopCoroutine(nameof(CalculateShootForce));
        }

        private void DisableRetinae()
        {
            activateShootingRetinae = false;
        }

        public void Predict()
        {
            transform.parent.rotation = Quaternion.Euler(0, 0, 0);
            transform.rotation = Quaternion.Euler(vertSnap * vertSnapAngle, horSnap * horSnapAngle, 0);
            PredictionManager.Instance.Predict(ballPrefab, firePoint.transform.position, CalculateForce());
            lemming.transform.rotation = Quaternion.Euler(0, horSnap * horSnapAngle + 180, 0);
        }

        public void ShouldPlayerActivate(int playerToActivate)
        {
            // Use this to define what player can move.
            if (playerToActivate == playerId)
            {
                active = true;
                rb.constraints = RigidbodyConstraints.FreezeAll; // Freeze player
                transform.rotation =
                    Quaternion.Euler(vertSnap * vertSnapAngle, horSnap * horSnapAngle,
                        0); // Set aiming retinae to actual shooting direction
                Predict();
            }
            else
            {
                active = false;
            }
        }

        private void CreateDust()
        {
            dust.Play();
        }
    }
}