using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExampleScript : MonoBehaviour
{
    // You need to drag and drop the current uCat prefab instance from the scene inspector into this variable.
    public Animator uCatAnimator;

    // This is just a test button. We'll trigger a specific animation from here.
    // Luke, feel free to just copy/paste the code in the "if(triggerErrorAnimation)" to play any animations.
    public bool triggerErrorAnimation = false;

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
        if (uCatAnimator == null)
        {
            return;
        }

        
        if (triggerErrorAnimation)
        {
            // This is basically the only code you need to trigger an animation.
            // I called the animation "Error" both as a file and in the Animator.
            // As we continue adding animations, you will be able to call them from code by name.
            uCatAnimator.Play("Error", -1, 0);
        }
        else
        {
            // Another sanity check. Just default into the Idle animation as a fall back.
            // oR default into the Error animation to give you an in-game heads-up that you mistyped the name of an animation or something.
            uCatAnimator.Play("Idle", -1, 0);
        }

    }
}
