using UnityEditor;
using UnityEngine;

namespace MText.EditorHelper
{
    public class MText_FindResource : MonoBehaviour
    {
#if UNITY_EDITOR
        public static MText_Settings VerifySettings(MText_Settings settings)
        {
            MText_Settings mySettings = Resources.Load("Modular 3D Text/M Text_Settings") as MText_Settings;
            if (mySettings)
                return mySettings;

            var objects = Resources.FindObjectsOfTypeAll(typeof(MText_Settings));

            if (objects.Length > 1)
            {
                Debug.LogWarning("Multiple MText_Settings files have been found. Please make sure only one exists to avoid unexpected behavior");
                for (int i = 0; i < objects.Length; i++)
                {
                    Debug.Log("Setting file " + (i + 1) + " : " + AssetDatabase.GetAssetPath(objects[i]));
                }
            }
            if (objects.Length == 0)
            {
                Debug.LogWarning("No settings file for Modulur 3D text was found. Creating one");
                MText_Settings asset = ScriptableObject.CreateInstance<MText_Settings>();
                AssetDatabase.CreateAsset(asset, "Assets/MText_Settings.asset");
                AssetDatabase.SaveAssets();
            }

            if (settings == null && objects.Length > 0)
                settings = (MText_Settings)objects[0];

            return settings;
        }
#endif
    }
}