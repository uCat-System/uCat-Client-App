using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationDriver : MonoBehaviour
{
    // This is the script which is going to be taking the animator and triggering animation sequences.
    private Animator catAnimator;

    // Remember to update this list and the switch below to extend the list of available animations.
    public enum CatAnimations
    {
        Idle,
        Happy,
        Surprised,
        Sad,
        Confused,
        Error

    }

    [SerializeField]
    private CatAnimations _catAnimation;

    public CatAnimations catAnimation
    {
        get
        {
            return _catAnimation;
        }
        set
        {
            _catAnimation = value;
            triggerCatAnimation(_catAnimation);
        }
    }

    // This is here to make the animation switch as soon as you choose it in the inspector while playing.
    // Animations don't show too well in Editing mode.
    private void OnValidate()
    {
        catAnimation = _catAnimation;
    }

    // We'll get our own Animator component because this script is in the prefab, right?
    void Start()
    {
        catAnimator = GetComponent<Animator>();
    }

    // Here is where we trigger the triggers that force transitions between animations.
    void triggerCatAnimation(CatAnimations calledAnimation)
    {
        // Your boilerplate sanity check.
        if (catAnimator == null)
        {
            return;
        }

        ResetAllTriggers();
        

        switch (calledAnimation)
        {
            case CatAnimations.Idle:
                // Let's remember that the Idle animation is there by default and NextAnim triggers the end of whatever loop we're in.
                catAnimator.SetTrigger("SlowBobbing");
                catAnimator.SetTrigger("NextAnim");
                break;
            case CatAnimations.Happy:
                catAnimator.SetTrigger("SlowBobbing");
                catAnimator.SetTrigger("Happy");
                break;
            case CatAnimations.Sad:
                catAnimator.SetTrigger("SlowBobbing");
                catAnimator.SetTrigger("Sad");
                break;
            case CatAnimations.Surprised:
                catAnimator.SetTrigger("StopBobbing");
                catAnimator.SetTrigger("Error");
                break;
            case CatAnimations.Confused:
                catAnimator.SetTrigger("SlowBobbing");
                catAnimator.SetTrigger("Confused");
                break;
            case CatAnimations.Error:
                catAnimator.SetTrigger("StopBobbing");
                catAnimator.SetTrigger("Error");
                break;
            default:
                // Let's keep the Error animation as the default to signal something has gone terribly wrong
                // or a trigger name has been misspelled in the cases above.
                catAnimator.SetTrigger("Error");
                break;
        }

    }

    // Triggers don't seem to autoreset themselves no matter what Unity documentation assures us on the contrary.
    // So, we reset all possible triggers and force the current loop to move on for good measure.
    private void ResetAllTriggers()
    {
        foreach (var param in catAnimator.parameters)
        {
            if (param.type == AnimatorControllerParameterType.Trigger)
            {
                catAnimator.ResetTrigger(param.name);
            }
        }

        catAnimator.SetTrigger("NextAnim");
        catAnimator.ResetTrigger("NextAnim");
    }
}
