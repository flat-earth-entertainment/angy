using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LemmingInitialDirection : MonoBehaviour
{
    public void RotateLemming(Shooter lemming){
        lemming.horSnap = transform.localEulerAngles.y + 180;
        lemming.predict();
    }
}
