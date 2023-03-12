/// Created by Ferdowsur Asif @ Tiny Giant Studio

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MText
{
    [RequireComponent(typeof(Modular3DText))]
    [AddComponentMenu("Modular 3D Text/Extra/Loop Animation")]
    public class MText_LoopAnimation : MonoBehaviour
    {
        [SerializeField] Vector2 duration = new Vector2(1, 2);
        [SerializeField] TargetType targetType = TargetType.letters;
        [Space]
        [SerializeField] Material focusedMaterial = null;

        //[HideInInspector]
        public List<GameObject> targetLetterList = new List<GameObject>();
        public List<List<GameObject>> targetWordsList = new List<List<GameObject>>();
        Modular3DText Modular3DText => GetComponent<Modular3DText>();


        private enum TargetType
        {
            letters,
            words
        }

        private void OnEnable()
        {
            UpdateTargetList();

            if (targetType == TargetType.letters)
                StartCoroutine(LetterAnimationRoutine());
            else
                StartCoroutine(WordAnimationRoutine());
        }

        /// <summary>
        /// This needs to be called everytime the text is changed
        /// </summary>
        public void UpdateTargetList()
        {
            targetLetterList.Clear();
            targetWordsList.Clear();

            if (targetType == TargetType.letters)
            {
                for (int i = 0; i < Modular3DText.characterObjectList.Count; i++)
                {
                    targetLetterList.Add(Modular3DText.characterObjectList[i]);
                }
            }
            else
            {
                List<GameObject> letterList = new List<GameObject>();
                for (int i = 0; i < Modular3DText.characterObjectList.Count; i++)
                {
                    if (Modular3DText.characterObjectList[i].name == "Space")
                    {
                        targetWordsList.Add(letterList);
                        letterList = new List<GameObject>();
                    }
                    else
                    {
                        letterList.Add(Modular3DText.characterObjectList[i]);
                    }
                }
                if (letterList.Count > 0)
                {
                    targetWordsList.Add(letterList);
                }
            }
        }

        private IEnumerator LetterAnimationRoutine()
        {
            yield return null;
            for (int i = 0; i < targetLetterList.Count; i++)
            {
                Focus(targetLetterList[i]);
                yield return new WaitForSeconds(Random.Range(duration.x, duration.y));
                UnFocus(targetLetterList[i]);
            }

            StartCoroutine(LetterAnimationRoutine());
        }
        private IEnumerator WordAnimationRoutine()
        {
            yield return null;
            for (int i = 0; i < targetWordsList.Count; i++)
            {
                for (int j = 0; j < targetWordsList[i].Count; j++)
                {
                    Focus(targetWordsList[i][j]);
                }

                yield return new WaitForSeconds(Random.Range(duration.x, duration.y));

                for (int j = 0; j < targetWordsList[i].Count; j++)
                {
                    UnFocus(targetWordsList[i][j]);
                }
            }

            StartCoroutine(WordAnimationRoutine());
        }


        private void Focus(GameObject target)
        {
            if (!target)
                return;

            if (focusedMaterial)
            {
                if (target.GetComponent<Renderer>())
                    target.GetComponent<Renderer>().material = focusedMaterial;
            }
        }
        private void UnFocus(GameObject target)
        {
            if (!target)
                return;

            if (target.GetComponent<Renderer>())
                target.GetComponent<Renderer>().material = Modular3DText.Material;

        }
    }
}