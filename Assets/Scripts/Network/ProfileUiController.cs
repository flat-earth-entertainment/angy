using NaughtyAttributes;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Network
{
    public class ProfileUiController : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI nicknameText;

        [SerializeField]
        private Image avatarImage;

        [SerializeField]
        private Button saveButton;

        [SerializeField]
        private Button backButton;

        [SerializeField]
        [Scene]
        private string backScene;

        [SerializeField]
        private Slider hueSlider;

        [SerializeField]
        private Slider saturationSlider;

        private void Awake()
        {
            hueSlider.onValueChanged.AddListener(delegate { UpdateAvatarColor(); });
            saturationSlider.onValueChanged.AddListener(delegate { UpdateAvatarColor(); });

            saveButton.onClick.AddListener(async delegate
            {
                saveButton.interactable = false;
                Color.RGBToHSV(avatarImage.color, out var h, out var s, out _);
                if (!await CurrentPlayer.TrySetAvatar(new PlayerAvatar(h, s)))
                {
                    Debug.LogError("Avatar not set! Some error...");
                }

                saveButton.interactable = true;
                backButton.onClick.Invoke();
            });

            backButton.onClick.AddListener(delegate { SceneManager.LoadScene(backScene); });

            SetProfile();
        }

        private void SetProfile()
        {
            nicknameText.text = CurrentPlayer.LeaderboardPlayer.Nickname;
            avatarImage.color = CurrentPlayer.LeaderboardPlayer.Avatar.AsColor();
            Color.RGBToHSV(avatarImage.color, out var h, out var s, out _);
            hueSlider.value = h;
            saturationSlider.value = s;
        }

        private void UpdateAvatarColor()
        {
            avatarImage.color = Color.HSVToRGB(hueSlider.value, saturationSlider.value, 1);
        }
    }
}