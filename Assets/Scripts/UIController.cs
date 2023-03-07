using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIController : MonoBehaviour
{
    // This is really the only blurb of code you need to implement a Unity singleton
    public static bool paused;
    private static UIController _Instance;

    public static UIController Instance
    {
        get
        {
            if (!_Instance)
            {
                _Instance = new GameObject().AddComponent<UIController>();
                // name it for easy recognition
                _Instance.name = _Instance.GetType().ToString();
                // mark root as DontDestroyOnLoad();
                DontDestroyOnLoad(_Instance.gameObject);
            }
            return _Instance;
        }
    }

    public void Start()
    {
        
    }

    public void EnableOrDisableUI()
    {
        paused = !paused;
        Debug.Log("Paused is now " + paused);
        if (paused)
        {
            Time.timeScale = 0;
        } else
        {
            Time.timeScale = 1;
        }
    }


    // implement your Awake, Start, Update, or other methods here...
}
