/// Created by Ferdowsur Asif @ Tiny Giant Studios


using System.Collections;
using UnityEngine;

namespace MText
{
    /// <summary>
    /// Variable Holders:
    /// 0 - Delay - Float
    /// 1 - Enable gravity - bool
    /// 2 - Don't add force - bool
    /// 3 - Horizontal Foce - Float
    /// 4 - Vertical Foce - Float
    /// 5 - Force Direction Min - Vector3
    /// 6 - Force Direction Max - Vector3
    /// 7 - Physic Material - Physic Material
    /// </summary>
    [CreateAssetMenu(menuName = "Modular 3d Text/Modules/Add Physics")]
    public class MText_Module_AddPhysics : MText_Module
    {
        public override IEnumerator ModuleRoutine(GameObject obj, VariableHolder[] variableHolders)
        {
            yield return new WaitForSeconds(variableHolders[0].floatValue);

            if (obj)
            {
                if (obj.GetComponent<MeshFilter>())
                {
                    if (!obj.GetComponent<Rigidbody>())
                        obj.AddComponent<Rigidbody>();


                    if (!obj.GetComponent<BoxCollider>())
                        obj.AddComponent<BoxCollider>();

                    if (variableHolders[7].physicMaterialValue)
                        obj.GetComponent<BoxCollider>().material = variableHolders[7].physicMaterialValue;

                    obj.GetComponent<Rigidbody>().useGravity = variableHolders[1].boolValue;

                    if (!variableHolders[2].boolValue)
                    {
                        float horizontalForcePower = variableHolders[3].floatValue;
                        float verticalForcePower = variableHolders[4].floatValue;
                        Vector3 forceDirectionMinimum = variableHolders[5].vector3Value;
                        Vector3 forceDirectionMaximum = variableHolders[6].vector3Value;
                        obj.GetComponent<Rigidbody>().AddForce(new Vector3(horizontalForcePower * Random.Range(forceDirectionMinimum.x, forceDirectionMaximum.x), verticalForcePower * Random.Range(forceDirectionMinimum.y, forceDirectionMaximum.y), horizontalForcePower * Random.Range(forceDirectionMinimum.z, forceDirectionMaximum.z)));
                    }
                }
            }
        }
        public override string VariableWarnings(VariableHolder[] variableHolders)
        {
            return null;
        }
    }
}

