using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteAlways]
public class PositionBasedOnLevel : MonoBehaviour
{
    public List<LevelPosition> levelPositions;
    private string currentLevel;

    private void Start()
    {
        currentLevel = SceneManager.GetActiveScene().name;
        PositionObjectBasedOnLevel();
    }

    private void PositionObjectBasedOnLevel()
    {
        // Check if the current level name matches one of your predetermined levels
        foreach (var levelPosition in levelPositions)
        {
            if (levelPosition.levelName == currentLevel)
            {
                transform.position = levelPosition.position;
                transform.rotation = levelPosition.rotation;
                break;
            }
        }
    }
}

[System.Serializable]
public class LevelPosition
{
    public string levelName;
    public Vector3 position;
    public Quaternion rotation;
}

#if UNITY_EDITOR
[CustomEditor(typeof(PositionBasedOnLevel))]
public class PositionBasedOnLevelEditor : Editor
{
    public override void OnInspectorGUI()
    {
        PositionBasedOnLevel positionScript = (PositionBasedOnLevel)target;

        // Draw the default inspector
        DrawDefaultInspector();

        GUILayout.Space(10);

        if (GUILayout.Button("Set Current Scene Position"))
        {
            Scene currentScene = SceneManager.GetActiveScene();
            LevelPosition levelPosition = new LevelPosition
            {
                levelName = currentScene.name,
                position = positionScript.transform.position,
                rotation = positionScript.transform.rotation
            };
            positionScript.levelPositions.Add(levelPosition);
        }
    }
}
#endif
