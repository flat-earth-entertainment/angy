using System;
using System.Collections.Generic;
using UnityEngine;

namespace UI
{
    public class HelpUi : MonoBehaviour
    {
        public static Action OnClose;
        public int optionPage;
        public List<GameObject> helpScreens;

        public void Next()
        {
            optionPage++;
            if (optionPage == helpScreens.Count)
            {
                optionPage = 0;
            }

            for (var i = 0; i < helpScreens.Count; i++)
            {
                if (i == optionPage)
                {
                    helpScreens[i].SetActive(true);
                }
                else
                {
                    helpScreens[i].SetActive(false);
                }
            }
        }

        public void Back()
        {
            OnClose?.Invoke();
            optionPage = 0;
        }
    }
}