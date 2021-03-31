using System.Collections.Generic;
using Ball;
using Player;
using UnityEngine;

namespace Environment.Teleporter
{
    public class TeleporterProximityCheck : MonoBehaviour
    {
        public float range = 4;

        [HideInInspector]
        public bool inRange;

        [HideInInspector]
        public TeleporterProximityCheck linkedTeleporter;

        [HideInInspector]
        public Animator anim;

        public List<PlayerView> players = new List<PlayerView>();

        public List<GameObject> particles = new List<GameObject>();

        private void Start()
        {
            players.AddRange(FindObjectsOfType<PlayerView>());
            linkedTeleporter = transform.parent.GetComponentInChildren<Teleporter>().teleportTarget
                .GetComponentInChildren<TeleporterProximityCheck>();
            anim = GetComponent<Animator>();
        }

        private void Update()
        {
            inRange = false;
            foreach (var item in players)
            {
                if (Vector3.Distance(transform.position,
                    item.GetComponentInChildren<BallBehaviour>(true).transform.position) < range)
                {
                    inRange = true;
                }
            }

            if (inRange || linkedTeleporter.inRange)
            {
                anim.SetBool("isOpened", true);
                foreach (var item in particles)
                {
                    item.SetActive(true);
                }
            }
            else
            {
                anim.SetBool("isOpened", false);
                foreach (var item in particles)
                {
                    item.SetActive(false);
                }
            }
        }
    }
}