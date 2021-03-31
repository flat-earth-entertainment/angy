using UnityEngine;

namespace Environment.HammerTrap
{
    public class HammerTrap : MonoBehaviour
    {
        public GameObject blood;

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Lemming"))
            {
                var bld = Instantiate(blood, other.transform.position, Quaternion.identity);
                bld.GetComponent<ParticleSystem>().Play();
                Destroy(bld, 8f);
            }
        }
    }
}