#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


namespace MText
{
    [ExecuteInEditMode]
    public class MText_SampleScene_ModuleTutorial : MonoBehaviour
    {
        [SerializeField] SpriteRenderer spriteRenderer;
        [SerializeField] GameObject target;

        [SerializeField] Sprite pressPlaySprite;
        [SerializeField] Sprite selectTextSprite;
        [SerializeField] Sprite openModuleSprite;
        [SerializeField] Sprite addModuleSprite;
        [SerializeField] Sprite selectModuleSprite;
        [SerializeField] Sprite modifyModuleSprite;
        [SerializeField] Sprite doSameForDeleteSprite;

        void Update()
        {
            if (!Application.isPlaying)
            {
                spriteRenderer.sprite = pressPlaySprite;
                return;
            }
            if (!Selection.Contains(target))
            {
                spriteRenderer.sprite = selectTextSprite;
                return;
            }
            Modular3DText text = target.gameObject.GetComponent<Modular3DText>();

            //if (text.showModuleSettingsInEditor.faded == 0)
            //{
            //    spriteRenderer.sprite = openModuleSprite;
            //    return;
            //}
            if (text.addingModules.Count == 0)
            {
                spriteRenderer.sprite = addModuleSprite;

            }
            else if (!text.addingModules[0].module)
            {
                spriteRenderer.sprite = selectModuleSprite;
            }
            else if (text.addingModules[0].variableHolders.Length <= 2)
                return;
            else if (text.addingModules[0].variableHolders[1].floatValue == 0 || text.addingModules[0].variableHolders[4].boolValue == false)
            {
                spriteRenderer.sprite = modifyModuleSprite;
            }
            else
            {
                spriteRenderer.sprite = doSameForDeleteSprite;
            }
        }
    }
}
#endif
