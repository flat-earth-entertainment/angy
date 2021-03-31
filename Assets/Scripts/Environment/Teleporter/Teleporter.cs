using System.Collections.Generic;
using Audio;
using UnityEngine;

namespace Environment.Teleporter
{
    public class Teleporter : MonoBehaviour
    {
        public enum TargetMode
        {
            Single,
            Multiple
        }

        public Transform teleportTarget;

        [HideInInspector]
        public float teleportCooldown;

        public bool directional;
        public TargetMode teleportTargetType;

        public List<Transform> multiTeleTarget;

        private void Awake()
        {
            if (teleportTarget == null && multiTeleTarget.Count > 0)
            {
                teleportTarget = multiTeleTarget[0];
            }
        }

        private void Update()
        {
            if (teleportCooldown > 0)
            {
                teleportCooldown -= Time.deltaTime;
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Lemming") && teleportCooldown <= 0)
            {
                AudioManager.PlaySfx(SfxType.TeleporterEngage);
                switch (teleportTargetType)
                {
                    case TargetMode.Single:
                    {
                        other.transform.position =
                            teleportTarget.GetComponentInChildren<Teleporter>().transform.position;
                        if (directional)
                        {
                            other.gameObject.GetComponent<Rigidbody>().velocity =
                                teleportTarget.GetComponentInChildren<Teleporter>().transform.forward *
                                other.gameObject.GetComponent<Rigidbody>().velocity.magnitude;
                        }

                        teleportTarget.GetComponentInChildren<Teleporter>().teleportCooldown = 0.25f;
                        teleportTarget.GetComponentInChildren<Animator>().SetTrigger("isTriggered");
                        transform.parent.GetComponentInChildren<Animator>().SetTrigger("isTriggered");
                        break;
                    }
                    case TargetMode.Multiple:
                    {
                        var selectedTeleporter = Random.Range(0, multiTeleTarget.Count);
                        other.transform.position = multiTeleTarget[selectedTeleporter]
                            .GetComponentInChildren<Teleporter>()
                            .transform.position;
                        if (directional)
                        {
                            other.gameObject.GetComponent<Rigidbody>().velocity =
                                teleportTarget.GetComponentInChildren<Teleporter>().transform.forward *
                                other.gameObject.GetComponent<Rigidbody>().velocity.magnitude;
                        }

                        multiTeleTarget[selectedTeleporter].GetComponentInChildren<Teleporter>().teleportCooldown =
                            0.25f;
                        multiTeleTarget[selectedTeleporter].GetComponentInChildren<Animator>()
                            .SetTrigger("isTriggered");
                        transform.parent.GetComponentInChildren<Animator>().SetTrigger("isTriggered");
                        break;
                    }
                }
            }
        }
    }
}