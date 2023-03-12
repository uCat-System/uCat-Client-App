using System.Collections.Generic;
using UnityEngine;

namespace MText
{
    public class MText_SampleScene_FontTest : MonoBehaviour
    {
        [SerializeField] Modular3DText modular3DText = null;
        [SerializeField] Modular3DText fontText = null;

        [Space]

        [SerializeField] List<MText_Font> fonts = new List<MText_Font>();
        int selectedFont = 0;

        public void NextFont()
        {
            selectedFont++;
            if (selectedFont >= fonts.Count) selectedFont = 0;

            UpdateInfo();
        }

        public void PreviousFont()
        {
            selectedFont--;
            if (selectedFont < 0) selectedFont = fonts.Count - 1;

            UpdateInfo();
        }

        void UpdateInfo()
        {
            modular3DText.Font = fonts[selectedFont];
            modular3DText.UpdateText();
            fontText.Font = fonts[selectedFont];
            fontText.UpdateText(fonts[selectedFont].name);
        }
    }
}