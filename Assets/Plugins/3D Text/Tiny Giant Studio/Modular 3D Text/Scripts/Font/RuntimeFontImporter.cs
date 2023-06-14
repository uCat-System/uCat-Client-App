//using System.IO;
using UnityEngine;

namespace MText
{
    /// <summary>
    /// This handles importing font files during runtime
    /// </summary>
    public class RuntimeFontImporter : MonoBehaviour
    {
        //[SerializeField] string path;

        //[ContextMenu("Test")]
        //public void Test()
        //{
        //    byte[] fontdata = File.ReadAllBytes(path);
        //    CreateFontFromTTFFile(fontdata);
        //}

        public MText_Font CreateFontFromTTFFile(byte[] rfontBytes)
        {
            MText_Font font = ScriptableObject.CreateInstance<MText_Font>();

            font.SetFontBytes(rfontBytes);
            font.GetTypeFaceFromBytes(); 

            font.unitPerEM = font.TypeFace.unitsPerEm;
            font.lineHeight = font.TypeFace.lineHeight;

            float emptySpaceSpacing = 200;

            font.TypeFace.SetGlyphData(' ');
            if (font.TypeFace.glyphs.ContainsKey(' '))
            {
                int index = font.TypeFace.glyphs[' '].glyphIndex;

                if (index < font.TypeFace.advanceArray.Length && index >= 0)
                {                  
                    emptySpaceSpacing = font.TypeFace.advanceArray[index];
                }
            }

            font.emptySpaceSpacing = emptySpaceSpacing;

            return font;
        }

    }
}