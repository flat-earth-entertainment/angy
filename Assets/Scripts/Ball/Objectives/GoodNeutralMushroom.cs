using System;
using System.Collections.Generic;
using Abilities;
using Audio;
using DG.Tweening;
using DinoFracture;
using Logic;
using MeshPhysics;
using Models.Berries;
using Player;
using UI;
using UnityEngine;

namespace Ball.Objectives
{
    public class GoodNeutralMushroom : MonoBehaviour
    {
        public enum AbilitySelect
        {
            None,
            IceBlockAbility,
            ExpandAbility,
            NoGravityAbility,
            FireDashAbility,
            RandomAbility
        }

        public List<GameObject> fruit;
        public GameObject goal;
        public ParticleSystem splatter;
        public Material baseFruitMat, baseFruitTopMat;

        public AbilitySelect mushroomDropAbility;

        // Owner Id 99 refers to no ownership, shouldn't be a problem unless we want 100 players.
        public int ownerId = 99, pointValue = 1;

        [HideInInspector]
        public bool mushroomDisabled;

        private GameObject _point;
        private PointController _pointController;

        private void Start()
        {
            _pointController = FindObjectOfType<PointController>();
            baseFruitMat = new Material(baseFruitMat);
            baseFruitTopMat = new Material(baseFruitTopMat);
        }

        private void OnTriggerEnter(Collider other)
        {
            Trigger(other);
        }

        private void OnTriggerExit(Collider other)
        {
            Trigger(other);
        }

        public static event Action<GameObject> BecameHole;
        public static event Action<GameObject> HoleSpawned;

        private void Trigger(Collider other)
        {
            if (other.CompareTag("Lemming"))
            {
                var hitId = other.transform.GetChild(0).GetComponent<Shooter>().playerId;
                if (ownerId != hitId && _point != null)
                {
                    AudioManager.PlaySfx(SfxType.PointReclaimed);
                }

                ownerId = hitId;
                if (_point != null)
                {
                    mushroomDisabled = true;
                }
                else
                {
                    _pointController.EnemyHit();
                    if (!GetComponentInChildren<FractureOnCollision>() && !GetComponentInChildren<FractureOnTrigger>())
                    {
                        transform.GetChild(0).gameObject.SetActive(false);
                    }

                    AudioManager.PlaySfx(SfxType.MushroomHit);


                    _point = Instantiate(fruit[0], transform.position + new Vector3(0, 1f, 0),
                        Quaternion.Euler(0, 0, 15));
                    _point.SetActive(false);
                    DOTween.Sequence().AppendInterval(.5f).AppendCallback(delegate
                    {
                        _point.SetActive(true);
                        splatter.Play(true);
                        other.transform.parent.GetComponent<PlayerView>().AlterAngy(AngyEvent.MushroomHit);
                    });

                    switch (mushroomDropAbility)
                    {
                        case AbilitySelect.None:
                            break;
                        case AbilitySelect.IceBlockAbility:
                            other.transform.GetChild(0).GetComponent<Shooter>().PlayerView.Ability =
                                new IceBlockAbility();
                            break;
                        case AbilitySelect.NoGravityAbility:
                            other.transform.GetChild(0).GetComponent<Shooter>().PlayerView.Ability =
                                new NoGravityAbility();
                            break;
                        case AbilitySelect.ExpandAbility:
                            other.transform.GetChild(0).GetComponent<Shooter>().PlayerView.Ability =
                                new ExpandAbility();
                            break;
                        case AbilitySelect.FireDashAbility:
                            other.transform.GetChild(0).GetComponent<Shooter>().PlayerView.Ability =
                                new FireDashAbility();
                            break;
                        case AbilitySelect.RandomAbility:
                        {
                            var newAbility = Ability.RandomAbility();
                            var player = other.transform.GetChild(0).GetComponent<Shooter>().PlayerView;
                            player.Ability = newAbility;
                            FindObjectOfType<UiController>().DoSlotMachineFor(player, newAbility);
                            break;
                        }
                    }
                }

                _pointController.UpdateScore();
                if (!_point.GetComponent<Renderer>())
                {
                    foreach (var item in _point.GetComponentsInChildren<Renderer>())
                    {
                        item.materials = new[] {baseFruitMat};

                        item.materials[0].SetColor("BerryColor",
                            other.transform.GetChild(0).GetComponent<Shooter>().PlayerView.PlayerPreset.PlayerColor);
                        item.gameObject.GetComponent<BerryAnim>().BerryHit();
                    }
                }
                else
                {
                    _point.GetComponent<BerryAnim>().BerryHit();
                    _point.GetComponent<Renderer>().materials[0].SetColor("BerryColor",
                        other.transform.GetChild(0).GetComponent<Shooter>().PlayerView.PlayerPreset.PlayerColor);
                }
            }
        }

        public void SpawnGoal()
        {
            pointValue++;
            if (Physics.Raycast(transform.position + Vector3.up, Vector3.down, out var hit, 50f,
                LayerMask.GetMask("IgnoredMap")))
            {
                var hitObject = hit.collider.gameObject;
                print($"HIT {hitObject.name} {LayerMask.LayerToName(hitObject.layer)}");
                var hole = Instantiate(goal, hitObject.transform.position, Quaternion.identity);
                HoleSpawned?.Invoke(hole);
                BecameHole?.Invoke(hitObject);
                AudioManager.PlaySfx(SfxType.HoleAppeared);
                Destroy(hitObject);
                transform.GetChild(0).gameObject.SetActive(false);
            }
            else
            {
                Debug.Log("Error, ground not found");
            }

            GetComponent<Renderer>().enabled = false;
            _point = Instantiate(fruit[fruit.Count - 1], transform.position + new Vector3(0, 1, 0),
                Quaternion.identity);
        }
    }
}