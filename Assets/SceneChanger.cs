using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{


    public void UpdateScene(string[] values)
    {
        Debug.Log("Inside updatescene");

        var changeString = values[0];
        var sceneString = values[1];

        Debug.Log(changeString);
        Debug.Log(sceneString);

        if (sceneString == "main menu" || sceneString == "menu") {
            //TODO --> check if scene actually exists
            SceneManager.LoadScene("menu");
        }

        if (sceneString == "one" || sceneString == "menu")
        {
            //TODO --> check if scene actually exists
            SceneManager.LoadScene("one");
        }

        if (sceneString == "home" || sceneString == "back home")
        {
            //TODO --> check if scene actually exists
            SceneManager.LoadScene("JammoScene");
        }


    }
}
