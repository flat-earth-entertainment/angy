using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class CellFractureOptimizer : MonoBehaviour
{
    public float duration = 5f;
    public float timeleft;

    // Start is called before the first frame update
    void Start()
    {
        timeleft = duration;
        DOTween.Init(true, true, LogBehaviour.Verbose).SetCapacity(200, 10);
    }

    // Update is called once per frame
    void Update()
    {
        timeleft -= Time.deltaTime;
        transform.DOScale(0f, 5);
        
        if (timeleft < 0)
        {
            gameObject.SetActive(false);
        }
    }
}
