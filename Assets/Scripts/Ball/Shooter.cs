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

    void Start(){
        rewiredPlayer = ReInput.players.GetPlayer(playerId);

        currentPosition = transform.position;
        currentRotation = transform.rotation;
        predict();
    }

    public Vector3 calculateForce(){
        return transform.forward * power;
    }

    void shoot(){   // Needs to be redone, atm just creates a new ball
        GameObject ball = Instantiate(ballPrefab, firePoint.transform.position, Quaternion.identity);
        ball.GetComponent<Rigidbody>().AddForce(calculateForce(), ForceMode.Impulse);
    }

    void Update(){
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

        if(currentRotation != transform.rotation){
           predict();
        }   

        if(currentPosition != transform.transform.position){
           predict();
        }       
    
        currentRotation = transform.rotation;

        if(rewiredPlayer.GetButtonDown("Confirm")){
            shoot();
        }
    }

    void predict(){
        PredictionManager.instance.predict(ballPrefab, firePoint.transform.position, calculateForce());
    }
}
