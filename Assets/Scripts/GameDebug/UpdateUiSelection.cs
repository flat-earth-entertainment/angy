using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace GameDebug
{
    public class UpdateUiSelection : MonoBehaviour
    {
        public EventSystem[] evSys;

        private void Awake()
        {
            evSys = FindObjectsOfType<EventSystem>();
            foreach (var item in evSys)
            {
                if (item.gameObject == gameObject)
                {
                }
                else
                {
                    item.gameObject.SetActive(false);
                }
            }
        }

        private void Update()
        {
            if (EventSystem.current != null && EventSystem.current.currentSelectedGameObject == null)
            {
                EventSystem.current.SetSelectedGameObject(FindObjectOfType<Button>().gameObject);
            }
        }

        private void OnDestroy()
        {
            foreach (var item in evSys)
            {
                if (item.gameObject == gameObject)
                {
                }
                else
                {
                    item.gameObject.SetActive(true);
                }
            }
        }
    }
}