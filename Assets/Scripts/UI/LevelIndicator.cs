using System.Linq;
using GameSession;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UI
{
    public class LevelIndicator : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI currentLevelText;

        [SerializeField]
        private TextMeshProUGUI levelCountText;

        private void Awake()
        {
            var currentMapIndex = CurrentGameSession.MapCollection.Maps.ToList()
                .IndexOf(SceneManager.GetActiveScene().name);

            currentLevelText.text = (currentMapIndex + 1).ToString();

            levelCountText.text = CurrentGameSession.MapCollection.Maps.Length.ToString();
        }
    }
}