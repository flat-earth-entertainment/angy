using UnityEngine;

namespace Ball
{
    public class GroundIndicator : MonoBehaviour
    {
        public GameObject hitIndicator;

        [HideInInspector]
        public GameObject spawnedIndicator;

        public bool isPlayer;
        private int _timer;

        private void OnCollisionEnter(Collision collision)
        {
            if (_timer == 1 && !isPlayer)
            {
                spawnedIndicator = Instantiate(hitIndicator, transform.position, Quaternion.identity);
                if (Physics.Raycast(transform.position, Vector3.down, out var hit, 10, LayerMask.GetMask("IgnoredMap")))
                {
                    spawnedIndicator.transform.up = hit.normal;
                    spawnedIndicator.transform.position = new Vector3(0, 0.01f, 0) + hit.point;
                }

                //spawnedIndicator.transform.rotation = Quaternion.LookRotation(direction, transform.up);
            }

            _timer++;
        }
    }
}