using UnityEngine;

namespace Ball.Objectives
{
    public class MushroomMood : MonoBehaviour
    {
        private Animator _anim;
        private int _playerInTriggerCount;

        private void Start()
        {
            _anim = GetComponent<Animator>();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Lemming"))
            {
                _playerInTriggerCount++;
                UpdateMood();
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Lemming"))
            {
                _playerInTriggerCount--;
                UpdateMood();
            }
        }

        private void UpdateMood()
        {
            if (_playerInTriggerCount > 0)
            {
                _anim.SetBool("isTerrifyed", true);
                transform.GetChild(1).GetComponent<SkinnedMeshRenderer>().materials[0].SetFloat("MushroomFace", 1);
            }
            else
            {
                _anim.SetBool("isTerrifyed", false);
                transform.GetChild(1).GetComponent<SkinnedMeshRenderer>().materials[0].SetFloat("MushroomFace", 0);
            }
        }
    }
}