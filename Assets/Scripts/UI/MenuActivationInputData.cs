using UnityEngine;
using System.Collections.Generic;
[CreateAssetMenu(fileName = "MenuActivationInputData", menuName = "ScriptableObjects/Menu Activation Input Data")]
public class MenuActivationInputData : ScriptableObject
{
    public List<string> acceptableWakeWords = new List<string>()
    {
        "menu",
        "activate menu",
        "hey cat",
        "hey kat",
        "hey cap",
        "hey you cap",
        "hey you can't",
        "hey you cat",
        "hey you kat",
        "hey, you cat",
        "hey, you kat"
    };
}
