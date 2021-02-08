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
    private Transform velocityDirection, offsetPointer, offsetHolder;
    public Vector3 displayVector;
    private float windDown = 1;

    public void ResetRotation()
    {
        //transform.rotation = Quaternion.Euler(180, 0, 0);
        //shooter.lemming.transform.rotation = Quaternion.Euler(0, 0, 0);
    }
    
    // Start is called before the first frame update
    void Start()
    {
        shooter = GetComponentInChildren<Shooter>();
        rb = GetComponent<Rigidbody>();

        velocityDirection = new GameObject("Velocity direction of player " + shooter.playerId).GetComponent<Transform>();
        velocityDirection.parent = transform;

        // All of these would be unnecessary if i for the life of me could just turn a vector correctly
        offsetPointer = new GameObject("Offset pointer").GetComponent<Transform>();
        offsetPointer.parent = velocityDirection;
        offsetHolder = new GameObject("Offset holder").GetComponent<Transform>();
        offsetHolder.parent = velocityDirection;
        offsetPointer.position = offsetHolder.position = new Vector3(0,0,0);
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
                    shooter.lemmingAnim.SetBool("isKnockback", false);
                    transform.rotation = Quaternion.Euler(180,0,0);
                    shooter.lemming.transform.rotation = Quaternion.Euler(0, (shooter.horSnap * shooter.horSnapAngle) + 180, 0);
                }
            }else{
                stopTimer = 0;
            }
            velocityDirection.transform.position = transform.position;
            velocityDirection.LookAt(rb.velocity + transform.position, Vector3.up);
        }
        if(rb.velocity.magnitude > 0.2f && !inMotion){
            inMotion = true;
        }
    }
    public IEnumerator BallSpin(Vector3 spinDir){
        yield return null;
        spinDirection = spinDir;
        offsetHolder.localPosition = spinDirection;
        offsetPointer.LookAt(offsetHolder, Vector3.up);
        
        Vector3 direction = offsetPointer.TransformDirection(offsetPointer.position);
        rb.AddTorque(direction * 20, ForceMode.Impulse);
        while(inMotion)
        {   
            direction = offsetPointer.TransformDirection(offsetPointer.position);
            displayVector = offsetPointer.forward;
            rb.AddTorque((direction * Time.deltaTime) * windDown, ForceMode.Impulse);
            //Quaternion rotation = Quaternion.Euler(0,velocityDirection.eulerAngles.y,0);
            //Vector3 rotateVector = rotation * spinDirection;
            //Vector3 rotateVector = velocityDirection.forward + spinDirection; 
            //Vector3 rotateVector = Quaternion.AngleAxis(velocityDirection.localEulerAngles.y, velocityDirection.up) * spinDirection;
            //Vector3 rotateVector = velocityDirection.localEulerAngles * spinDirection;
            //Vector3 rotateVector = Quaternion.Euler(0,velocityDirection.localRotation.y,0) * spinDirection;
            if(windDown > 0){
                windDown -= Time.deltaTime;
            }else{
                windDown = 0;
            }
            yield return null;
        }
    }
}
