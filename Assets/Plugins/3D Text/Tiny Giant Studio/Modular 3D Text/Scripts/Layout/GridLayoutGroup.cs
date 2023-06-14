using System;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif


namespace MText
{
    [HelpURL("https://app.gitbook.com/@ferdowsur/s/modular-3d-text/scripts/layout-group-1")]
    [AddComponentMenu("Modular 3D Text/Layout/Grid Layout Group (M3D)")]
    public class GridLayoutGroup : LayoutGroup
    {
        [SerializeField]
        private Alignment _anchor = Alignment.MiddleCenter;
        public Alignment Anchor
        {
            get { return _anchor; }
            set
            {
                _anchor = value;
                if (!UpdateTextIfRequired())
                    UpdateLayout();
            }
        }
        [Tooltip("Elements will be split to fill the entire width")]
        [SerializeField]
        private bool _justiceHorizontal = false;
        public bool JusticeHorizontal
        {
            get { return _justiceHorizontal; }
            set
            {
                _justiceHorizontal = value;
                if (!UpdateTextIfRequired())
                    UpdateLayout();
            }
        }

        [Tooltip("Justice will be only be applied if the elements hold equal/more than the % width")]
        [SerializeField]
        private float _justiceHorizontalPercent = 70;
        public float JusticeHorizontalPercent
        {
            get { return _justiceHorizontalPercent; }
            set
            {
                _justiceHorizontalPercent = value;
                if (!UpdateTextIfRequired())
                    UpdateLayout();
            }
        }

        [Tooltip("Elements will be split to fill the entire width")]
        [SerializeField]
        private bool _justiceVertical = false;
        public bool JusticeVertical
        {
            get { return _justiceVertical; }
            set
            {
                _justiceVertical = value;
                if (!UpdateTextIfRequired())
                    UpdateLayout();
            }
        }

        [Tooltip("Justice will be only be applied if the elements hold equal/more than the % height")]
        [SerializeField]
        private float _justiceVerticalPercent = 70;
        public float JusticeVerticalPercent
        {
            get { return _justiceVerticalPercent; }
            set
            {
                _justiceVerticalPercent = value;
                if (!UpdateTextIfRequired())
                    UpdateLayout();
            }
        }

        [SerializeField]
        private Vector2 _spacing = new Vector2(5, 5);
        /// <summary>
        /// This will always return _spacing/100
        /// </summary>
        public Vector2 Spacing
        {
            get { return _spacing / 100; }
            set
            {
                _spacing = value;

                if (!UpdateTextIfRequired())
                    UpdateLayout();
            }
        }




        [SerializeField]
        private float _width = 23;
        public float Width
        {
            get { return _width; }
            set
            {
                _width = value;

                if (!UpdateTextIfRequired())
                    UpdateLayout();
            }
        }

        [SerializeField]
        private float _height = 4;
        public float Height
        {
            get { return _height; }
            set
            {
                _height = value;

                if (!UpdateTextIfRequired())
                    UpdateLayout();
            }
        }

        [SerializeField]
        private LineSpacingStyle _lineSpacingStyle = LineSpacingStyle.maximum;
        public LineSpacingStyle MyLineSpacingStyle
        {
            get { return _lineSpacingStyle; }
            set
            {
                _lineSpacingStyle = value;

                if (!UpdateTextIfRequired())
                    UpdateLayout();
            }
        }

        //#if UNITY_EDITOR
        //        private readonly float depth = 0f;
        //#endif

        private int currentLine = 0;
        private int currentLetter;
        private int currentWord;

        public List<Line> lines = new List<Line>();
        [Serializable]
        public class Line
        {
            public List<GameObject> gameObjects = new List<GameObject>();
            public List<MeshLayout> meshLayouts = new List<MeshLayout>();
#if UNITY_EDITOR
            /// <summary>
            /// Editor only
            /// </summary>
            [Tooltip("Editor only. Not included in build")]
            public float width;
            /// <summary>
            /// Editor only
            /// </summary>
            [Tooltip("Editor only. Not included in build")]
            public float individualYSpace;
            /// <summary>
            /// Editor only
            /// </summary>
            [Tooltip("Editor only. Not included in build")]
            public float widthIfNextObjectWasHere;
            /// <summary>
            /// Editor only
            /// </summary>
            [Tooltip("Editor only. Not included in build")]
            public string editorNote;
#endif
        }


        [ContextMenu("Update Layout")]
        public override void UpdateLayout()
        {
            bounds = GetAllChildBounds();

            Modular3DText text = GetComponent<Modular3DText>();

            lines = GetLines(bounds); //TODO: get text as parameter

            float ySpace = GetYSpace(text);
            float y = -StartingY(ySpace);
            float z = 0;

            for (int lineCount = 0; lineCount < lines.Count; lineCount++)
            {
                float spaceRequired = GetXSpaceRequired(lines[lineCount]);
                float justiceXMultiplier = GetJusticeXMultiplier(spaceRequired);

                float x = StartingX(lines[lineCount].gameObjects, spaceRequired);

                if (!text && MyLineSpacingStyle == LineSpacingStyle.individual)
                {
                    ySpace = GetSpecificLineMaxYSpace(lines[lineCount].gameObjects);
#if UNITY_EDITOR
                    lines[lineCount].individualYSpace = ySpace;
#endif
                }

                y -= (Spacing.y + ySpace) / 2;

                for (int i = 0; i < lines[lineCount].gameObjects.Count; i++)
                {
                    Bounds bound = GetBound(lines[lineCount].gameObjects[i].transform);

                    float spaceX = ((Spacing.x + bound.size.x) / 2) * justiceXMultiplier;
                    x += spaceX;

                    Vector3 targetPos = new Vector3(x - bound.center.x, y, z);

                    if (targetPos != lines[lineCount].gameObjects[i].transform.localPosition)
                        lines[lineCount].gameObjects[i].transform.localPosition = targetPos;

                    x += spaceX;
                }

                y -= (Spacing.y + ySpace) / 2;
            }
        }

        private float GetYSpace(Modular3DText text)
        {
            if (MyLineSpacingStyle == LineSpacingStyle.maximum || text)
                return GetMaxYSpace(bounds);
            else if (MyLineSpacingStyle == LineSpacingStyle.average)
                return GetAverageYSpace(bounds);

            //individual
            return 1;
        }

        /// <summary>
        /// Used by Text only
        /// </summary>
        /// <param name="meshLayouts"></param>
        /// <returns></returns>
        public override List<MeshLayout> GetPositions(List<MeshLayout> meshLayouts)
        {
            if (meshLayouts.Count == 0)
                return null;

            Bounds[] bounds = GetAllChildBounds(meshLayouts);

            lines.Clear();
            lines = GetLines(bounds, meshLayouts);

            float ySpace = GetMaxYSpace(bounds);
            float y = -StartingY(ySpace);
            float z = 0;


            for (int lineCount = 0; lineCount < lines.Count; lineCount++)
            {
                float spaceRequired = GetXSpaceRequired(lines[lineCount]);
                float justiceMultiplier = GetJusticeXMultiplier(spaceRequired);

                float x = StartingX(lines[lineCount].meshLayouts, spaceRequired);

                y -= (Spacing.y + ySpace) / 2;
                for (int i = 0; i < lines[lineCount].meshLayouts.Count; i++)
                {
                    Bounds bound = GetBound(lines[lineCount].meshLayouts[i]);

                    float spaceX = ((Spacing.x + bound.size.x) / 2) * justiceMultiplier;
                    x += spaceX;


                    Vector3 targetPos = new Vector3(x - bound.center.x, y, z);

                    //if (targetPos != lines[lineCount].gameObjects[i].transform.localPosition)
                    lines[lineCount].meshLayouts[i].position = targetPos;

                    x += spaceX;
                }
                y -= (Spacing.y + ySpace) / 2;
            }

            return meshLayouts;
        }
        bool ApplyHorizontalJustice(float spaceRequired)
        {
            if (!JusticeHorizontal)
            {
                return false;
            }
            else
            {
                if ((spaceRequired / Width) * 100 >= JusticeHorizontalPercent)
                    return true;
                else
                    return false;
            }
        }
        float GetJusticeXMultiplier(float spaceRequired)
        {
            if (ApplyHorizontalJustice(spaceRequired))
                return Width / spaceRequired;

            return 1;
        }


        /// <summary>
        /// Used by 3d Text only when single mesh is used
        /// </summary>
        /// <param name="bounds"></param>
        /// <returns></returns>
        private List<Line> GetLines(Bounds[] bounds, List<MeshLayout> meshLayouts)
        {
            List<Line> lines = new List<Line>();

            Line line = new Line();
            lines.Add(line);

            float x = 0;

            Modular3DText text = GetComponent<Modular3DText>();
            currentLetter = 0;
            currentWord = 0;
            currentLine = 0;

            bool checkedIfFits = false; //This checks if a word in text is already tested to fit.

            for (int i = 0; i < bounds.Length; i++)
            {


                IsThisAnewWord(text, ref checkedIfFits, i);

                ///New line
                if (ItsALineBreak(meshLayouts, lines, ref currentLine, ref x, i))
                {
                    currentLetter = 0;
                    currentWord++;
                    continue;
                }

#if UNITY_EDITOR
                lines[currentLine].widthIfNextObjectWasHere = x + Spacing.x + bounds[i].size.x;
#endif

                //Debug.Log("current word number:" + currentWordNumber);

                //Get current line number
                //Willl it be new line if current object is added
                if ((float)Math.Round(x + Spacing.x + bounds[i].size.x, 5) > Width) //New line 
                {
                    x = bounds[i].size.x + Spacing.x;
                    Line newLine = new Line();
                    lines.Add(newLine);

                    MoveWordToNextLine(meshLayouts, bounds, lines, currentLine, ref x, text, currentWord, ref checkedIfFits, i);

                    //Debug.Log(" New line ");
                    currentLine++;
                }
                else
                {
                    x += bounds[i].size.x + Spacing.x;
#if UNITY_EDITOR
                    if (x != 0)
                        lines[currentLine].width = x;
#endif
                }

                lines[currentLine].meshLayouts.Add(meshLayouts[i]);
                currentLetter++;
            }

            if (text)
                lines = RemoveUnnecessarySpaces(lines, true);

            if (lines.Count > 0)
            {
                //remove empty first line
                if (lines[0].gameObjects.Count == 0 && lines[0].meshLayouts.Count == 0)
                    lines.RemoveAt(0);

                if (lines.Count > 0) //this is because if the only text is space and the line is removed, there are no lines and returns null error
                {
                    //remove empty last line
                    if (lines[lines.Count - 1].gameObjects.Count == 0 && lines[lines.Count - 1].meshLayouts.Count == 0)
                        lines.RemoveAt(lines.Count - 1);
                }
            }


            return lines;
        }






        /// <summary>
        /// Used in all cases except when text has single mesh turned on
        /// </summary>
        /// <param name="bounds"></param>
        /// <returns></returns>
        private List<Line> GetLines(Bounds[] bounds)
        {
            List<Line> lines = new List<Line>();

            Line line = new Line();
            lines.Add(line);

            float x = 0;

            Modular3DText text = GetComponent<Modular3DText>();
            currentLetter = 0;
            currentWord = 0;
            currentLine = 0;

            bool checkedIfFits = false;

            for (int i = 0; i < transform.childCount; i++)
            {
                if (text)
                    if (!text.characterObjectList.Contains(transform.GetChild(i).gameObject))
                        continue;

                IsThisAnewWord(text, ref checkedIfFits, i);

                ///New line
                if (ItsALineBreak(lines, ref currentLine, ref x, i))
                {
                    currentLetter = 0;
                    currentWord++;
                    continue;
                }
                //transform.GetChild(i).gameObject.name = "Word: " + currentWord + " | Letter: " + currentLetter.ToString(); //for debugging

                if (IgnoreChildBound(bounds, i))
                    continue;

                //ignore spaces infront of line //TODO: add a setting in 3d text component
                //if (x == 0 && meshLayouts[i].mesh == null)
                //{
                //    continue;
                //}

#if UNITY_EDITOR
                lines[currentLine].widthIfNextObjectWasHere = x + Spacing.x + bounds[i].size.x;
#endif
                //Get current line number
                //Willl it be new line if current object is added
                //Debug.Log((float)Math.Round(x + Spacing.x + bounds[i].size.x, 5));
                if ((float)Math.Round(x + Spacing.x + bounds[i].size.x, 5) > Width) //New line 
                {
                    x = bounds[i].size.x + Spacing.x;
                    Line nline = new Line();
                    lines.Add(nline);
                    //Debug.Log(text.wordArray[word] + " word for: " + currentLetter + " letter for: " + transform.GetChild(i), transform.GetChild(i));
                    MoveWordToNextLine(bounds, lines, currentLine, ref x, text, currentLetter, currentWord, ref checkedIfFits, i);

                    currentLine++;
                }
                else // in the same line
                {
                    x += bounds[i].size.x + Spacing.x;
#if UNITY_EDITOR
                    lines[currentLine].width = x;
                    //if (x > Width)
                    //    Debug.Log(lines[currentLine].width);
                    //else
                    //    Debug.Log(Width);
#endif
                }

                lines[currentLine].gameObjects.Add(transform.GetChild(i).gameObject);
                currentLetter++;
            }


            if (text)
                lines = RemoveUnnecessarySpaces(lines);

            if (lines.Count > 0)
            {
                //remove empty first line
                if (lines[0].gameObjects.Count == 0 && lines[0].meshLayouts.Count == 0)
                    lines.RemoveAt(0);

                if (lines.Count > 0)
                {
                    //remove empty last line
                    if (lines[lines.Count - 1].gameObjects.Count == 0)
                        lines.RemoveAt(lines.Count - 1);
                }
            }

            return lines;
        }

        private void IsThisAnewWord(Modular3DText text, ref bool checkedIfFits, int childNumber)
        {
            if (text)
            {
                if (text.wordArray != null)
                {
                    if (text.wordArray.Length > currentWord)
                    {
                        if (text.wordArray[currentWord] == string.Empty)
                        {
                            currentLetter = 0;
                            currentWord++;
                            checkedIfFits = false;
                        }
                    }
                    if (text.wordArray.Length > currentWord)
                    {
                        if (currentLetter >= text.wordArray[currentWord].Length)
                        {
                            currentLetter = 0;
                            currentWord++;
                            checkedIfFits = false;
                        }
                    }
                }
            }
        }

        private bool ItsALineBreak(List<Line> lines, ref int currentLine, ref float x, int i)
        {
            if (transform.GetChild(i).GetComponent<LayoutElement>())
            {
                if (transform.GetChild(i).GetComponent<LayoutElement>().lineBreak)
                {
                    lines[currentLine].gameObjects.Add(transform.GetChild(i).gameObject);

                    x = 0;
                    Line nline = new Line();
                    lines.Add(nline);
                    currentLine++;

                    return true;
                }
            }
            return false;
        }




        #region Pos
        private float StartingX(List<MeshLayout> meshlayouts, float spaceRequired)
        {
            if (Anchor == Alignment.UpperLeft || Anchor == Alignment.MiddleLeft || Anchor == Alignment.LowerLeft)
                return (-Width / 2) - Spacing.x / 2;

            else if (Anchor == Alignment.UpperRight || Anchor == Alignment.MiddleRight || Anchor == Alignment.LowerRight)
            {
                if (!ApplyHorizontalJustice(spaceRequired))
                    return (Width / 2) - GetXSpaceRequired(meshlayouts) + Spacing.x / 2;
                else
                    return (-Width / 2) - Spacing.x / 2;
            }
            else
            {
                if (!ApplyHorizontalJustice(spaceRequired))
                    return -GetXSpaceRequired(meshlayouts) / 2;
                else
                    return (-Width / 2) - Spacing.x / 2;
            }
        }

        private float StartingX(List<GameObject> gameObjects, float spaceRequired)
        {
            if (Anchor == Alignment.UpperLeft || Anchor == Alignment.MiddleLeft || Anchor == Alignment.LowerLeft)
                return (-Width / 2) - Spacing.x / 2;

            else if (Anchor == Alignment.UpperRight || Anchor == Alignment.MiddleRight || Anchor == Alignment.LowerRight)
            {
                if (!ApplyHorizontalJustice(spaceRequired))
                    return (Width / 2) - GetXSpaceRequired(gameObjects) + Spacing.x / 2;
                else
                    return (-Width / 2) - Spacing.x / 2;
            }

            else
            {
                if (!ApplyHorizontalJustice(spaceRequired))
                    return -GetXSpaceRequired(gameObjects) / 2;
                else
                    return (-Width / 2) - Spacing.x / 2;
            }
        }

        private float GetXSpaceRequired(Line line)
        {
            if (line.gameObjects.Count > 0)
                return GetXSpaceRequired(line.gameObjects);
            else
                return GetXSpaceRequired(line.meshLayouts);
        }

        private float GetXSpaceRequired(List<GameObject> targets)
        {
            float width = 0;

            for (int i = 0; i < targets.Count; i++)
            {
                width += GetBound(targets[i].transform).size.x + Spacing.x;
            }

            return width;
        }

        private float GetXSpaceRequired(List<MeshLayout> targets)
        {
            float width = 0;

            for (int i = 0; i < targets.Count; i++)
            {
                width += GetBound(targets[i]).size.x + Spacing.x;
            }

            return width;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="linesCount"></param>
        /// <param name="spacingY">Spacing Y selected in the inspector</param>
        /// <param name="ySpace"></param>
        /// <returns></returns>
        private float StartingY(float ySpace)
        {
            Modular3DText text = GetComponent<Modular3DText>();

            if (IsUpperAlignment())
                return StartingYforUpperAlignment();

            else if (IsMiddleAlignment())
                return StartingYforMiddleAlignment(ySpace, text);

            else // (Anchor == Alignment.LowerLeft || Anchor == Alignment.LowerCenter || Anchor == Alignment.LowerRight)
                return StartingYforLowerAlignment(ySpace, text);

        }





        private float StartingYforUpperAlignment()
        {
            return (-Height / 2) - Spacing.y / 2;
        }
        private float StartingYforMiddleAlignment(float ySpace, Modular3DText text)
        {
            if (text || MyLineSpacingStyle != LineSpacingStyle.individual)
                return -(lines.Count * (Spacing.y + ySpace)) / 2;

            return -GetTotalYSpaceTakenByCheckingEachLineIndividually() / 2;
        }
        private float StartingYforLowerAlignment(float ySpace, Modular3DText text)
        {
            if (text || MyLineSpacingStyle != LineSpacingStyle.individual)
            {
                float ySize = lines.Count * (Spacing.y + ySpace);
                return (Height / 2) - ySize + Spacing.y / 2;
            }

            return (Height / 2) - GetTotalYSpaceTakenByCheckingEachLineIndividually() + Spacing.y / 2;
        }

        private float GetTotalYSpaceTakenByCheckingEachLineIndividually()
        {
            float ySpaceTaken = 0;
            for (int i = 0; i < lines.Count; i++)
            {
                ySpaceTaken += GetSpecificLineMaxYSpace(lines[i].gameObjects) + Spacing.y;
            }
            return ySpaceTaken;
        }

        private float GetMaxYSpace(Bounds[] bounds)
        {
            Modular3DText text = GetComponent<Modular3DText>();
            if (text)
                if (text.Font)
                {
                    return text.Font.lineHeight * text.FontSize.y / (Mathf.Clamp(text.Font.unitPerEM, 0.0001f, 1000000) * 8);
                }


            float maxY = 0;

            for (int i = 0; i < bounds.Length; i++)
            {
                if (bounds[i].size.y > maxY)
                {
                    if (!IgnoreChildBoundAndLineBreak(bounds, i))
                    {
                        maxY = bounds[i].size.y;
                    }
                }
            }

            return maxY;
        }

        /// <summary>
        /// Isn't used by text
        /// </summary>
        /// <param name="bounds"></param>
        /// <returns></returns>
        private float GetAverageYSpace(Bounds[] bounds)
        {
            float totalY = 0;
            float totalElements = 0;

            for (int i = 0; i < bounds.Length; i++)
            {
                if (!IgnoreChildBoundAndLineBreak(bounds, i))
                {
                    totalY += bounds[i].size.y;
                    totalElements++;
                }
            }

            return totalY / totalElements;
        }
        /// <summary>
        /// Isn't used by text
        /// </summary>
        /// <param name="bounds"></param>
        /// <returns></returns>
        private float GetSpecificLineMaxYSpace(List<GameObject> itemsInLine)
        {
            float maxY = 0;

            for (int i = 0; i < itemsInLine.Count; i++)
            {
                Bounds bound = GetBound(itemsInLine[i].transform);
                if (bound.size.y > maxY)
                {
                    if (!IgnoreChildBoundAndLineBreak(bounds, i))
                    {
                        maxY = bound.size.y;
                    }
                }
            }

            return maxY;
        }





        private bool IsUpperAlignment() => Anchor == Alignment.UpperLeft || Anchor == Alignment.UpperCenter || Anchor == Alignment.UpperRight;
        private bool IsMiddleAlignment() => Anchor == Alignment.MiddleLeft || Anchor == Alignment.MiddleCenter || Anchor == Alignment.MiddleRight;

        #endregion Pos



        #region  Text stuff
        private bool ItsALineBreak(List<MeshLayout> meshLayouts, List<Line> lines, ref int currentLine, ref float x, int i)
        {
            if (meshLayouts[i].lineBreak)
            {
                lines[currentLine].meshLayouts.Add(meshLayouts[i]);

                x = 0;
                Line nline = new Line();
                lines.Add(nline);
                currentLine++;

                return true;
            }
            return false;
        }

        private void MoveWordToNextLine(List<MeshLayout> meshLayouts, Bounds[] bounds, List<Line> lines, int currentLine, ref float x, Modular3DText text, int word, ref bool checkedIfFits, int i)
        {
            if (text)
            {
                if (currentLetter > 0)
                {
                    //Debug.Log("Current letter number: " + letter);
                    if (checkedIfFits == false)
                    {
#if UNITY_EDITOR
                        lines[currentLine].editorNote = "Checking if fits";
#endif
                        if (WordIsntTooBigForOneLine(bounds, text, i, word, currentLetter))
                        {
#if UNITY_EDITOR
                            lines[currentLine].editorNote = "Word isn't too big";
#endif
                            List<MeshLayout> temp = new List<MeshLayout>();

                            //if the a letter of a word is pushed to a new line,
                            //scroll through  all previous letters in current word
                            //and add them to the new line
                            for (int j = currentLetter; j >= 0; j--)
                            {
                                if (lines[currentLine].meshLayouts.Count > 0)
                                {
                                    MeshLayout g = lines[currentLine].meshLayouts[lines[currentLine].meshLayouts.Count - 1];
                                    lines[currentLine].meshLayouts.Remove(g);
                                    x += bounds[i - j].size.x + Spacing.x; //add the size of the letters for new word's calculation
                                    temp.Add(g);
                                }
                            }
                            for (int k = temp.Count - 1; k >= 0; k--)
                            {
                                lines[currentLine + 1].meshLayouts.Add(temp[k]);
                            }
                        }
                        else
                        {
                            //Debug.Log("Word too big to fit");
                        }
                    }
                }
            }
        }
        private void MoveWordToNextLine(Bounds[] bounds, List<Line> lines, int currentLine, ref float x, Modular3DText text, int letter, int word, ref bool checkedIfFits, int i)
        {
            if (text)
            {
                //Debug.Log(letter + " letter " + transform.GetChild(i) + " is on new line");
                if (letter > 0)
                {
                    if (checkedIfFits == false)
                    {
                        if (WordIsntTooBigForOneLine(bounds, text, i, word, letter))
                        {
                            List<GameObject> temp = new List<GameObject>();

                            //if the a letter of a word is pushed to a new line,
                            //scroll through  all previous letters in current word
                            //and add them to the new line
                            for (int j = letter; j >= 0; j--)
                            {
                                if (lines[currentLine].gameObjects.Count > 0)
                                {
                                    GameObject g = lines[currentLine].gameObjects[lines[currentLine].gameObjects.Count - 1];
                                    lines[currentLine].gameObjects.Remove(g);
                                    x += bounds[i - j].size.x + Spacing.x;
                                    temp.Add(g);
                                }
                            }

                            for (int k = temp.Count - 1; k >= 0; k--)
                            {
                                lines[currentLine + 1].gameObjects.Add(temp[k]);
                            }
                        }

                        checkedIfFits = true;
                    }
                }
            }
        }


        /// <summary>
        /// Bool because the updatelayout is called by update text
        /// If returns true, the layout has already been updated
        /// </summary>
        /// <returns></returns>
        private bool UpdateTextIfRequired()
        {
            if (GetComponent<Modular3DText>())
            {
                if (!GetComponent<Modular3DText>().ShouldItCreateChild())
                {
                    GetComponent<Modular3DText>().CleanUpdateText();
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// If the word is too big to fit in a single line
        /// </summary>
        /// <param name="bounds"></param>
        /// <param name="text"></param>
        /// <param name="currentChildNumber"></param>
        /// <param name="word"></param>
        /// <param name="letter"></param>
        /// <returns></returns>
        private bool WordIsntTooBigForOneLine(Bounds[] bounds, Modular3DText text, int currentChildNumber, int word, int letter)
        {

            if (text.wordArray == null)
                return false;


            if (word >= text.wordArray.Length)
            {
                //Debug.Log("word index:" + word + " | Text's word array length: " + text.wordArray.Length);
                return true;
            }

            int wordStartsAtChildIndex = currentChildNumber - letter;
            int wordEndssAtChildIndex = currentChildNumber - letter + text.wordArray[word].Length - 1;

            //Debug.Log("word starts at index: " + wordStartsAtChildIndex + " word ends at index:" + wordEndssAtChildIndex);

            float x = 0;
            for (int i = wordStartsAtChildIndex; i <= wordEndssAtChildIndex; i++)
            {
                if (bounds.Length <= i)
                {
                    return false;
                }

                if (bounds.Length > i)
                    x += bounds[i].size.x;

                if (x > Width)
                {
                    return false;
                }
            }

            return true;
        }
        private List<Line> RemoveUnnecessarySpaces(List<Line> lines, bool meshLayout)
        {
            for (int i = 0; i < lines.Count; i++)
            {
                if (lines[i].meshLayouts.Count > 0) //should never be 0. Just incase
                {
                    if (lines[i].meshLayouts[0].mesh == null)
                    {
                        lines[i].meshLayouts.RemoveAt(0);
                    }


                    if (lines[i].meshLayouts.Count > 1)
                    {
                        int lastIndex = lines[i].meshLayouts.Count - 1;

                        if (lines[i].meshLayouts[lastIndex].mesh == null)
                        {
                            lines[i].meshLayouts.RemoveAt(lastIndex);
                        }
                        else if (lines[i].meshLayouts[lastIndex].lineBreak)
                        {
                            lines[i].meshLayouts.RemoveAt(lastIndex);
                            lastIndex--;
                            if (lastIndex > 0)
                            {
                                if (lines[i].meshLayouts[lastIndex].mesh == null)
                                {
                                    lines[i].gameObjects.RemoveAt(lastIndex);
                                }
                            }
                        }
                    }
                }
                else
                {
                    lines.RemoveAt(i);
                    i--;
                }
            }

            return lines;
        }
        private List<Line> RemoveUnnecessarySpaces(List<Line> lines)
        {
            for (int i = 0; i < lines.Count; i++)
            {
                if (lines[i].gameObjects.Count > 0) //should never be 0. Just incase
                {
                    if (i != 0 && lines[i].gameObjects[0].name == "Space")
                    {
                        lines[i].gameObjects.RemoveAt(0);
                    }


                    if (lines[i].gameObjects.Count > 1)
                    {
                        int lastIndex = lines[i].gameObjects.Count - 1;

                        if (lines[i].gameObjects[lastIndex].name == "Space")
                        {
                            lines[i].gameObjects.RemoveAt(lastIndex);
                        }
                        else if (lines[i].gameObjects[lastIndex].name == "New Line")
                        {
                            lines[i].gameObjects.RemoveAt(lastIndex);
                            lastIndex--;
                            if (lastIndex > 0)
                            {
                                if (lines[i].gameObjects[lastIndex].name == "Space")
                                {
                                    lines[i].gameObjects.RemoveAt(lastIndex);
                                }
                            }
                        }
                    }
                }
                else
                {
                    lines.RemoveAt(i);
                    i--;
                }
            }

            return lines;
        }
        #endregion Text stuff

#if UNITY_EDITOR
        /// <summary>
        /// Draws the width and height
        /// </summary>
        private void OnDrawGizmos()
        {
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.color = new Color(1, 1, 1, 0.75f);

            Gizmos.DrawWireCube(Vector3.zero, new Vector3(Width + 0.1f, Height + 0.1f, 0.006f));
            //Gizmos.DrawWireCube(Vector3.zero, new Vector3(Width + 0.1f, Height + 0.1f, depth + 0.1f));
            //Gizmos.DrawCube(Vector3.zero, new Vector3(Width, Height, depth));
        }
#endif
    }





    /// <summary>
    /// Where the anchor of the text is placed.
    /// </summary>
    public enum Alignment
    {
        //
        // Summary:
        //     Text is anchored in upper left corner.
        UpperLeft = 0,
        //
        // Summary:
        //     Text is anchored in upper side, centered horizontally.
        UpperCenter = 1,
        //
        // Summary:
        //     Text is anchored in upper right corner.
        UpperRight = 2,
        //
        // Summary:
        //     Text is anchored in left side, centered vertically.
        MiddleLeft = 3,
        //
        // Summary:
        //     Text is centered both horizontally and vertically.
        MiddleCenter = 4,
        //
        // Summary:
        //     Text is anchored in right side, centered vertically.
        MiddleRight = 5,
        //
        // Summary:
        //     Text is anchored in lower left corner.
        LowerLeft = 6,
        //
        // Summary:
        //     Text is anchored in lower side, centered horizontally.
        LowerCenter = 7,
        //
        // Summary:
        //     Text is anchored in lower right corner.
        LowerRight = 8,
    }
    public enum LineSpacingStyle
    {
        maximum,
        average,
        individual
    }
}