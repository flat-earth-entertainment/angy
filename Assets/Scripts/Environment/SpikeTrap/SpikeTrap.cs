using System.Collections;
using System.Collections.Generic;
using Audio;
using UnityEngine;

namespace Environment.SpikeTrap
{
    public class SpikeTrap : MonoBehaviour
    {
        public enum TriggerMethod
        {
            Trigger,
            Timed
        }

        public List<GameObject> players = new List<GameObject>();
        public float triggerDelay = 1;
        public float timedInterval = 8;
        public float knockbackForce = 10;
        public TriggerMethod triggerType;
        public GameObject blood;
        private bool _enumActive;
        private SkinnedMeshRenderer _spike;

        private float _timedTimer, _enumTimer;

        private void Start()
        {
            _spike = GetComponentInChildren<SkinnedMeshRenderer>();
        }

        private void Update()
        {
            switch (triggerType)
            {
                case TriggerMethod.Trigger:
                {
                    // Triggers when someone is inside the trigger
                    if (!_enumActive && players.Count > 0)
                    {
                        StartCoroutine(nameof(Activate));
                    }

                    break;
                }
                case TriggerMethod.Timed:
                {
                    // Trigger after x amount of time
                    _timedTimer += Time.deltaTime;
                    if (_timedTimer > timedInterval && !_enumActive)
                    {
                        StartCoroutine(nameof(Activate));
                        _timedTimer = 0;
                    }

                    break;
                }
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Lemming"))
            {
                players.Add(other.gameObject);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Lemming"))
            {
                players.Remove(other.gameObject);
            }
        }

        private IEnumerator Activate()
        {
            // Handles the blendshape animation of the spikes
            AudioManager.PlaySfx(SfxType.SpikeTrapPrepare);
            _enumActive = true;
            while (true)
            {
                // Extends the spike slightly to show that they have been TRIGGURRRED
                _enumTimer += Time.deltaTime * 10;
                _spike.SetBlendShapeWeight(0, Mathf.Lerp(100, 75, _enumTimer));
                if (_enumTimer >= 1)
                {
                    _enumTimer = 0;
                    break;
                }

                yield return null;
            }

            yield return new WaitForSeconds(triggerDelay); // Waits a while
            while (true)
            {
                // Retracts the spikes to prepare them for maximum extension
                _enumTimer += Time.deltaTime * 10;
                _spike.SetBlendShapeWeight(0, Mathf.Lerp(75, 100, _enumTimer));
                if (_enumTimer >= 1)
                {
                    _enumTimer = 0;
                    break;
                }

                yield return null;
            }

            AudioManager.PlaySfx(SfxType.SpikeTrap);

            while (true)
            {
                // Extends the spikes to the max
                _enumTimer += Time.deltaTime * 10;
                _spike.SetBlendShapeWeight(0, Mathf.Lerp(100, 0, _enumTimer));
                if (_enumTimer >= 1)
                {
                    // Applies damage to players
                    var plcount = players.Count;
                    if (plcount > 0)
                    {
                        for (var i = 0; i < plcount; i++)
                        {
                            if (players[i] != null)
                            {
                                Hit(players[i]);
                            }
                        }
                    }
                    // Apply full extention sound

                    _enumTimer = 0;
                    break;
                }

                yield return null;
            }

            yield return new WaitForSeconds(0.1f);
            while (true)
            {
                // Slowly retract the spikes
                _enumTimer += Time.deltaTime * 1;
                _spike.SetBlendShapeWeight(0, Mathf.Lerp(0, 100, _enumTimer));
                if (_enumTimer >= 1)
                {
                    _enumTimer = 0;
                    break;
                }

                yield return null;
            }

            _enumActive = false;
        }

        private void Hit(GameObject lemming)
        {
            if (lemming.activeSelf)
            {
                // Check if the player is active

                // applies Knockback
                lemming.GetComponent<Rigidbody>().velocity =
                    (lemming.GetComponent<Rigidbody>().velocity.normalized + Vector3.up).normalized * knockbackForce;
                // Applies blood
                var bld = Instantiate(blood, lemming.transform.position - new Vector3(0, -0.5f, 0),
                    Quaternion.Euler(90, 0, 0));
                bld.GetComponent<ParticleSystem>().Play();
                Destroy(bld, 8f);
                // Apply sound

                // Apply Angy
            }
            else
            {
                // If player is not active, remove them
                players.Remove(lemming);
            }
        }
    }
}