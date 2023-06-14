using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace MText.FontCreation
{
    public class MText_NewFontCharacterRange
    {
        public List<char> RetrieveCharacterListFromUnicodeSequence(string sequence)
        {
            List<char> charList = new List<char>();

            //Remove spaces
            sequence = sequence.Replace(" ", string.Empty);

            //split codes by comma
            string[] splitCodes = sequence.Split(',');


            for (int i = 0; i < splitCodes.Length; i++)
            {
                //check for ranges
                string[] codeRange = splitCodes[i].Split('-', '-', '–'); // one is minus from numpade, one is beside backspace. are they really different? //TODO
                if (codeRange.Length == 2)
                {
                    List<char> chars = RetrieveCharactersList(ConvertCharFromUnicode(codeRange[0]), ConvertCharFromUnicode(codeRange[1]));
                    charList.AddRange(chars);
                }
                //not range
                else if (codeRange.Length < 2)
                {
                    charList.Add(ConvertCharFromUnicode(splitCodes[i]));
                }
            }
            return charList;
        }




        public List<char> RetrieveCharactersList(char start, char end)
        {
            List<char> newCharacterList = new List<char>();

            for (char letter = start; letter <= end; letter++)
            {
                newCharacterList.Add(letter);
            }

            return newCharacterList;
        }

        private char ConvertCharFromUnicode(string unicode)
        {
            string s = Regex.Unescape("\\u" + unicode);

            s.ToCharArray();
            if (s.Length > 0)
                return s[0];
            else
                return ' ';
        }

    }
}