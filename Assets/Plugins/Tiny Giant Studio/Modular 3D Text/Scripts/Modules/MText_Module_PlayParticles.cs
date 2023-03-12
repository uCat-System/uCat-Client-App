using System.Collections;
using UnityEngine;

namespace MText
{
    /// <summary>
    /// Variable holders:
    /// Index 0: Delay - Float
    /// Index 1: Particle Prefab - GameObject
    /// Index 2: Dont Auto Destroy Particle - Bool
    /// Index 3: Destroy Particle After - Float
    /// </summary>
    [CreateAssetMenu(menuName = "Modular 3d Text/Modules/Play Particle")]
    public class MText_Module_PlayParticles : MText_Module
    {
        public override IEnumerator ModuleRoutine(GameObject obj, VariableHolder[] variableHolders)
        {
            if (variableHolders[0].floatValue > 0)
                yield return new WaitForSeconds(variableHolders[0].floatValue); //even with 0, this was causing particle to not spawn.

            if (obj)
            {
                if (variableHolders[1].gameObjectValue)
                {
                    GameObject spawnedParticle = Instantiate(variableHolders[1].gameObjectValue);
                    spawnedParticle.transform.SetPositionAndRotation(obj.transform.position, obj.transform.rotation);

                    if (spawnedParticle.GetComponent<ParticleSystem>())
                        spawnedParticle.GetComponent<ParticleSystem>().Play();

                    Destroy(spawnedParticle, variableHolders[3].floatValue);
                }
            }
        }

        public override string VariableWarnings(VariableHolder[] variableHolders)
        {
            string warning = string.Empty;

            if (variableHolders == null)
                return warning;

            if (variableHolders.Length < 1)
                return warning;

            try
            {
                if (variableHolders[1].gameObjectValue == null)
                {
                    warning = AddWarning("Please specify a particle prefab to spawn.", warning);
                }
                if (!variableHolders[2].boolValue)
                {
                    if (variableHolders[3].floatValue <= 0)
                    {
                        warning = AddWarning("Invalid particle auto destroy timer", warning);
                    }
                }
            }
            catch
            {
                return warning;
            }
            return warning;
        }
    }
}