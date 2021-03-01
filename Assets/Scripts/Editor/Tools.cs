using TMPro;
using UnityEditor;
using UnityEditor.Experimental.SceneManagement;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Editor
{
    public class Tools : EditorWindow
    {
        [MenuItem("Editor Tools/Tools")]
        private static void ShowWindow()
        {
            var window = GetWindow<Tools>();
            window.titleContent = new GUIContent("Tools");
            window.Show();
        }

        private TMP_FontAsset _tmpFontAsset;

        private void FontReplace()
        {
            _tmpFontAsset = (TMP_FontAsset) EditorGUILayout.ObjectField(_tmpFontAsset, typeof(TMP_FontAsset), true);

            if (_tmpFontAsset != null && GUILayout.Button("Replace font"))
            {
                var stageHandle = StageUtility.GetCurrentStageHandle();
                foreach (var text in stageHandle.FindComponentsOfType<TextMeshProUGUI>())
                {
                    if (PrefabUtility.GetPrefabAssetType(text.gameObject) == PrefabAssetType.NotAPrefab)
                    {
                        text.font = _tmpFontAsset;
                    }
                    else
                    {
                        Debug.Log(text.gameObject.name);
                    }
                }

                var currentPrefabStage = PrefabStageUtility.GetCurrentPrefabStage();
                if (currentPrefabStage != null)
                {
                    EditorSceneManager.MarkSceneDirty(currentPrefabStage.scene);
                }
                else
                {
                    EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
                }
            }
        }

        private void OnGUI()
        {
            // FontReplace();

            FindMushroomsWithAbilities();
        }

        private void FindMushroomsWithAbilities()
        {
            if (GUILayout.Button("Find mushrooms with abilities"))
            {
                foreach (var mushroom in FindObjectsOfType<GoodNeutralMushroom>())
                {
                    if (mushroom.mushroomDropAbility != GoodNeutralMushroom.AbilitySelect.none)
                    {
                        Debug.Log(
                            $"This mushroom has the {mushroom.mushroomDropAbility.ToString()}! Click this line to highlight it",
                            mushroom);
                    }
                }
            }
        }
    }
}