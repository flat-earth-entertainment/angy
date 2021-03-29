using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UpdateUiSelection : MonoBehaviour
{
    public EventSystem[] evSys;
    private void Awake() {
        evSys = FindObjectsOfType<EventSystem>();
        foreach (var item in evSys)
        {
            if(item.gameObject == gameObject){

            }else{
                item.gameObject.SetActive(false);
            } 
        }
    }
    private void OnDestroy() {
        foreach (var item in evSys)
        {
            if(item.gameObject == gameObject){

            }else{
                item.gameObject.SetActive(true);
            }
        }
    }
    // Update is called once per frame
    void Update()
    {
        if(EventSystem.current!=null && EventSystem.current.currentSelectedGameObject == null){
            EventSystem.current.SetSelectedGameObject(FindObjectOfType<UnityEngine.UI.Button>().gameObject);
        }
    }
}
