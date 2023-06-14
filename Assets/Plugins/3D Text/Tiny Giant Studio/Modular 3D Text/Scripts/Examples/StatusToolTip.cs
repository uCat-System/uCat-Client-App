// You can use this one script for the entire scene
//1. Create text with the style you want
//2. Assign it as prefab to a styles list
//3. Call this script with ShowToolTip() method. Has multiple overloads


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MText
{
    public class StatusToolTip : MonoBehaviour
    {
        public List<Styles> styles = new List<Styles>();
        [Space]
        [Space]
        public float defaultDuration = 3;

        [Tooltip("This uses itself as pool holder, not poolmanager like the text and the text prefabs MUST have unique names because that's what is used for pool key")]
        public bool pooling = true;
        public Dictionary<string, Queue<GameObject>> poolDictionary = new Dictionary<string, Queue<GameObject>>();

        [System.Serializable]
        public class Styles
        {
            public GameObject textPrefab = null;
            public Vector3 offsetMin = Vector3.zero;
            public Vector3 offsetMax = Vector3.zero;
        }



        public void ShowToolTip(string text, int style, Vector3 position, Quaternion rotation, bool worldPosition)
        {
            ShowToolTip(text, style, position, rotation, worldPosition, defaultDuration);
        }
        public void ShowToolTip(string text, int style, Vector3 position, Quaternion rotation, bool worldPosition, float duration)
        {
            GameObject obj = GetIObj(style);
            SetPosition(obj.transform, style, position, rotation, worldPosition);
            obj.SetActive(true);
            ApplyText(text, obj);
            StartCoroutine(DestroyObj(obj, duration));
        }



        GameObject GetIObj(int style)
        {
            if (pooling)
            {
                string key = styles[style].textPrefab.name;
                if (poolDictionary.ContainsKey(key))
                {
                    if (poolDictionary[key].Count > 0)
                    {
                        GameObject poolItem = poolDictionary[key].Dequeue();
                        return poolItem;
                    }
                }
            }
            return GetNewObj(style);
        }

        private GameObject GetNewObj(int style)
        {
            GameObject newItem = Instantiate(styles[style].textPrefab);

            return newItem;
        }

        void SetPosition(Transform objTransform, int style, Vector3 position, Quaternion rotation, bool worldPosition)
        {
            if (worldPosition)
            {
                objTransform.position = position + RandomVector3(styles[style].offsetMin, styles[style].offsetMax);
                objTransform.rotation = rotation;
            }
            else
            {
                objTransform.localPosition = position + RandomVector3(styles[style].offsetMin, styles[style].offsetMax);
                objTransform.localRotation = rotation;
            }
        }

        Vector3 RandomVector3(Vector3 min, Vector3 max)
        {
            float x = Random.Range(min.x, max.x);
            float y = Random.Range(min.y, max.y);
            float z = Random.Range(min.z, max.z);
            return new Vector3(x, y, z);
        }

        void ApplyText(string text, GameObject obj)
        {
            obj.GetComponent<Modular3DText>().UpdateText(text);
        }
        IEnumerator DestroyObj(GameObject obj, float duration)
        {
            yield return new WaitForSeconds(duration);
            obj.GetComponent<Modular3DText>().UpdateText(string.Empty);
            yield return new WaitForSeconds(1);

            Destroy(obj);
        }
    }
}