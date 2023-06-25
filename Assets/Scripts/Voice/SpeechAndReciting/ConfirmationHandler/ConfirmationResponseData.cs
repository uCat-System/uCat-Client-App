using UnityEngine;

[CreateAssetMenu(fileName = "ConfirmationResponseData", menuName = "ScriptableObjects/Confirmation Response Data")]
public class ConfirmationResponseData : ScriptableObject
{
    public string positiveConfirmationResponse = "Cool!";
    public string negativeConfirmationResponse = "Oops, let's try again.";
    public string unknownConfirmationResponse = "Sorry, I didn't understand that. Please say yes or no.";   
}
