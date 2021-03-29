using Audio;
using UnityEngine;

namespace Environment.Speed_pad
{
    public class SpeedPad : MonoBehaviour
    {
        public float speed = 10;

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Lemming"))
            {
                other.GetComponent<Rigidbody>().velocity = transform.forward * speed;
                AudioManager.PlaySfx(SfxType.SpeedPadTouch);
            }
        }
    }
}