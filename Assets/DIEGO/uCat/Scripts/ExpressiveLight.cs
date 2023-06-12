using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.tvOS;
using static Emotes;

public class ExpressiveLight : MonoBehaviour
{
    // We're using one single material for the neck and for the visor rim. Emission must be enabled for it to work.
    public Material expressiveMaterial;

    // We'll have a range of options to choose colors.
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

    // And let's allow people to set the intensity manually, just in case.
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

    // Emissive intensity can't just be plugged into color. You have to lower it a bit and multiply it by the power of two.
    // No idea why, Unity has very bad documentation on that regard.
    // Check these forum posts to see where I got the idea from: https://forum.unity.com/threads/setting-material-emission-intensity-in-script-to-match-ui-value.661624/
    // Anyway, we'll use this variable to store the number.
    private float intensityPrepared;

    //We'll be using the following section to allow updates to happen not only while we play but also on the editor.
    private ExpressiveColor colorCache;
    private float intensityCache;

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

    // Here's where the magic happens.
    public void colorChange(ExpressiveColor newColor, float newEmissiveIntensity)
    {
        // Let's cut this section short if, during the update, there has been no changes to the exposed values.
        if (colorCache == newColor && intensityCache == newEmissiveIntensity)
        {
            return;
        }
        // Let's make sure the cache is updated so the check above can do its job on the next update.
        colorCache = newColor;
        intensityCache = newEmissiveIntensity;

        // Here's where we prepare the intensity we're given to plug it into the new emissive color properly. 
        // The user should see no difference between the color they give and the Intensity value shown.
        intensityPrepared = Mathf.Pow(2.0f, (newEmissiveIntensity - 0.4169f));

        // Self explanatory, really. 
        // Tweak color values in a range from 0 to 1. Otherwise you'll have very intense colors if you go from 0 to 255.
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
            expressiveMaterial.SetColor("_EmissionColor", new Color(0.15f, 0f, 1f) * intensityPrepared);
        }
        else if (newColor == ExpressiveColor.White)
        {
            expressiveMaterial.SetColor("_EmissionColor", new Color(0.6f, 0.6f, 0.6f) * intensityPrepared);
        }
        else 
        {
            // This color is left intentionally bright to show something has gone horribly wrong.
            // Or that a new color that has not yet been assigned a color value has been selected.
            // Maybe we should be using switches at this point....? Or a Dictionnary?
            expressiveMaterial.SetColor("_EmissionColor", Color.red * 1000);
        }

    }

}
