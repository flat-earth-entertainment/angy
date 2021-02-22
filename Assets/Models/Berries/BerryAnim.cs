using UnityEngine;
using DG.Tweening;

public class BerryAnim : MonoBehaviour
{
    public float rotSpeed;
    public float duration = 3f;
    public int vibrato = 10;
    public float elasticity = 0.5f;
    public Vector3 punchScale = new Vector3(1,1,1);
    
    // Start is called before the first frame update
    void Start()
    {
        rotSpeed = 10;
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(new Vector3(0, rotSpeed * Time.deltaTime, 0), Space.World);
       
       if (Input.GetButtonDown("Fire1"))
       {
           transform.DOPunchScale(punchScale, duration, vibrato, elasticity);
       }
       
    }
}