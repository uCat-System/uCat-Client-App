using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using MText.EditorHelper;
using UnityEditor;
#endif

#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace MText
{
    [ExecuteAlways]
    public class AutoUpdateInputSystemToSampleScene : MonoBehaviour
    {
#if UNITY_EDITOR
        void Awake()
        {
#if ENABLE_INPUT_SYSTEM
            if (!gameObject.GetComponent<PlayerInput>())
                gameObject.AddComponent<PlayerInput>();
            MText_Settings settings = MText_FindResource.VerifySettings(null);

            if (settings)
            {
                if (!settings.inputActionAsset)
                {
                    string[] guids;

                    guids = AssetDatabase.FindAssets("t:inputActionAsset");
                    foreach (string guid in guids)
                    {
                        if (AssetDatabase.GUIDToAssetPath(guid).Contains("3D Text UI Controls.inputactions"))
                        {
                            settings.inputActionAsset = AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(guid), typeof(InputActionAsset)) as InputActionAsset;
                            EditorUtility.SetDirty(settings);
                            break;
                        }
                    }
                }
                gameObject.GetComponent<PlayerInput>().actions = settings.inputActionAsset;
            }
#endif
        }
#endif
    }
}