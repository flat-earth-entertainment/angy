using System;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

public class HelpUi : MonoBehaviour
{
    public static Action OnClose;
    private Rewired.Player rewiredPlayer;
    public int optionPage;
    public List<GameObject> helpScreens;

    // Start is called before the first frame update
    void Start()
    {
        rewiredPlayer = ReInput.players.GetPlayer(0);
    }

    // Update is called once per frame
    void Update()
    {
    }
    public void Next(){
        optionPage++;
        if(optionPage == helpScreens.Count){
            optionPage = 0;
        }
        for (int i = 0; i < helpScreens.Count; i++)
        {
            if(i == optionPage){
                helpScreens[i].SetActive(true);
            }else{
                helpScreens[i].SetActive(false);
            }
        }
    }
    public void Back(){
        OnClose?.Invoke();
        optionPage = 0;
    }
}
