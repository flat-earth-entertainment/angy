using System;
using System.Collections;
using System.Collections.Generic;
using Logic;
using UnityEngine;

public class WaterPhysics : MonoBehaviour
{
    public float baseDrag = 0.25f, waterDrag = 2f, bounceThreshold = 5;

    [Tooltip("Should be above 3 or else bounce isn't strong enough")]
    public float waterVerticalBounce = 5;

    public bool iceAbility, bounce;
    public GameObject ice, iceCylinder;
    private Abilities.IceBlockOnCollision player;
    private bool delay;

    private AbilityController _abilityController;
    private List<Transform> playerIcePlatforms = new List<Transform>();

    private void Awake()
    {
        _abilityController = FindObjectOfType<AbilityController>();
    }

    void OnTriggerEnter(Collider other)
    {
        // Must be rewritten if we end up wanting to kill players underwater
        if (other.tag == "Lemming")
        {
            WaterEvents(other);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.tag == "Lemming")
        {
            other.attachedRigidbody.drag = baseDrag;
        }
    }

    void WaterEvents(Collider other)
    {
        // Find out what to do
        // is player ice?
        if (other.gameObject.TryGetComponent(out Abilities.IceBlockOnCollision iceAbil))
        {
            player = iceAbil;
            iceAbility = true;
        }

        // is the player fast enough for a bounce?
        if (other.attachedRigidbody.velocity.magnitude > bounceThreshold)
        {
            bounce = true;
        }

        // Do stuff
        if (iceAbility)
        {
            ice.SetActive(true);
            GameObject newIcePlat = Instantiate(iceCylinder, transform.position, Quaternion.identity);
            newIcePlat.transform.parent = ice.transform;
            playerIcePlatforms.Add(newIcePlat.transform);
            StartCoroutine("DisableIce");
        }
        else if (bounce)
        {
            Vector3 vel = other.attachedRigidbody.velocity;
            other.attachedRigidbody.velocity = new Vector3(vel.x, waterVerticalBounce, vel.z);
        }
        else
        {
            // Normal water logic
            other.attachedRigidbody.drag = waterDrag;
        }

        iceAbility = bounce = false;
    }

    private IEnumerator DisableIce()
    {
        var plView = player.transform.parent.GetComponent<Player.PlayerView>();
        while (true)
        {
            if (_abilityController.GetPlayerAbility(plView).Finished)
            {
                break;
            }
            // Ice tracking
            foreach (Transform item in playerIcePlatforms)
            {
                item.transform.position = new Vector3(
                    Mathf.Clamp(player.transform.position.x,
                     transform.position.x - transform.localScale.x / 2,
                      transform.position.x + transform.localScale.x / 2),
                    item.transform.position.y,
                    Mathf.Clamp(player.transform.position.z,
                     transform.position.z - transform.localScale.z / 2,
                      transform.position.z + transform.localScale.z / 2));
            }

            yield return null;
        }
        foreach (var item in playerIcePlatforms)
        {
            Destroy(item.gameObject);
        }
        playerIcePlatforms = new List<Transform>();
        ice.SetActive(false);
    }
}