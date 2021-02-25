using UnityEngine;
using DG.Tweening;

public class BerryAnim : MonoBehaviour
{
    public float rotSpeed = 50;
    public float duration = 3f;
    public int vibrato = 10;
    public float elasticity = 0.5f;
    public Vector3 punchScale = new Vector3(1,1,1);

    private float _startTime;
    private float _journeyLength;
    
    // Start is called before the first frame update
    void Start()
    {
        _startTime = Time.time;
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(new Vector3(0, rotSpeed * Time.deltaTime, 0), Space.World);
    }
    public void BerryHit(){
        if(!DOTween.IsTweening(transform)){
            transform.DOPunchScale(punchScale, duration, vibrato, elasticity);
        }

    }
}
