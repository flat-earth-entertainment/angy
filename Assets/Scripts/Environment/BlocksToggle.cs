using MeshPhysics;
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
                    var newState = !block.activeSelf;
                    block.SetActive(newState);

                    if (newState)
                    {
                        MeshCombiner.AddObject(block.transform);
                    }
                    else
                    {
                        MeshCombiner.RemoveObject(block.transform);
                    }

                    _beenToggled = true;
                }
            }
        }
    }
}