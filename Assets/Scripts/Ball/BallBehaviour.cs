using System;
using System.Collections;
using UnityEngine;

namespace Ball
{
    public class BallBehaviour : MonoBehaviour
    {
        public bool inMotion;

        private Rigidbody _rb;
        private Shooter _shooter;
        private Vector3 _spinDirection;
        private float _timer, _stopTimer;
        private Transform _velocityDirection, _offsetPointer, _offsetHolder;
        private float _windDown = 1;

        private void Start()
        {
            _shooter = GetComponentInChildren<Shooter>();
            _rb = GetComponent<Rigidbody>();

            _velocityDirection =
                new GameObject("Velocity direction of player " + _shooter.playerId).GetComponent<Transform>();
            _velocityDirection.parent = transform;

            // All of these would be unnecessary if i for the life of me could just turn a vector correctly
            _offsetPointer = new GameObject("Offset pointer").GetComponent<Transform>();
            _offsetPointer.parent = _velocityDirection;
            _offsetHolder = new GameObject("Offset holder").GetComponent<Transform>();
            _offsetHolder.parent = _velocityDirection;
            _offsetPointer.position = _offsetHolder.position = new Vector3(0, 0, 0);
        }

        private void Update()
        {
            if (inMotion)
            {
                _timer += Time.deltaTime;
                if (_rb.velocity.magnitude < 0.2f && _timer > 0.5f)
                {
                    _stopTimer += Time.deltaTime;
                    if (_stopTimer > 1)
                    {
                        BecameStill?.Invoke();
                        _timer = 0;
                        inMotion = false;
                        _shooter.activateShootingRetinae = true;
                        _rb.velocity = new Vector3(0, 0, 0);
                        _rb.angularVelocity = new Vector3(0, 0, 0);
                        _shooter.lemmingAnim.SetBool("isBall", false);
                        _shooter.lemmingAnim.SetBool("isKnockback", false);
                        transform.rotation = Quaternion.Euler(180, 0, 0);
                        _shooter.lemming.transform.rotation =
                            Quaternion.Euler(0, _shooter.horSnap * _shooter.horSnapAngle + 180, 0);
                    }
                }
                else
                {
                    _stopTimer = 0;
                }

                _velocityDirection.transform.position = transform.position;
                _velocityDirection.LookAt(_rb.velocity + transform.position, Vector3.up);
            }

            if (_rb.velocity.magnitude > 0.2f && !inMotion)
            {
                inMotion = true;
            }
        }

        public event Action BecameStill;

        public IEnumerator BallSpin(Vector3 spinDir)
        {
            yield return null;
            _spinDirection = spinDir;
            _offsetHolder.localPosition = _spinDirection;
            _offsetPointer.LookAt(_offsetHolder, Vector3.up);

            var direction = _offsetPointer.TransformDirection(_offsetPointer.position);
            _rb.AddTorque(direction * 20, ForceMode.Impulse);
            while (inMotion)
            {
                direction = _offsetPointer.TransformDirection(_offsetPointer.position);
                _rb.AddTorque(direction * (Time.deltaTime * _windDown), ForceMode.Impulse);
                //Quaternion rotation = Quaternion.Euler(0,velocityDirection.eulerAngles.y,0);
                //Vector3 rotateVector = rotation * spinDirection;
                //Vector3 rotateVector = velocityDirection.forward + spinDirection; 
                //Vector3 rotateVector = Quaternion.AngleAxis(velocityDirection.localEulerAngles.y, velocityDirection.up) * spinDirection;
                //Vector3 rotateVector = velocityDirection.localEulerAngles * spinDirection;
                //Vector3 rotateVector = Quaternion.Euler(0,velocityDirection.localRotation.y,0) * spinDirection;
                if (_windDown > 0)
                {
                    _windDown -= Time.deltaTime;
                }
                else
                {
                    _windDown = 0;
                }

                yield return null;
            }
        }
    }
}