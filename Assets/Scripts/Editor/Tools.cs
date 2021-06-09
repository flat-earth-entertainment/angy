using System;
using Ball.Objectives;
using Config;
using TMPro;
using UnityEditor;
using UnityEditor.Experimental.SceneManagement;
using UnityEditor.SceneManagement;
using UnityEngine;
using Random = UnityEngine.Random;

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
            _tmpFontAsset = (TMP_FontAsset)EditorGUILayout.ObjectField(_tmpFontAsset, typeof(TMP_FontAsset), true);

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

        private GameObject _prefab;
        private bool _shouldPaint;
        private bool _randomizeY;
        private bool _align;

        private void OnEnable()
        {
            SceneView.duringSceneGui += OnSceneGui;
        }

        private void OnDisable()
        {
            SceneView.duringSceneGui -= OnSceneGui;
        }

        private Transform _drawnObjectsTransform;

        private void OnSceneGui(SceneView sceneView)
        {
            if (!_shouldPaint || _prefab == null)
                return;


            if (_shouldPaint && Event.current.type == EventType.MouseUp && Event.current.button == 0)
            {
                if (Physics.Raycast(HandleUtility.GUIPointToWorldRay(Event.current.mousePosition), out var hit,
                    Mathf.Infinity, LayerMask.GetMask("Ground")))
                {
                    var newObject = (GameObject)PrefabUtility.InstantiatePrefab(_prefab);
                    Selection.activeObject = newObject;
                    newObject.transform.position = hit.point;

                    if (_drawnObjectsTransform == null)
                    {
                        var foundObject = GameObject.Find("Drawn Objects");
                        if (foundObject != null)
                        {
                            _drawnObjectsTransform = foundObject.transform;
                        }
                        else
                        {
                            var newParent = new GameObject("Drawn Objects");
                            _drawnObjectsTransform = newParent.transform;
                        }
                    }

                    newObject.transform.parent = _drawnObjectsTransform;

                    if (_align)
                    {
                        switch (_normalOrient)
                        {
                            case NormalOrient.X:
                                newObject.transform.right = hit.point;
                                break;
                            case NormalOrient.Y:
                                newObject.transform.up = hit.point;
                                break;
                            case NormalOrient.Z:
                                newObject.transform.forward = hit.point;
                                break;
                        }
                    }

                    if (_randomizeY)
                    {
                        var rotation = newObject.transform.rotation;
                        rotation = Quaternion.Euler(rotation.eulerAngles.x,
                            Random.Range(-180, 181),
                            rotation.eulerAngles.z);
                        newObject.transform.rotation = rotation;
                    }

                    Undo.RegisterCreatedObjectUndo(newObject, "Spawned prefab");
                    EditorUtility.SetDirty(newObject);
                }
            }

            if (Event.current.type == EventType.Layout)
                HandleUtility.AddDefaultControl(GUIUtility.GetControlID(GetHashCode(), FocusType.Passive));
        }

        public enum NormalOrient
        {
            X,
            Y,
            Z
        }

        private NormalOrient _normalOrient;

        private void OnGUI()
        {
            _prefab = (GameObject)EditorGUILayout.ObjectField("Prefab:", _prefab, typeof(GameObject), false);

            _shouldPaint = GUILayout.Toggle(_shouldPaint, "Draw?");
            _randomizeY = GUILayout.Toggle(_randomizeY, "Randomize Y rotation?");
            _align = GUILayout.Toggle(_align, "Align with ground normal?");
            if (_align)
            {
                _normalOrient = (NormalOrient)EditorGUILayout.EnumPopup("Orient axis", _normalOrient);
            }
        }

        private void FindMushroomsWithAbilities()
        {
            if (GUILayout.Button("Find mushrooms with abilities"))
            {
                foreach (var mushroom in FindObjectsOfType<GoodNeutralMushroom>())
                {
                    if (mushroom.mushroomDropAbility != GoodNeutralMushroom.AbilitySelect.None)
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