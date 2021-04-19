using System;
using NaughtyAttributes;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Network
{
    public class LoginSceneUiController : MonoBehaviour
    {
        [SerializeField]
        private Toggle registerToggle;

        [SerializeField]
        private TMP_InputField loginInput;

        [SerializeField]
        private TMP_InputField passwordInput;

        [SerializeField]
        private Button loginButton;

        [SerializeField]
        private Button backButton;

        [SerializeField]
        [Scene]
        private string backScene;

        [SerializeField]
        [Scene]
        private string onlineScene;

        [SerializeField]
        private TextMeshProUGUI errorText;

        [SerializeField]
        private GameObject errorParent;

        [SerializeField]
        private Button closeErrorButton;

        private void Awake()
        {
            registerToggle.onValueChanged.AddListener(delegate(bool isOn)
            {
                if (isOn)
                {
                    loginButton.GetComponentInChildren<TextMeshProUGUI>().text = "Register";
                }
                else
                {
                    loginButton.GetComponentInChildren<TextMeshProUGUI>().text = "Login";
                }
            });

            closeErrorButton.onClick.AddListener(delegate { errorParent.SetActive(false); });

            loginButton.onClick.AddListener(delegate
            {
                loginButton.interactable = false;

                if (registerToggle.isOn)
                {
                    TryRegisterAndLogin();
                }
                else
                {
                    TryLogin();
                }

                loginButton.interactable = true;
            });

            backButton.onClick.AddListener(delegate { SceneManager.LoadScene(backScene); });
        }

        private async void TryRegisterAndLogin()
        {
            var registerResponse = await CurrentPlayer.TryRegister(loginInput.text, passwordInput.text);
            if (registerResponse == RegisterResponse.Success)
            {
                TryLogin();
            }
            else
            {
                errorText.text = registerResponse.ToString();
                errorParent.SetActive(true);
            }
        }

        private async void TryLogin()
        {
            var loginResponse = await CurrentPlayer.TryLogin(loginInput.text, passwordInput.text);
            if (loginResponse == LoginResponse.Success || loginResponse == LoginResponse.AlreadyLoggedIn)
            {
                PhotonNetwork.OfflineMode = false;
                if (PhotonNetwork.ConnectUsingSettings())
                {
                    SceneManager.LoadScene(onlineScene);
                }
            }
            else
            {
                errorText.text = loginResponse.ToString();
                errorParent.SetActive(true);
            }
        }
    }
}