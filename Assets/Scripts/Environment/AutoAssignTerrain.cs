using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoAssignTerrain : MonoBehaviour
{
    [SerializeField]
    private TerrainData terrainData;

    // Start is called before the first frame update
    void Start()
    {
        // if there is a Terrain Component on this game object, assign the data
        if (GetComponent<Terrain>() != null)
        {
            GetComponent<Terrain>().terrainData = terrainData;
        }

        // also assign the terrain collider information, in the terrain data slot
        if (GetComponent<TerrainCollider>() != null)
        {
            GetComponent<TerrainCollider>().terrainData = terrainData;
        }
    }
}
