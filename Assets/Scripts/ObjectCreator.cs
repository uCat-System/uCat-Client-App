using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ObjectCreator : MonoBehaviour
{
    public GameObject createdCube;
    public GameObject createdSphere;
    public float spawnDistance;

    private GameObject player;


    void Start()
    {
        player = GameObject.FindWithTag("Player");
    }
    public void CreateObject(string[] values)
    {
        Debug.Log("Inside creator");

        var createString = values[0];
        var objectString = values[1];

        Debug.Log(createString);
        Debug.Log(objectString);

        GameObject createdObj;
        switch (objectString)
        {
            case "cube":
                {
                    createdObj = Instantiate(createdCube);
                    SetObjectProperties(createdObj);
                    break;
                }

            case "sphere":
                {
                    createdObj = Instantiate(createdSphere);
                    SetObjectProperties(createdObj);
                    break;
                }

             default:
                createdObj = null;
                break;
        }

        


    }
    void SetObjectProperties(GameObject createdObj)
    {
        // If player is tagged correctly and object exists, spawn object in front of them
        if (player && createdObj)
        {
            createdObj.transform.position
                = player.transform.position + new Vector3(0f, 0f, spawnDistance);
        }
    }
}
