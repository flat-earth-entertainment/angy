using Rewired;
using UnityEngine;

namespace Ball
{
    public class Rotator : MonoBehaviour
    {
        private int _playerId = 1;
        private Rewired.Player _rewiredPlayer;
        private float _rotationSpeed;

        private void Start()
        {
            _playerId = transform.GetChild(0).GetComponent<Shooter>().playerId;
            _rotationSpeed = transform.GetChild(0).GetComponent<Shooter>().rotationSpeed;
            _rewiredPlayer = ReInput.players.GetPlayer(_playerId);
        }

        private void Update()
        {
            var horizontal = _rewiredPlayer.GetAxis("Move Horizontal");
            transform.Rotate(new Vector3(0, horizontal * _rotationSpeed * Time.deltaTime, 0));
        }
    }
}