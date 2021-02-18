using UnityEngine;

public class PointAnim : MonoBehaviour
{
    private Vector3 startPos;
    private float heightIncrease = 2, timer;
    // Start is called before the first frame update
    void Start()
    {
        startPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if(timer < 1){
            timer += Time.deltaTime;
            transform.position = Vector3.Lerp(startPos, startPos + new Vector3(0,heightIncrease,0), timer * 3);
        }

    }
}
