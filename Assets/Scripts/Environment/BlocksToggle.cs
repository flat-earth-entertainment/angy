using UnityEngine;

namespace Environment
{
    public class BlocksToggle : MonoBehaviour
    {
        [SerializeField]
        private GameObject[] blocksToToggle;

        [SerializeField]
        private bool fireOnlyOnce;

        private bool _beenToggled;

        private void OnTriggerEnter(Collider other)
        {
            if (fireOnlyOnce && _beenToggled)
                return;

            if (other.CompareTag("Lemming"))
            {
                foreach (var block in blocksToToggle)
                {
                    block.SetActive(!block.activeSelf);
                    _beenToggled = true;
                }
            }
        }
    }
}