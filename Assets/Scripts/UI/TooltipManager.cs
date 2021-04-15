using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

public class TooltipManager : MonoBehaviour
{
    private Animator animator;
    public List<GameObject> tooltips, borders;
    private int tooltipId;
    private List<Rewired.Player> _rewiredPlayer = new List<Rewired.Player>();
    private List<bool> achieved;
    private List<Player.PlayerView> players = new List<Player.PlayerView>();
    private bool waiting = true, once;
    // Start is called before the first frame update
    IEnumerator Start()
    {
        yield return null;
        players.AddRange(FindObjectsOfType<Player.PlayerView>());
        while (waiting)
        {
            foreach (var item in players)
            {
                if(item.PlayerState == Player.PlayerState.ActiveAiming){
                    waiting = false;
                }
            }
            yield return null;
        }
        animator = GetComponent<Animator>();
        _rewiredPlayer.AddRange(ReInput.players.AllPlayers);

        achieved = new List<bool>(new bool[tooltips.Count]);
        StartCoroutine("Tooltip");
    }

    // Update is called once per frame
    void Update()
    {
        if(!waiting){
            switch (tooltipId)
            {
                case 0: // Listen for A & D
                    foreach (var item in _rewiredPlayer)
                    {
                        if(Mathf.Abs(item.GetAxis("Move Horizontal")) > 0 && !achieved[tooltipId]){
                            achieved[tooltipId] = true;
                        }
                    }
                    break;
                case 1: // Listen for W & S
                    foreach (var item in _rewiredPlayer)
                    {
                        if(Mathf.Abs(item.GetAxis("Move Vertical")) > 0 && !achieved[tooltipId]){
                            achieved[tooltipId] = true;
                        }
                    }
                    break;
                case 2: // Listen for Space
                    foreach (var item in _rewiredPlayer)
                    {
                        if(item.GetButtonDown("Confirm") && !achieved[tooltipId]){
                            if(once){   // Wait for another press
                                achieved[tooltipId] = true;
                                once = false;
                            }
                            once = true;
                        }
                    }
                    break;
                case 3: // Listen for F
                    break;
            }
        }
    }
    IEnumerator Tooltip(){
        yield return null;
        // enable new 
        for (int i = 0; i < tooltips.Count; i++)
        {
            if(tooltipId == i){
                tooltips[i].SetActive(true);
            }else{
                tooltips[i].SetActive(false);
            }
        }
        // play animation
        animator.SetBool("ShowTooltip", true);
        while (!achieved[tooltipId])
        {
            yield return null;
        }
        // Change frame to green
        borders[0].SetActive(false);
        borders[1].SetActive(true);

        // reset animator
        yield return new WaitForSeconds(2);
        animator.SetBool("ShowTooltip", false);
        yield return new WaitForSeconds(2);
        // reset frame
        borders[0].SetActive(true);
        borders[1].SetActive(false);
        // add one to tooltip id
        tooltipId++;
        if(tooltipId < tooltips.Count){
            StartCoroutine("Tooltip");
        }
    }
}
