using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif


namespace MText
{
    [DisallowMultipleComponent]
    [ExecuteAlways]
    [AddComponentMenu("Modular 3D Text/Text Updater (M3D)")]
    [HelpURL("https://app.gitbook.com/@ferdowsur/s/modular-3d-text/~/drafts/-Mg0jgJfqcpdm2-Mbd5o/scripts/text-updater")]
    public class MText_TextUpdater : MonoBehaviour
    {
#if UNITY_EDITOR
        [HideInInspector]
        [SerializeField]
        private int openTime = 0;
#endif

        Modular3DText Text => GetComponent<Modular3DText>();




        [ExecuteAlways]
        private void Awake()
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
                this.enabled = true;

            if (openTime < 5)
            {
                openTime++;
                if (openTime < 2)
                    return;
            }
            PrefabUtility.prefabInstanceUpdated += OnPrefabInstanceUpdated;
#endif

            if (!Text)
                return;

            if (EmptyText(Text))
                Text.UpdateText();
        }



#if UNITY_EDITOR
        void OnPrefabInstanceUpdated(GameObject instance)
        {
            EditorApplication.delayCall += () => CheckPrefab();
        }

        private void CheckPrefab()
        {
            if (!this)
                return;

            if (Text == null)
                return;

            if (!Text.Font)
                return;

            bool prefabConnected = PrefabUtility.GetPrefabInstanceStatus(this.gameObject) == PrefabInstanceStatus.Connected;
            if (prefabConnected)
            {
                EditorApplication.delayCall += () => UpdateText();
            }
        }

        private void UpdateText()
        {
            if (!this)
                return;

            if (Text == null)
                return;

            if (Text)
            {
                if (!Text.updatedAfterStyleUpdateOnPrefabInstances)
                {
                    if (Text.debugLogs)
                        Debug.Log("Text updated due to prefab update.");
                    Text.CleanUpdateText();
                    Text.updatedAfterStyleUpdateOnPrefabInstances = true;//buggy
                }
            }
        }
#endif



        private bool EmptyText(Modular3DText text)
        {
            if (string.IsNullOrEmpty(text.Text))
            {
                return false;
            }

            if (gameObject.GetComponent<MeshFilter>())
            {
                if (gameObject.GetComponent<MeshFilter>().sharedMesh != null)
                {
                    return false;
                }
            }

            if (text.characterObjectList.Count > 0)
            {
                for (int i = 0; i < text.characterObjectList.Count; i++)
                {
                    if (text.characterObjectList[i])
                    {
                        return false;
                    }
                }
            }


            return true;
        }
    }
}