using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Rewired;

public class Shooter : MonoBehaviour{

    public event Action Shot;
    
    public PlayerView PlayerView { get; private set; }

    public GameObject BallStorage => ballStorage;

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

    [SerializeField]
    private GameObject ballStorage;
    
    public bool activateShootingRetinae = true, active = true;
    private int vertSnap = 36, horSnap = 35;
    // how many degrees the shooting retinae should snap. MUST add up to 360
    public int vertSnapAngle = 5, horSnapAngle = 5, greatSnapAngle = 30;
    private float snapCooldownTimer, vertSnapCooldownTimer, horSnapCooldownTimer;
    public float snapCooldown = 0.2f;
    private bool movedRet, forcePercentBool;
    [SerializeField]
    private Rigidbody rb;
    public float forcePercent = 1;
    [HideInInspector]
    public Slider powerSlider;

    public void SetPlayer(PlayerView playerView)
    {
        PlayerView = playerView;
    }
    
    void Start(){
        rewiredPlayer = ReInput.players.GetPlayer(playerId);

        currentPosition = transform.position;
        currentRotation = transform.rotation;
        //predict(); Doesn't work atm since i had to move the physics scene creation by one frame

        ballStorage = transform.parent.gameObject;
        rb = ballStorage.GetComponent<Rigidbody>();

        ShouldPlayerActivate(playerId);

        // MUST BE IMPROVED
        powerSlider = GameObject.FindGameObjectWithTag("TEMPFINDSLIDER").transform.GetChild(0).GetChild(0).GetComponent<Slider>();
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
                StartCoroutine("PreShot");
            }
        }
    }
    void shoot(){
        rb.constraints = RigidbodyConstraints.None;
        rb.AddForce(calculateForce(), ForceMode.Impulse);
        ballStorage.GetComponent<BallBehaviour>().inMotion = true;
        
        Shot?.Invoke();
    }
    public Vector3 calculateForce(){
        return transform.forward * power * forcePercent;
    }
    private IEnumerator PreShot(){
        activateShootingRetinae = false;

        // change spin and tilt goes here //


        // Power Goes Here //
        powerSlider.transform.parent.gameObject.SetActive(true);
        yield return StartCoroutine("CalculateShootForce");
        powerSlider.transform.parent.gameObject.SetActive(false);
        // Power Ends Here //
        
        // Shoot the ball
        shoot();
        DisableRetinae();
        active = false;
        
        yield return true;
        
        forcePercent = 1;
        activateShootingRetinae = true;
    }
    private IEnumerator CalculateShootForce(){
        forcePercent = 0;
        yield return null;
        while (!rewiredPlayer.GetButtonDown("Confirm") && forcePercent >= 0){
            powerSlider.value = forcePercent;
            if(!forcePercentBool){
                forcePercent += Time.deltaTime / 3;
                if(forcePercent >= 1.033f){
                    forcePercentBool = true;
                }
            }else{
                forcePercent -= Time.deltaTime / 3;
            }
            yield return null;
        }
        if(forcePercent < 0){
            forcePercent = 0;
        }
        if(forcePercent > 1){
            forcePercent = 1;
        }
        forcePercentBool = false;
        StopCoroutine("CalculateShootForce");
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

    public void predict(){
        PredictionManager.instance.predict(ballPrefab, firePoint.transform.position, calculateForce());
    }
    public void ShouldPlayerActivate(int playerToActivate){ // Use this to define what player can move.
        if(playerToActivate == playerId){
            active = true;
            rb.constraints = RigidbodyConstraints.FreezeAll;    // Freeze player
            transform.rotation = Quaternion.Euler(vertSnap * vertSnapAngle, horSnap * horSnapAngle, 0); // Set aiming retinae to actual shooting direction
            predict();
        }else{
            active = false;
        }
    }
}
