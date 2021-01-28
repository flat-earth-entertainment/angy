using System;
using UnityEngine;
using Rewired;

public class Shooter : MonoBehaviour{

    public event Action Shot;
    
    public int playerId = 1;
    private Rewired.Player rewiredPlayer;

    public GameObject firePoint;
    public GameObject ballPrefab;
    public float power;
    public float rotationSpeed;

    Vector3 currentPosition;
    Quaternion currentRotation;
    [HideInInspector]
    public float rotationX;
    [Range(180, 270)]
    [Header("Must be a multiplicative of 5")]
    public int xMinAngle = 180, xMaxAngle = 270;

    public GameObject ballStorage { get; private set; }
    public LineRenderer lineRender;
    public bool activateShootingRetinae = true, active = true;
    private int vertSnap, horSnap;
    // how many degrees the shooting retinae should snap. MUST add up to 360
    public int vertSnapAngle = 5, horSnapAngle = 5, greatSnapAngle = 30;
    private float snapCooldownTimer, vertSnapCooldownTimer, horSnapCooldownTimer;
    public float snapCooldown = 0.2f;
    private bool movedRet;
    private Rigidbody rb;

    void Start(){
        rewiredPlayer = ReInput.players.GetPlayer(playerId);

        currentPosition = transform.position;
        currentRotation = transform.rotation;
        //predict(); Doesn't work atm since i had to move the physics scene creation by one frame

        // Needs to be fixed : Find shared linerenderer
        //lineRender = transform.parent.GetComponentInChildren<LineRenderer>();

        // Debug.Log(lineRender.gameObject.name,lineRender);
        ballStorage = Instantiate(ballPrefab, firePoint.transform.position, Quaternion.identity);
        rb = ballStorage.GetComponent<Rigidbody>();
        transform.parent = ballStorage.transform;

        ShouldPlayerActivate(playerId);
    }

    public Vector3 calculateForce(){
        return transform.forward * power;
    }

    void shoot(){   // Needs to be redone, atm just creates a new ball
        rb.constraints = RigidbodyConstraints.None;
        rb.AddForce(calculateForce(), ForceMode.Impulse);
        ballStorage.GetComponent<BallBehaviour>().inMotion = true;
        
        Shot?.Invoke();
        
        if (lineRender !=null || !lineRender.Equals(null)) lineRender.enabled = false;
    }

    void Update(){
        if(activateShootingRetinae && active){
            // Vertical movement controls
            if(vertSnapCooldownTimer <= 0){ // delays snapping intervals
                float vertical = rewiredPlayer.GetAxis("Move Vertical");
                if(Mathf.Abs(vertical) > 0){
                    vertSnap += PositiveOrNegative(vertical);
                    vertSnap = Mathf.Clamp(vertSnap, xMinAngle / vertSnapAngle, xMaxAngle / vertSnapAngle );
                    vertSnapCooldownTimer = snapCooldown;
                    movedRet = true;
                }
            }else{
                vertSnapCooldownTimer -= Time.deltaTime;
            }
            // Horizontal movement controls
            if(horSnapCooldownTimer <= 0){  // delays snapping intervals
                float horizontal = rewiredPlayer.GetAxis("Move Horizontal");
                if(Mathf.Abs(horizontal) > 0){
                    horSnap += PositiveOrNegative(horizontal);
                    horSnapCooldownTimer = snapCooldown;
                    movedRet = true;
                }
                if(rewiredPlayer.GetButtonDown("SnapLeft")){    
                    horSnap -= greatSnapAngle / horSnapAngle;
                    horSnapCooldownTimer = snapCooldown;
                    movedRet = true;
                }
                if(rewiredPlayer.GetButtonDown("SnapRight")){
                    horSnap += greatSnapAngle / horSnapAngle;
                    horSnapCooldownTimer = snapCooldown;
                    movedRet = true;
                }
            }else{
                horSnapCooldownTimer -= Time.deltaTime;
            }
            if(movedRet){
                transform.rotation = Quaternion.Euler(vertSnap * vertSnapAngle, horSnap * horSnapAngle, 0);
                
                // Draw prediction curve if player has moved curve
                if(currentRotation != transform.rotation){
                    predict();
                    currentRotation = transform.rotation;
                }   

                if(currentPosition != transform.position){
                    predict();
                    currentPosition = transform.position;
                }
                movedRet = false;
            }
            // Shoot the ball
            if(rewiredPlayer.GetButtonDown("Confirm")){
                shoot();
                DisableRetinae();
                active = false;
            }
        }
    }
    void DisableRetinae(){
        activateShootingRetinae = false;

    }
    int PositiveOrNegative(float num){ // Checks if the number is negative or positive, returns -1 if negative, returns 1 if positive, returns 0 if 0
        if(num < 0){
            return -1;
        }else if(num > 0){
            return 1;
        }
        return 0;
    }

    void predict(){
        PredictionManager.instance.predict(ballPrefab, firePoint.transform.position, calculateForce());
    }
    public void ShouldPlayerActivate(int playerToActivate){ // Use this to define what player can move.
        if(playerToActivate == playerId){
            if (lineRender != null || !lineRender.Equals(null)) lineRender.enabled = true;
            active = true;
            rb.constraints = RigidbodyConstraints.FreezeAll;    // Freeze player
            predict();
        }else{
            active = false;
        }
    }
}
