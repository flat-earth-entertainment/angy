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
    private float timer, stopTimer;
    private Vector3 spinDirection, currentVelocity;
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
            if(rb.velocity.magnitude < 0.2f && timer > 0.5f){
                stopTimer += Time.deltaTime;
                if(stopTimer > 1){
                    BecameStill?.Invoke();
                    timer = 0;
                    inMotion = false;
                    shooter.activateShootingRetinae = true;
                    rb.velocity = new Vector3(0,0,0);
                    rb.angularVelocity = new Vector3(0,0,0);
                    shooter.lemmingAnim.SetBool("isBall", false);
                    transform.rotation = Quaternion.Euler(180,0,0);
                    shooter.lemming.transform.rotation = Quaternion.Euler(0, 0, 0);
                }
            }else{
                stopTimer = 0;
            }
            currentVelocity = rb.velocity.normalized;
        }
    }
    public IEnumerator BallSpin(Vector3 spinDir){
        spinDirection = spinDir;
        yield return null;
        while (inMotion)
        {
            rb.AddTorque(transform.rotation * spinDirection, ForceMode.Force);
            yield return null;
        }
    }
}
