using UnityEngine;
using Rewired;

public class Shooter : MonoBehaviour{
    public int playerId = 1;
    private Player rewiredPlayer;

    public GameObject firePoint;
    public GameObject ballPrefab;
    public float power;
    public float rotationSpeed;

    Vector3 currentPosition;
    Quaternion currentRotation;
    [HideInInspector]
    public float rotationX;
    [Range(270f, 360f)]
    public float xMinAngle = 300f, xMaxAngle = 358f;

    public GameObject ballStorage { get; private set; }
    private LineRenderer lineRender;
    public bool activateShootingRetinae = true;

    void Start(){
        rewiredPlayer = ReInput.players.GetPlayer(playerId);

        currentPosition = transform.position;
        currentRotation = transform.rotation;
        //predict(); Doesn't work atm since i had to move the physics scene creation by one frame


        lineRender = transform.parent.GetComponentInChildren<LineRenderer>();
        Debug.Log(lineRender.gameObject.name,lineRender);
        ballStorage = Instantiate(ballPrefab, firePoint.transform.position, Quaternion.identity);
        transform.parent = ballStorage.transform;
    }

    public Vector3 calculateForce(){
        return transform.forward * power;
    }

    void shoot(){   // Needs to be redone, atm just creates a new ball
        ballStorage.GetComponent<Rigidbody>().AddForce(calculateForce(), ForceMode.Impulse);

        ballStorage.GetComponent<BallBehaviour>().inMotion = true;
    }

    void Update(){
        if(activateShootingRetinae){
            if (lineRender != null || !lineRender.Equals(null)) lineRender.enabled = true;

            float vertical = rewiredPlayer.GetAxis("Move Vertical") * rotationSpeed;
            float Horizontal = rewiredPlayer.GetAxis("Move Horizontal") * rotationSpeed;
            
            transform.Rotate(-vertical * Time.deltaTime, Horizontal * Time.deltaTime, 0.0f);

            rotationX = Mathf.Clamp(transform.eulerAngles.x, xMinAngle, xMaxAngle);
            if(transform.eulerAngles.x < 10f){  // prevents the max clamp from flipping to min angle because of lag
                rotationX = xMaxAngle;
            }

            transform.rotation = Quaternion.Euler(rotationX, transform.eulerAngles.y, 0);

            float snapIndex = Mathf.Floor(transform.eulerAngles.y / 30);    // round down to the closest snapping point
            if(rewiredPlayer.GetButtonDown("SnapLeft")){    // Snap to the left
                // If you've snapped to a snap point, allows you to snap to the next one
                if(transform.eulerAngles.y >= (snapIndex * 30) - 1 || transform.eulerAngles.y <= (snapIndex * 30) + 1){
                    snapIndex--;
                }
                transform.rotation = Quaternion.Euler(transform.eulerAngles.x, snapIndex * 30, 0);
            }
            if(rewiredPlayer.GetButtonDown("SnapRight")){   // Snap to the right
                transform.rotation = Quaternion.Euler(transform.eulerAngles.x, (snapIndex + 1) * 30, 0);
            }

            // Draw prediction curve if player has moved curve
            if(currentRotation != transform.rotation){
            predict();
            }   

            if(currentPosition != transform.transform.position){
            predict();
            }       
        
            currentRotation = transform.rotation;
            // Shoot the ball
            if(rewiredPlayer.GetButtonDown("Confirm")){
                shoot();
                DisableRetinae();
            }
        }else
        {
            if (lineRender !=null || !lineRender.Equals(null)) lineRender.enabled = false;
        }
    }
    void DisableRetinae(){
        activateShootingRetinae = false;

    }

    void predict(){
        PredictionManager.instance.predict(ballPrefab, firePoint.transform.position, calculateForce());
    }
}
