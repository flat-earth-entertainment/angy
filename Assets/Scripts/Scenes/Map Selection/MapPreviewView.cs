using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Scenes.Map_Selection
{
    public class MapPreviewView : MonoBehaviour
    {
        public static event Action<MapCollection> MapPreviewSelected;

        [SerializeField]
        private TextMeshProUGUI nameText;

        [SerializeField]
        private Image previewImage;

        [SerializeField]
        private Button selectButton;


        private MapCollection _mapCollection;

        private void Awake()
        {
            selectButton.onClick.AddListener(delegate { MapPreviewSelected?.Invoke(_mapCollection); });
        }

        public void Setup(MapCollection mapCollection)
        {
            _mapCollection = mapCollection;
            nameText.text = mapCollection.Name;
            previewImage.sprite = mapCollection.PreviewImage;
        }
    }
}