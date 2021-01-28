using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

public class Rotator : MonoBehaviour
{
    private int playerId = 1;
    private Rewired.Player rewiredPlayer;
    private float rotationSpeed;
    // Start is called before the first frame update
    void Start()
    {
        playerId = transform.GetChild(0).GetComponent<Shooter>().playerId;
        rotationSpeed = transform.GetChild(0).GetComponent<Shooter>().rotationSpeed;
        rewiredPlayer = ReInput.players.GetPlayer(playerId);
    }

    // Update is called once per frame
    void Update()
    {
        float Horizontal = rewiredPlayer.GetAxis("Move Horizontal");
        transform.Rotate(new Vector3(0,Horizontal * rotationSpeed * Time.deltaTime,0));
    }
}
