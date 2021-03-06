﻿using Config;
using NaughtyAttributes;
using UnityEngine;

namespace UI
{
    public class OptionsSceneUiController : MonoBehaviour
    {
        [Scene]
        [SerializeField]
        private string mainMenu;

        private void Awake()
        {
            OptionsController.BackButtonClicked = delegate { SceneChanger.ChangeScene(mainMenu); };
            OptionsController.Show(false);
        }
    }
}