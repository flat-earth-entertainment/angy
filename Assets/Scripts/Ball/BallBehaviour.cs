using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallBehaviour : MonoBehaviour
{
    public event Action BecameStill;
    
    private Shooter shooter;
    private Rigidbody rb;
    public bool inMotion;
    private float timer;
    // Start is called before the first frame update
    void Start()
    {
        shooter = GetComponentInChildren<Shooter>();
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if(inMotion){
            timer += Time.deltaTime;
        }
        if(rb.velocity.magnitude < 0.2f && inMotion && timer > 0.5f){
            print("Still");
            BecameStill?.Invoke();
            timer = 0;
            inMotion = false;
            shooter.activateShootingRetinae = true;
            rb.velocity = new Vector3(0,0,0);
        }
    }
}
