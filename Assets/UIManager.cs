using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
     private static UIManager instance;

     public Animator animator;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public GameObject menu;

    public void ListenForMenuCommands(string text) {
        Debug.Log("Listening for menu commands: " + text);

        if (!menu.activeInHierarchy && text.Contains("activate menu") || text.Contains("hey you cat") 
            || text.Contains("hey you kat")) {
             menu.SetActive(true);
        }

if (menu.activeInHierarchy)
{
    switch (text)
    {
        case "repeat level":
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            break;
        case "next level":
           int nextBuildIndex = SceneManager.GetActiveScene().buildIndex + 1;
            if (nextBuildIndex < SceneManager.sceneCountInBuildSettings)
            {
                SceneManager.LoadScene(nextBuildIndex);
            }
            else
            {
                // Handle failure case when there is no next scene
                Debug.Log("No next scene available.");
            }
            break;
        case "quit":
            Application.Quit();
            break;
        case "restart":
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            break;
        case "resume": 
            animator.Play("CloseClip");
            StartCoroutine(WaitForAnimationToEnd());
            break;
        case ("level one"):
            SceneManager.LoadScene("Level1"); // Replace "Level1" with the name of your scene
            break;
        case "level two":
            SceneManager.LoadScene("Level2"); // Replace "Level2" with the name of your scene
            break;
        case "level three":
            SceneManager.LoadScene("Level3"); // Replace "Level3" with the name of your scene
            break;
        default:
            break;
    }
}
        
        
    }

     private System.Collections.IEnumerator WaitForAnimationToEnd()
    {
        Debug.Log("Waiting for animation to end");
        while (animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1)
        {
            Debug.Log("Animation playing");
            yield return null;
        }
        Debug.Log("Animation ended");
        menu.SetActive(false);
    }

    public void ActivateMenu() 
    {
        menu.SetActive(true);
    }
    // Start is called before the first frame update
    void Start()
    {
        menu.SetActive(false);
    }
}
