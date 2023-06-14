using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExampleScript : MonoBehaviour
{
    // You need to drag and drop the current uCat prefab instance from the scene inspector into this variable.
    public AnimationDriver catAnimationDriver;

    // This is just a test button. We'll trigger a specific animation from here.
    // Luke, feel free to just copy/paste the code in the "if(triggerErrorAnimation)" to play any animations.
    public bool triggerTestAnimation = false;

    // We'll be using OnValidate() instead of Update().
    // Otherwise animations will be forever starting and never moving on.
    // NEVER call an animation from Update().
    private void OnValidate()
    {
        triggerCatAnimation();
    }

    // This is the meat of the example.
    void triggerCatAnimation()
    {
        // Your boilerplate sanity check.
        if (catAnimationDriver == null)
        {
            return;
        }
  
        if (triggerTestAnimation)
        {
            // This is basically the only code you need to trigger an animation.
            // I know, it looks a bit redundant but it is easier this way in the long run.
            // To know the exact animations available to you as a programmer check the CatAnimations enum in AnimationDriver.cs
            catAnimationDriver.catAnimation = AnimationDriver.CatAnimations.Confused;
        }
        else
        {
            // Another sanity check. Just default into the Idle animation as a fall back.
            // Or default into the Error animation to give you an in-game heads-up that you mistyped the name of an animation or something.
            catAnimationDriver.catAnimation = AnimationDriver.CatAnimations.Idle;
        }

    }

}
