using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace MText.FontCreation
{
    public class MText_FontExporter
    {
        public void CreateFontFile(string prefabPath, string fontName, MText_CharacterGenerator fontCreator, byte[] fontData)
        {
            MText_Font newFont = ScriptableObject.CreateInstance<MText_Font>();
            newFont.fontBytes = fontData;
            newFont.averageYValue = fontCreator.averageYValue;
            newFont.GetMonoSpacingFromAverageCharacterSpacing();
            GameObject fontSet = AssetDatabase.LoadAssetAtPath(prefabPath, typeof(GameObject)) as GameObject;


#if UNITY_EDITOR
            {
                ModelImporter importer = ModelImporter.GetAtPath(prefabPath) as ModelImporter;

                importer.isReadable = true;
                importer.materialImportMode = ModelImporterMaterialImportMode.None;
                importer.importAnimation = false;

                importer.SaveAndReimport();
            }
#endif


            newFont.UpdateCharacterList(fontSet);
            for (int i = 0; i < newFont.characters.Count; i++)
            {
                newFont.characters[i].glyphIndex = fontCreator.Index(newFont.characters[i].character);
                newFont.characters[i].spacing = fontCreator.GetChracterAdvance(newFont.characters[i].glyphIndex);
            }

            if (fontCreator.KerningSupported())
                newFont = GetKerning(fontCreator, newFont);
            //else
            //    Debug.Log("Kerning not supported");

            PassFontInformations(fontCreator, newFont);

            string scriptableObjectSaveLocation = EditorUtility.SaveFilePanel("Save font location", "", fontName, "asset");
            scriptableObjectSaveLocation = FileUtil.GetProjectRelativePath(scriptableObjectSaveLocation);
            AssetDatabase.CreateAsset(newFont, scriptableObjectSaveLocation);
            AssetDatabase.SaveAssets();



        }

        private MText_Font GetKerning(MText_CharacterGenerator fontCreator, MText_Font newFont)
        {
            fontCreator.GetKerningInfo(out List<ushort> lefts, out List<ushort> rights, out List<short> offsets);

            for (int i = 0; i < lefts.Count; i++)
            {
                //kerning settings need to be redone
                newFont.kernTable.Add(new MText_KernPairHolder(new MText_KernPair(lefts[i], rights[i]), offsets[i]));
            }

            return newFont;
        }

        private void PassFontInformations(MText_CharacterGenerator fontCreator, MText_Font newFont)
        {
            float unitsPerEM = 1;
            float originalLineHeight = 0;
            float whiteSpaceSpacing = 1;

            fontCreator.GetTypeFaceInformation(ref originalLineHeight, ref unitsPerEM, ref whiteSpaceSpacing);

            newFont.unitPerEM = unitsPerEM;
            newFont.lineHeight = originalLineHeight;
            //newFont.lineHeight = (originalLineHeight / (unitsPerEM / 1000)) * 0.00175f; //Here, 1000 is the default EM. No specific reason
            newFont.emptySpaceSpacing = whiteSpaceSpacing;
        }
    }
}
