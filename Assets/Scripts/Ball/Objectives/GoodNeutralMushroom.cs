using System;
using System.Collections.Generic;
using Abilities;
using Audio;
using DG.Tweening;
using DinoFracture;
using Player;
using UI;
using UnityEngine;

public class GoodNeutralMushroom : MonoBehaviour
{
    public static event Action<GameObject> BecameHole;
    public static event Action<GameObject> HoleSpawned;
    
    public List<GameObject> fruit;
    private GameObject point;
    public GameObject goal;
    private PointController pointController;
    public ParticleSystem splatter;
    public Material baseFruitMat, baseFruitTopMat;
    public enum AbilitySelect{none,IceBlockAbility,ExpandAbility,NoGravityAbility,FireDashAbility,RandomAbility}
    public AbilitySelect mushroomDropAbility;
    // Owner Id 99 refers to no ownership, shouldn't be a problem unless we want 100 players.
    public int ownerId = 99, pointValue = 1;
    [HideInInspector]
    public bool mushroomDisabled;
    private void Start() {
        pointController = GameObject.FindObjectOfType<PointController>();
        baseFruitMat = new Material(baseFruitMat);
        baseFruitTopMat = new Material(baseFruitTopMat);
    }
    private void OnTriggerEnter(Collider other) {
        Trigger(other);
    }
    private void OnTriggerExit(Collider other) {
        Trigger(other);
    }
    void Trigger(Collider other){
        if(other.tag == "Lemming"){
            int hitId = other.transform.GetChild(0).GetComponent<Shooter>().playerId;
            if (ownerId != hitId && point != null)
            {
                AudioManager.PlaySfx(SfxType.PointReclaimed);
            }
            ownerId = hitId;
            if(point != null){
                mushroomDisabled = true;
            }else{
                pointController.EnemyHit(hitId);
                if (!GetComponentInChildren<FractureOnCollision>() && !GetComponentInChildren<FractureOnTrigger>())
                {
                    transform.GetChild(0).gameObject.SetActive(false);
                }

                AudioManager.PlaySfx(SfxType.MushroomHit);


                point = Instantiate(fruit[0], transform.position + new Vector3(0, 1f, 0),
                    Quaternion.Euler(0, 0, 15));
                point.SetActive(false);
                DOTween.Sequence().AppendInterval(.5f).AppendCallback(delegate
                {
                    point.SetActive(true);
                    splatter.Play(true);
                    other.transform.parent.GetComponent<PlayerView>().AlterAngy(AngyEvent.MushroomHit);
                });

                if(mushroomDropAbility == AbilitySelect.none){

                }else if(mushroomDropAbility == AbilitySelect.IceBlockAbility){
                    other.transform.GetChild(0).GetComponent<Shooter>().PlayerView.Ability = new Abilities.IceBlockAbility();
                }else if(mushroomDropAbility == AbilitySelect.NoGravityAbility){
                    other.transform.GetChild(0).GetComponent<Shooter>().PlayerView.Ability = new Abilities.NoGravityAbility();
                }else if(mushroomDropAbility == AbilitySelect.ExpandAbility){
                    other.transform.GetChild(0).GetComponent<Shooter>().PlayerView.Ability = new Abilities.ExpandAbility();
                }else if(mushroomDropAbility == AbilitySelect.FireDashAbility){
                    other.transform.GetChild(0).GetComponent<Shooter>().PlayerView.Ability = new Abilities.FireDashAbility();
                }else if(mushroomDropAbility == AbilitySelect.RandomAbility){
                    var newAbility = Ability.RandomAbility();
                    var player = other.transform.GetChild(0).GetComponent<Shooter>().PlayerView;
                    player.Ability = newAbility;
                    FindObjectOfType<UiController>().DoSlotMachineFor(player, newAbility);
                }
                
            }
            pointController.UpdateScore();
            if(!point.GetComponent<Renderer>()){
                foreach (Renderer item in point.GetComponentsInChildren<Renderer>())
                {
                    item.materials = new Material[1] {baseFruitMat};

                    item.materials[0].SetColor("BerryColor", other.transform.GetChild(0).GetComponent<Shooter>().PlayerView.PlayerColor);
                    item.gameObject.GetComponent<BerryAnim>().BerryHit();
                }
            }else{
                point.GetComponent<BerryAnim>().BerryHit();
                point.GetComponent<Renderer>().materials[0].SetColor("BerryColor", other.transform.GetChild(0).GetComponent<Shooter>().PlayerView.PlayerColor);
            }
        }
    }
    public void SpawnGoal(){
        pointValue++;
        RaycastHit hit;
        if(Physics.Raycast(transform.position + Vector3.up, Vector3.down, out hit, 50f, LayerMask.GetMask("IgnoredMap"))){
            GameObject hitObject = hit.collider.gameObject;
            print($"HIT {hitObject.name} {LayerMask.LayerToName(hitObject.layer)}");
            var hole = Instantiate(goal, hitObject.transform.position, Quaternion.identity);
            HoleSpawned?.Invoke(hole);
            BecameHole?.Invoke(hitObject);
            AudioManager.PlaySfx(SfxType.HoleAppeared);
            Destroy(hitObject);
            transform.GetChild(0).gameObject.SetActive(false);
        }else{
            Debug.Log("Error, ground not found");
        }
        GetComponent<Renderer>().enabled = false;
        point = Instantiate(fruit[fruit.Count-1], transform.position + new Vector3(0,1,0), Quaternion.identity);
    }
}
