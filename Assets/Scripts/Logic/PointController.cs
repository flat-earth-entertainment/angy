using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PointController : MonoBehaviour
{
    public List<GameObject> pointHolders;
    public List<int> pointIds;
    private int enemiesRemaining;
    public List<Text> pointText;
    // Start is called before the first frame update
    void Start()
    {
        enemiesRemaining = pointHolders.Count;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void EnemyHit(int playerId){
        enemiesRemaining--;
        if(enemiesRemaining == 1){
            foreach (GameObject item in pointHolders){
                GoodNeutralMushroom enemy = item.GetComponent<GoodNeutralMushroom>();
                if(enemy.ownerId == 99){
                    enemy.SpawnGoal();

                    // Tell camera to focus on goal and freeze balls
                }
            }
        }
    }
    public void UpdateScore(){
        for (int i = 0; i < pointIds.Count; i++)
        {
            pointIds[i] = 0;
        }
        foreach (GameObject item in pointHolders)
        {
            GoodNeutralMushroom enemy = item.GetComponent<GoodNeutralMushroom>();
            if(enemy.ownerId < 98){
                pointIds[enemy.ownerId] += enemy.pointValue;
                pointText[enemy.ownerId].text = pointIds[enemy.ownerId].ToString();
            }
        }
    }
}
