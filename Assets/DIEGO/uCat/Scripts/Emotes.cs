using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Emotes : MonoBehaviour
{

    public Material faceMaterial;
    public Texture faceBlink, faceHappy, faceNormal, faceSad, faceSurprised;
    public enum Emote
    {
        Blink,
        Happy,
        Normal, 
        Sad,
        Surprised
    }

    [SerializeField]
    private Emote _myEmote;

    public Emote myEmote
    {
        get
        {
            return _myEmote;
        }
        set 
        {
            _myEmote = value;
            faceChange(_myEmote);
        }
    }

    private Emote emoteCache;

    private void OnValidate()
    {
        myEmote = _myEmote;
    }

    private void Update()
    {
        faceChange(_myEmote);
    }

    public void faceChange(Emote faceName)
    {
        if (emoteCache == faceName)
        {
            return;
        }

        emoteCache = faceName;

        if (faceName == Emote.Blink)
        {
            faceMaterial.mainTexture = faceBlink ;
            faceMaterial.SetTexture("_EmissionMap", faceBlink);
        }
        else if (faceName == Emote.Happy)
        {
            faceMaterial.mainTexture = faceHappy;
            faceMaterial.SetTexture("_EmissionMap", faceHappy);
        }
        else if (faceName == Emote.Normal)
        {
            faceMaterial.mainTexture = faceNormal;
            faceMaterial.SetTexture("_EmissionMap", faceNormal);
        }
        else if (faceName == Emote.Sad)
        {
            faceMaterial.mainTexture = faceSad;
            faceMaterial.SetTexture("_EmissionMap", faceSad);
        }
        else if (faceName == Emote.Surprised)
        {
            faceMaterial.mainTexture = faceSurprised;
            faceMaterial.SetTexture("_EmissionMap", faceSurprised);
        }
        else 
        { 
            faceMaterial.mainTexture = faceNormal;
            faceMaterial.SetTexture("_EmissionMap", faceNormal);
        }
    }


}
