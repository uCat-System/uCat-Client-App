using System.Collections;
using UnityEngine;


namespace MText
{
    [HelpURL("https://app.gitbook.com/@ferdowsur/s/modular-3d-text/scripts/modules")]
    public abstract class MText_Module : ScriptableObject
    {
        [HideInInspector]
        public VariableHolder[] variableHolders;
        public abstract IEnumerator ModuleRoutine(GameObject obj, VariableHolder[] variableHolders);

        /// <summary>
        /// The return of this string is shown as warning. Used to show warnings incase of faulty settings
        /// </summary>
        /// <returns></returns>
        public abstract string VariableWarnings(VariableHolder[] variableHolders);

        public string AddWarning(string toAdd, string original)
        {
            if (!string.IsNullOrEmpty(original))
                original += '\n';
            original += toAdd;

            return original;
        }
    }
}
