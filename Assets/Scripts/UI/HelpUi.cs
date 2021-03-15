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
        if(rewiredPlayer.GetButtonDown("Confirm")){
            optionPage++;
        }
        if(rewiredPlayer.GetButtonDown("AbilityFire")){
            optionPage--;
        }
        if(optionPage < 0){
            // GO BACK TO PAUSE/MENU SCREEN
            OnClose?.Invoke();

            optionPage = 0;
        }
        if(optionPage == helpScreens.Count){
            // GO BACK TO PAUSE/MENU SCREEN
            OnClose?.Invoke();

            optionPage = helpScreens.Count - 1;
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
}
