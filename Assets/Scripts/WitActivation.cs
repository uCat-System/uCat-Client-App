using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Meta.WitAi;
using UnityEngine.InputSystem;


public class WitActivation : MonoBehaviour
{
   [SerializeField] private Wit wit;

   private void OnValidate()
   {
       if (wit==null) wit = GetComponent<Wit>();
   }
   
    public void OpenXrTriggerPressed()
    {
        Debug.Log("TriggerPressed");
        WitActivate();
    }

   public void TriggerPressed(InputAction.CallbackContext context){
        Debug.Log("original");
        if (context.performed){
            WitActivate();
        }

   }
   public void WitActivate()
   {
        wit.Activate();   
   }

    // Use space to debug (luke)
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            wit.Activate();
            print("pressing space");
        }
    }
}
