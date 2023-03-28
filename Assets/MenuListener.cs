using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Meta.WitAi;


public class MenuListener : MonoBehaviour
{
    [SerializeField] private Wit wit;

    // Start is called before the first frame update

    void Awake()
    {
        // assign wit to the instance attached to this gameobject:
        wit = GetComponent<Wit>();
    }
    void Start()
    {
        wit.Activate();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

            public void HandlePartialTranscription(string text)
        {
            Debug.Log("Partial");
            Debug.Log(text);
            //partialText.text = text;
        }

     public void StartedListening()
        {
            Debug.Log("Started!");
        }

          public void StoppedListening()
        {
            Debug.Log("Stopped!");
        }

    public void HandleFullTranscription(string text)
    {
        Debug.Log("menu" + text);
        StartCoroutine(HandleTranscriptionThenWait(text));
        //StartCoroutine(HandleTranscriptionThenWait(text));
    }

     public IEnumerator HandleTranscriptionThenWait(string text)
        {
            Debug.Log("Full");
            Debug.Log(text);
        
            yield return new WaitForSeconds(0.000001f);
            wit.Activate();

        }
}
