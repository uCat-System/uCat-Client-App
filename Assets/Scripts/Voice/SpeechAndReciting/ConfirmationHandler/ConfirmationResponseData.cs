using UnityEngine;

[CreateAssetMenu(fileName = "ConfirmationResponseData", menuName = "ScriptableObjects/Confirmation Response Data")]
public class ConfirmationResponseData : ScriptableObject
{
    public string positiveConfirmationResponse = "Cool!";
    public AudioClip positiveConfirmationAudio;

    public string negativeConfirmationResponse = "Oops, let's try again.";
    public AudioClip negativeConfirmationAudio = null;

    public string unknownConfirmationResponse = "Sorry, I didn't understand that. Please say yes or no.";   
    public AudioClip unknownConfirmationAudio = null;

    public string positiveProceedResponse = "Proceeding!";
    public AudioClip positiveProceedAudio = null;

    public string negativeProceedResponse = "Ok, let's repeat.";
    public AudioClip negativeProceedAudio = null;
    
    public string unknownProceedResponse = "Sorry, I didn't understand that.";
    public AudioClip unknownProceedAudio = null;
}
