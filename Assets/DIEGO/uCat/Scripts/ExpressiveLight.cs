using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.tvOS;
using static Emotes;

public class ExpressiveLight : MonoBehaviour
{
    public Material expressiveMaterial;

    public enum ExpressiveColor
    {
        Red,
        Pink,
        Orange,
        Yellow,
        Green,
        Cyan,
        Blue,
        Purple,
        White
    }

    [SerializeField]
    private ExpressiveColor _currentExpressiveColor;

    public ExpressiveColor currentExpressiveColor
    {
        get
        {
            return _currentExpressiveColor;
        }
        set
        {
            _currentExpressiveColor = value;
            colorChange(_currentExpressiveColor, _EmissiveIntensity);
        }
    }

    [SerializeField]
    private float _EmissiveIntensity = 4f;

    public float EmissiveIntensity
    {
        get
        {
            return _EmissiveIntensity;
        }
        set
        {
            _EmissiveIntensity = value;
            colorChange(_currentExpressiveColor, _EmissiveIntensity);
        }
    }

    private ExpressiveColor colorCache;
    private float intensityCache;

    private float intensityPrepared;

    private void OnValidate()
    {
        currentExpressiveColor = _currentExpressiveColor;
        EmissiveIntensity = _EmissiveIntensity;
        colorChange(_currentExpressiveColor, _EmissiveIntensity);
    }

    private void Update()
    {
        colorChange(_currentExpressiveColor, _EmissiveIntensity);
    }

    public void colorChange(ExpressiveColor newColor, float newEmissiveIntensity)
    {
        if (colorCache == newColor && intensityCache == newEmissiveIntensity)
        {
            return;
        }

        colorCache = newColor;

        intensityPrepared = Mathf.Pow(2.0f, (newEmissiveIntensity - 0.4169f));

        if(newColor == ExpressiveColor.Red)
        {
            expressiveMaterial.SetColor("_EmissionColor", Color.red * intensityPrepared);
        }
        else if (newColor == ExpressiveColor.Pink)
        {
            expressiveMaterial.SetColor("_EmissionColor", Color.magenta * intensityPrepared);
        }
        else if (newColor == ExpressiveColor.Orange)
        {
            expressiveMaterial.SetColor("_EmissionColor", new Color(0.9f, 0.4f, 0.2f) * intensityPrepared);
        }
        else if (newColor == ExpressiveColor.Yellow)
        {
            expressiveMaterial.SetColor("_EmissionColor", new Color(0.8f, 0.7f, 0.1f) * intensityPrepared);
        }
        else if (newColor == ExpressiveColor.Green)
        {
            expressiveMaterial.SetColor("_EmissionColor", new Color(0.7f, 0.8f, 0.1f) * intensityPrepared);
        }
        else if (newColor == ExpressiveColor.Cyan)
        {
            expressiveMaterial.SetColor("_EmissionColor", new Color(0.01f, 0.7f, 0.7f) * intensityPrepared);
        }
        else if (newColor == ExpressiveColor.Blue)
        {
            expressiveMaterial.SetColor("_EmissionColor", new Color(0.01f, 0.05f, 0.9f) * intensityPrepared);
        }
        else if (newColor == ExpressiveColor.Purple)
        {
            expressiveMaterial.SetColor("_EmissionColor", new Color(0.05f, 0f, 1f) * intensityPrepared);
        }
        else if (newColor == ExpressiveColor.White)
        {
            expressiveMaterial.SetColor("_EmissionColor", new Color(0.6f, 0.6f, 0.6f) * intensityPrepared);
        }
        else 
        {
            expressiveMaterial.SetColor("_EmissionColor", Color.red * 100);
        }

    }

}
