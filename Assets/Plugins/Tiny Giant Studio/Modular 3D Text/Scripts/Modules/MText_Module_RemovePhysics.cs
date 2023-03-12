/// Created by Ferdowsur Asif @ Tiny Giant Studios


using System.Collections;
using UnityEngine;

namespace MText
{
    /// <summary>
    /// Variable Holders:
    /// 0 - Delay - Float
    /// </summary>
    [CreateAssetMenu(menuName = "Modular 3d Text/Modules/Remove Physics")]
    public class MText_Module_RemovePhysics : MText_Module
    {
        public override IEnumerator ModuleRoutine(GameObject obj, VariableHolder[] variableHolders)
        {
            yield return new WaitForSeconds(variableHolders[0].floatValue);
            if (obj)
            {
                if (obj.GetComponent<BoxCollider>())
                {
                    Destroy(obj.GetComponent<Rigidbody>());
                }
                if (obj.GetComponent<Rigidbody>())
                {
                    Destroy(obj.GetComponent<Rigidbody>());
                }
            }
        }
        public override string VariableWarnings(VariableHolder[] variableHolders)
        {
            return null;
        }
    }
}
