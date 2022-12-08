using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Meta.WitAi;

public class GameLogicManager : MonoBehaviour
{
    // Start of the game: somehow activate the start of game (keypress etc)
    // We loop through a list of words to recite
    // turn on mic
    // listen for word
    // does it match wordsToRecite[i]?
    // do something if yes or no
    // ask next question if there's any left

    [SerializeField] private Wit wit;

    bool gameIsRunning = false;

    void Start()
    {
        // Debug - will eventually start through voice 
        gameIsRunning = true;

        wit.Activate();
    }


    void Update()
    {
        
    }
}
