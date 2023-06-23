using UnityEngine;

namespace MText
{
    [CreateAssetMenu(fileName = "ConfirmationHandler", menuName = "MText/ConfirmationHandler")]
    public class ConfirmationHandlerSO : ScriptableObject
    {
        public string positiveConfirmationResponse;
        public string negativeConfirmationResponse;
        public string unknownConfirmationResponse;
        
        // Other serialized fields as needed
        
        // Getters for accessing the confirmation response strings
        public string PositiveConfirmationResponse => positiveConfirmationResponse;
        public string NegativeConfirmationResponse => negativeConfirmationResponse;
        public string UnknownConfirmationResponse => unknownConfirmationResponse;
    }
}
