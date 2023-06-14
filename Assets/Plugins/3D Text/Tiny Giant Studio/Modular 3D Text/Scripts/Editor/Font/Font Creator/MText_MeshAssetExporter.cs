using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace MText.FontCreation
{
    public class MText_MeshAssetExporter
    {
        public void DoExport(GameObject targetObj)
        {
            string folderName = EditorUtility.OpenFolderPanel("Select mesh location", "Assets", "");
            folderName = FileUtil.GetProjectRelativePath(folderName);
            if (string.IsNullOrEmpty(folderName)) return;

            string fontName = targetObj.name;
            string guid = AssetDatabase.CreateFolder(folderName, fontName);
            string newFolderPath = AssetDatabase.GUIDToAssetPath(guid);


            MText_Font newFont = ScriptableObject.CreateInstance<MText_Font>();



            List<string> meshPaths = new List<string>();


            Transform targetTrasnform = targetObj.transform;
            for (int i = 0; i < targetTrasnform.childCount; i++)
            {
                string savePath = newFolderPath + "/" + targetTrasnform.GetChild(i).gameObject.name + ".asset";
                AssetDatabase.CreateAsset(targetTrasnform.GetChild(i).gameObject.GetComponent<MeshFilter>().sharedMesh, savePath);
                AssetDatabase.ImportAsset(savePath);
                meshPaths.Add(savePath);
            }

            EditorApplication.delayCall += () => AddCharacterToFont(newFont, meshPaths, fontName);
        }


        private void AddCharacterToFont(MText_Font newFont, List<string> meshPaths, string fontName)
        {
            string scriptableObjectSaveLocation = EditorUtility.SaveFilePanel("Save font location", "", fontName, "asset");
            scriptableObjectSaveLocation = FileUtil.GetProjectRelativePath(scriptableObjectSaveLocation);

            for (int i = 0; i < meshPaths.Count; i++)
            {
                Mesh newAsset = (Mesh)AssetDatabase.LoadAssetAtPath(meshPaths[i], typeof(Mesh));
                newFont.AddCharacter(newAsset);
            }

            AssetDatabase.CreateAsset(newFont, scriptableObjectSaveLocation);
            AssetDatabase.SaveAssets();
        }
    }
}
