/// Created by Ferdowsur Asif @ Tiny Giant Studios

using System.Collections;
using UnityEngine;

namespace MText
{
    public class DamageText : MonoBehaviour
    {
        public Modular3DText modular3DText = null;
        public Transform textHolder = null;
        public MText_Module module = null;



        public void UpdateText(string str)
        {
            ResetTransform();
            modular3DText.UpdateText(str);
            StartCoroutine(ApplyModules());
        }
        public void UpdateText(float number)
        {
            ResetTransform();
            modular3DText.UpdateText(number);
            StartCoroutine(ApplyModules());
        }
        public void UpdateText(int number)
        {
            ResetTransform();
            modular3DText.UpdateText(number);
            StartCoroutine(ApplyModules());
        }

        void ResetTransform()
        {
            textHolder.localPosition = Vector3.zero;
            textHolder.localRotation = Quaternion.identity;

            if (textHolder.gameObject.GetComponent<Rigidbody>())
            {
                textHolder.gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
                textHolder.gameObject.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
            }
        }

        IEnumerator ApplyModules()
        {
            if (gameObject.activeInHierarchy)
            {
                yield return null;

                if (module)
                    StartCoroutine(module.ModuleRoutine(textHolder.gameObject, null));
            }
        }
    }
}
