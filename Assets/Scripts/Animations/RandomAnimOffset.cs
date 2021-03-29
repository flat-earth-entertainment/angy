using UnityEngine;

namespace Animations
{
    public class RandomAnimOffset : MonoBehaviour
    {
        private void Awake()
        {
            GetComponent<Animator>().SetFloat("Offset", Random.Range(0.0f, 1.0f));
        }
    }
}