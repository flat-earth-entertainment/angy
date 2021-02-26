using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UpdateUiSelection : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        if(EventSystem.current.currentSelectedGameObject == null){
            EventSystem.current.SetSelectedGameObject(FindObjectOfType<UnityEngine.UI.Button>().gameObject);
        }
    }
}
