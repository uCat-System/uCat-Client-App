using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace MText
{
    /// <summary>
    /// Apologies for my poor explaination:
    /// 
    /// Inside the editor, Unity doesn't let certain tasks to be done on the same frame the script calls them.
    /// So, they have to created with EditorApplication.delayCall.
    /// A lot of things can cause issues with this. 
    /// For example: if the script reloads while a task is on delaycall, the reloaded script isn't the one that called the delaycall. 
    /// This can result in leftover letter getting created but no text that they belong to. (Usually as a copy).
    /// This is super rare but can be annoying if it happens
    /// 
    /// This script handles this issue.
    /// </summary>

    [ExecuteAlways]
    public class DelayCallCleanUp : MonoBehaviour
    {
#if UNITY_EDITOR
        public Modular3DText text;
        public bool textContainsThis = false;    //TODO //this is for debugging

        [ExecuteAlways]
        void Start()
        {
            if (Application.isPlaying)
                return;

            if (text == null)
                EditorApplication.delayCall += () => DestroyImmediate(gameObject);
            else
            {
                if (text._allcharacterObjectList.Contains(gameObject))
                {
                    textContainsThis = true;
                    DestroyImmediate(this);
                }
                else
                    EditorApplication.delayCall += () => DestroySelf();
            }
        }
        void DestroySelf()
        {
            if (!this)
                return;
            if (!gameObject)
                return;

            try //editor only
            {
                DestroyImmediate(gameObject);
            }
            catch
            {

            }
        }
#endif
    }
}