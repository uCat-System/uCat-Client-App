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

    // We'll be using this variable to store uCat's Transform for the rotation example.
    private Transform uCatInstance;

    private void Start()
    {
        // We just get the Transform component from the very uCat where we got the driver from. It's a bit sneaky, but it works. 
        // For more elegance, store the uCat GameObject as a variable and then look for component like this: uCatInstance.GetComponent<AnimationDriver>();
        uCatInstance = catAnimationDriver.GetComponentInParent<Transform>();
    }

    private void Update()
    {
        // Calling this all the time may consume resources, so maybe have a bool checking if this method is necessary.
        // It might require some work checking if uCat has reached the target or not.
        // Due to how Slerp works, this must always be triggered from the Update() function.
        // Consider installing a Lerp package to have more robust tools for smoothly moving uCat's transfrom from the code.
        triggerCatRotation(triggerTestAnimation);
    }

    // We'll be using OnValidate() instead of Update().
    // Otherwise animations will be forever starting and overlapping and never moving on.
    // NEVER call an animation from Update().
    private void OnValidate()
    {
        triggerCatAnimation();
    }

    // This is the example to rotate uCat from code, in case, you'd like to make it took at the camera or elsewhere in the scene.\
    // In this example I am taking the boolean value to make uCat turn when the Test Animation is triggered. 
    void triggerCatRotation(bool direction)
    {
        // This value will dictate to which value in Y uCat will be rotating.
        float targetRot;

        if (direction) 
        { 
            // If the test animation is triggered, uCat will rotate to (0,35,0) to look at the camera.
            targetRot = 35f; 
        }
        else
        {
            // Otherwise, uCat will go back to (0,0,0) to its initial default rotation.
            targetRot = 0f;
        }

        // To understand what this code means, check this quick documentation: https://docs.unity3d.com/ScriptReference/Quaternion.Slerp.html
        uCatInstance.rotation = Quaternion.Slerp(uCatInstance.rotation, Quaternion.Euler(uCatInstance.rotation.x, uCatInstance.rotation.y + targetRot, uCatInstance.rotation.z), Time.deltaTime * 3.0f);
    }

    // This is the meat of the animation example.
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
            catAnimationDriver.catAnimation = AnimationDriver.CatAnimations.Happy;
            
        }
        else
        {
            // Another sanity check. Just default into the Idle animation as a fall back.
            // Or default into the Error animation to give you an in-game heads-up that you mistyped the name of an animation or something.
            catAnimationDriver.catAnimation = AnimationDriver.CatAnimations.Idle;

        }

    }

}
