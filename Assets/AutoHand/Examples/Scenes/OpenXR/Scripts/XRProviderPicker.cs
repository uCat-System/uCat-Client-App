using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Management;

[DefaultExecutionOrder(-10000)]
public class XRProviderPicker : MonoBehaviour
{
    //This will disable this script unless the given loader name matches the loader
    //(XRGeneralSettings.Instance.Manager.activeLoaders) in the XR Plugin Management window.
    //This is because different providers give different tracked positions.
    //Shouldn't matter for distribution of build, but does matter for distribution of this asset
    public string providerName = "";
    public XRHandOffset enableMe;
    public XRHandOffset disableMe;

    bool hasProvider = false;

    // Start is called before the first frame update
    void Start() {
        var loaders = XRGeneralSettings.Instance.Manager.activeLoaders;
        foreach(var loader in loaders) {
            if(providerName == "" || providerName == loader.name)
                hasProvider = true;
        }

        if (hasProvider)
        {
           // enableMe.AdjustPositions(disableMe);
            enableMe.enabled = true;
            disableMe.enabled = false;
        }
        else
        {

            disableMe.AdjustPositions(enableMe);
            enableMe.enabled = false;
            disableMe.enabled = true;
        }
    }


}
