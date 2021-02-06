using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Map_Selection
{
    public class MapPreviewView : MonoBehaviour
    {
        public static event Action<MapPreview> MapPreviewSelected;

        [SerializeField]
        private TextMeshProUGUI nameText;

        [SerializeField]
        private Image previewImage;

        [SerializeField]
        private Button selectButton;


        private MapPreview _mapPreview;

        private void Awake()
        {
            selectButton.onClick.AddListener(delegate { MapPreviewSelected?.Invoke(_mapPreview); });
        }

        public void Setup(MapPreview mapPreview)
        {
            _mapPreview = mapPreview;
            nameText.text = mapPreview.Name;
            previewImage.sprite = mapPreview.PreviewImage;
        }
    }
}